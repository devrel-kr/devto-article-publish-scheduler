namespace DevRelKr.PublishDevTo.FunctionApp.Models
{
    /// <summary>
    /// This represens the response entity for scheduling.
    /// </summary>
    public class SchedulingResponse
    {
        /// <summary>
        /// Gets or sets the value indicating whether the article is published or not.
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets the scheduling result message.
        /// </summary>
        public virtual string Message { get; set; }
    }
}
