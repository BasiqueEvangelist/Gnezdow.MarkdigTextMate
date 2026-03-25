using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;

namespace Gnezdow.MarkdigTextMate;

public class MarkdigTextMateExtension(RegistryOptions registryOptions) : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline) { }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not TextRendererBase<HtmlRenderer> html) return;

        var original = html.ObjectRenderers.FindExact<CodeBlockRenderer>();

        html.ObjectRenderers.ReplaceOrAdd<CodeBlockRenderer>(new TextMateCodeBlockRenderer(original ?? new CodeBlockRenderer(), registryOptions));
    }
}