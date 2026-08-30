using System.Text;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace Gnezdow.MarkdigTextMate;

public class TextMateCodeBlockRenderer(CodeBlockRenderer wrapping, RegistryOptions options) : HtmlObjectRenderer<CodeBlock>
{
    protected override void Write(HtmlRenderer renderer, CodeBlock obj)
    {
        if (obj is not FencedCodeBlock { Info: { } info })
        {
            wrapping.Write(renderer, obj);
            return;
        }

        var registry = new Registry(options);
        var theme = registry.GetTheme();

        string? scope = options.GetScopeByLanguageId(info);

        if (scope == null) 
            scope = options.GetScopeByExtension(info);

        if (scope == null)
        {
            wrapping.Write(renderer, obj);
            return;
        }
        
        var grammar = registry.LoadGrammar(scope);

        if (grammar == null)
        {
            wrapping.Write(renderer, obj);
            return;
        }
        
        var attributes = new HtmlAttributes();
        attributes.AddClass("language-" + info);

        renderer.Write("<pre>");
        renderer.Write("<code");
        renderer.WriteAttributes(attributes);
        renderer.Write(">");

        IStateStack? stack = null;
        foreach (var line in obj.Lines.Cast<StringLine>())
        {
            if (!line.Slice.IsEmpty)
            {
                var res = grammar.TokenizeLine(line.Slice.ToString(), stack, TimeSpan.MaxValue);
                stack = res.RuleStack;

                foreach (var token in res.Tokens)
                {
                    int foreground = -1;
                    int background = -1;
                    FontStyle fontStyle = FontStyle.NotSet;

                    foreach (var themeRule in theme.Match(token.Scopes))
                    {
                        if (foreground == -1 && themeRule.foreground > 0)
                            foreground = themeRule.foreground;

                        if (background == -1 && themeRule.background > 0)
                            background = themeRule.background;

                        if (fontStyle == FontStyle.NotSet && themeRule.fontStyle > 0)
                            fontStyle = themeRule.fontStyle;
                    }

                    StringBuilder styleBuilder = new();

                    if (foreground != -1) styleBuilder.Append($"color: {theme.GetColor(foreground)};");
                    if (background != -1) styleBuilder.Append($"background-color: {theme.GetColor(background)};");

                    if (fontStyle != FontStyle.NotSet)
                    {
                        if ((fontStyle & FontStyle.Bold) != 0) styleBuilder.Append("font-weight: bolder;");
                        if ((fontStyle & FontStyle.Italic) != 0) styleBuilder.Append("font-style: italic;");
                        if ((fontStyle & FontStyle.Underline) != 0) styleBuilder.Append("text-decoration: underline;");
                        if ((fontStyle & FontStyle.Strikethrough) != 0)
                            styleBuilder.Append("text-decoration: line-through;");
                    }

                    HtmlAttributes spanAttributes = new();
                    spanAttributes.AddProperty("style", styleBuilder.ToString());

                    renderer.Write("<span");
                    renderer.WriteAttributes(spanAttributes);
                    renderer.Write(">");

                    renderer.WriteEscape(line.Slice.ToString()
                        .Substring(token.StartIndex, token.EndIndex - token.StartIndex));

                    renderer.Write("</span>");
                }
            }

            renderer.Write("<br>");
        }
        
        renderer.Write("</code>");
        renderer.Write("</pre>");
    }
}