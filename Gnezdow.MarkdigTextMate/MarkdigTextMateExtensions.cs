using Markdig;
using TextMateSharp.Grammars;

namespace Gnezdow.MarkdigTextMate;

public static class MarkdigTextMateExtensions
{
    public static MarkdownPipelineBuilder UseTextMate(this MarkdownPipelineBuilder pipeline,
        RegistryOptions registryOptions)
    {
        pipeline.Extensions.Add(new MarkdigTextMateExtension(registryOptions));
        return pipeline;
    }
}