using System;

using Newtonsoft.Json;

using YamlDotNet.Serialization;

namespace DevRelKr.PublishDevTo.FunctionApp.Models
{
    /// <summary>
    /// This represents the frontmatter entity.
    /// </summary>
    public class FrontMatter
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonProperty("title")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the article is published or not.
        /// </summary>
        [JsonProperty("published")]
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [JsonProperty("description")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the comma delimited list of tags.
        /// </summary>
        [JsonProperty("tags")]
        public virtual string Tags { get; set; }

        /// <summary>
        /// Gets or sets the canonical URL.
        /// </summary>
        [JsonProperty("canonical_url")]
        [YamlMember(typeof(string))]
        public virtual Uri CanonicalUrl { get; set; }

        /// <summary>
        /// Gets or sets the cover image URL.
        /// </summary>
        [JsonProperty("cover_image")]
        [YamlMember(typeof(string))]
        public virtual Uri CoverImage { get; set; }

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        [JsonProperty("series")]
        public virtual string Series { get; set; }
    }
}
