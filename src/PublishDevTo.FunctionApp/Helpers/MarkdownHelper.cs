using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevRelKr.PublishDevTo.FunctionApp.Models;

using YamlDotNet.Serialization;

namespace DevRelKr.PublishDevTo.FunctionApp.Helpers
{
    /// <summary>
    /// This represents the helper entity to handle the markdown document.
    /// </summary>
    public class MarkdownHelper : IMarkdownHelper
    {
        private readonly ISerializer _serialiser;
        private readonly IDeserializer _deserialiser;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownHelper" /> class.
        /// </summary>
        /// <param name="serialiser"><see cref="ISerializer" /> instance.</param>
        /// <param name="deserialiser"><see cref="IDeserializer" /> instance.</param>
        public MarkdownHelper(ISerializer serialiser, IDeserializer deserialiser)
        {
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this._deserialiser = deserialiser ?? throw new ArgumentNullException(nameof(deserialiser));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> SplitMarkdownDocumentAsync(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return new string[0];
            }

            var result = await Task.Factory.StartNew(() =>
            {
                var segments = markdown.Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(p => p.Trim());

                var frontmatter = segments.First();
                var body = string.Join("\n---\n", segments.Skip(1));

                return new[] { frontmatter, body };
            });

            return result;
        }

        /// <inheritdoc />
        public async Task<FrontMatter> DeserialiseFrontMatterAsync(string frontmatter)
        {
                var deserialised = await Task.Factory.StartNew(() =>
                {
                     return this._deserialiser.Deserialize<FrontMatter>(frontmatter);
                });

                return deserialised;
        }

        /// <inheritdoc />
        public async Task<string> SerialiseFrontMatterAsync(FrontMatter frontmatter)
        {
            var serialised = await Task.Factory.StartNew(() =>
            {
                return this._serialiser.Serialize(frontmatter);
            });

            return serialised;
        }

        /// <inheritdoc />
        public async Task<string> MergeMarkdownDocumentAsync(string frontmatter, string body)
        {
            var markdown = await Task.Factory.StartNew(() =>
            {
                var sb = new StringBuilder()
                             .AppendLine("---")
                             .AppendLine(frontmatter)
                             .AppendLine("---")
                             .AppendLine(body);

                return sb.ToString();
            });

            return markdown;
        }
    }
}
