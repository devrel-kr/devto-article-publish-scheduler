using System;

using Newtonsoft.Json;

namespace DevRelKr.PublishDevTo.FunctionApp.Models
{
    /// <summary>
    /// This represents the request entity for scheduling.
    /// </summary>
    public class SchedulingRequest
    {
        /// <summary>
        /// Gets or sets the URL of the dev.to article preview.
        /// </summary>
        [JsonProperty("previewUri", Required = Required.Always)]
        public virtual Uri PreviewUri { get; set; }

        /// <summary>
        /// Gets or sets the schedule to publish.
        /// </summary>
        [JsonProperty("schedule", Required = Required.Always)]
        public virtual DateTimeOffset Schedule { get; set; }
    }
}
