using System.Collections.Generic;
using System.Threading.Tasks;

using DevRelKr.PublishDevTo.FunctionApp.Models;

namespace DevRelKr.PublishDevTo.FunctionApp.Helpers
{
    /// <summary>
    /// This provides interfaces to <see cref="MarkdownHelper" /> class.
    /// </summary>
    public interface IMarkdownHelper
    {
        /// <summary>
        /// Splits the markdown document into both frontmatter and body.
        /// </summary>
        /// <param name="markdown">Markdown document.</param>
        /// <returns>Returns the list of string containing both frontmatter and body.</returns>
        Task<IEnumerable<string>> SplitMarkdownDocumentAsync(string markdown);

        /// <summary>
        /// Deserialises the frontmatter YAML into strongly-typed instance.
        /// </summary>
        /// <param name="frontmatter">Frontmatter YAML.</param>
        /// <returns>Returns <see cref="FrontMatter" /> instance.</returns>
        Task<FrontMatter> DeserialiseFrontMatterAsync(string frontmatter);

        /// <summary>
        /// Serialises the <see cref="FrontMatter" /> instance into the frontmatter YAML.
        /// </summary>
        /// <param name="frontmatter"><see cref="FrontMatter" /> instance.</param>
        /// <returns>Returns the frontmatter YAML.</returns>
        Task<string> SerialiseFrontMatterAsync(FrontMatter frontmatter);

        /// <summary>
        /// Merges the frontmatter and the markdown body.
        /// </summary>
        /// <param name="frontmatter">Frontmatter YAML.</param>
        /// <param name="body">Markdown body.</param>
        /// <returns>Returns the merged markdown document.</returns>
        Task<string> MergeMarkdownDocumentAsync(string frontmatter, string body);
    }
}
