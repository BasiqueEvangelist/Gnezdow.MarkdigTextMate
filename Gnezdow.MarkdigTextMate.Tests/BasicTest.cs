using Markdig;
using TextMateSharp.Grammars;

namespace Gnezdow.MarkdigTextMate.Tests;

public class BasicTest
{
    [Fact]
    public void CSharpFormattingWorksFine()
    {
        string test = """
                      ```csharp
                      using System;
                      
                      namespace TestProject;
                      
                      public class Program
                      {
                          public static void Main(string[] args)
                          {
                              Console.WriteLine("Meow!");
                          }
                      }
                      ```
                      """;
        
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseTextMate(new RegistryOptions(ThemeName.LightPlus))
            .Build();
        
        var html = Markdown.ToHtml(test, pipeline);
        
        Assert.Equal("<pre><code class=\"language-csharp\"><span style=\"color: #0000FF;\">using</span><span style=\"\"> </span><span style=\"color: #267F99;\">System</span><span style=\"\">;</span><br><br><span style=\"color: #0000FF;\">namespace</span><span style=\"\"> </span><span style=\"color: #267F99;\">TestProject</span><span style=\"\">;</span><br><br><span style=\"color: #0000FF;\">public</span><span style=\"\"> </span><span style=\"color: #0000FF;\">class</span><span style=\"\"> </span><span style=\"color: #267F99;\">Program</span><br><span style=\"\">{</span><br><span style=\"\">    </span><span style=\"color: #0000FF;\">public</span><span style=\"\"> </span><span style=\"color: #0000FF;\">static</span><span style=\"\"> </span><span style=\"color: #0000FF;\">void</span><span style=\"\"> </span><span style=\"color: #795E26;\">Main</span><span style=\"\">(</span><span style=\"color: #0000FF;\">string</span><span style=\"\">[</span><span style=\"\">]</span><span style=\"\"> </span><span style=\"color: #001080;\">args</span><span style=\"\">)</span><br><span style=\"\">    </span><span style=\"\">{</span><br><span style=\"\">        </span><span style=\"color: #001080;\">Console</span><span style=\"\">.</span><span style=\"color: #795E26;\">WriteLine</span><span style=\"\">(</span><span style=\"color: #A31515;\">&quot;</span><span style=\"color: #A31515;\">Meow!</span><span style=\"color: #A31515;\">&quot;</span><span style=\"\">)</span><span style=\"\">;</span><br><span style=\"\">    </span><span style=\"\">}</span><br><span style=\"\">}</span><br></code></pre>", html);
    }
}
