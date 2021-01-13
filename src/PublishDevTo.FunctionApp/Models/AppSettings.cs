using System;
using System.Globalization;

namespace DevRelKr.PublishDevTo.FunctionApp.Models
{
    /// <summary>
    /// This represents the app settings entity.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets the <see cref="SchedulerSettings" /> instance.
        /// </summary>
        public virtual SchedulerSettings Scheduler { get; } = new SchedulerSettings();

        /// <summary>
        /// Gets the <see cref="DevToSettings" /> instance.
        /// </summary>
        public virtual DevToSettings DevTo { get; } = new DevToSettings();
    }

    /// <summary>
    /// This represents the app settings entity for scheduling.
    /// </summary>
    public class SchedulerSettings
    {
        /// <summary>
        /// Gets the <see cref="TimeSpan" /> value.
        /// </summary>
        public virtual TimeSpan MaxDuration { get; } = TimeSpan.TryParse(Environment.GetEnvironmentVariable("Scheduler__MaxDuration"), CultureInfo.InvariantCulture, out TimeSpan result)
                                                       ? result
                                                       : new TimeSpan(7, 0, 0, 0);
    }

    /// <summary>
    /// This represents the app settings entity for dev.to.
    /// </summary>
    public class DevToSettings
    {
        /// <summary>
        /// Gets the API key to dev.to.
        /// </summary>
        public virtual string ApiKey { get; } = Environment.GetEnvironmentVariable("DevTo__ApiKey");

        /// <summary>
        /// Gets the <see cref="DevToArticleSettings" /> instance.
        /// </summary>
        public virtual DevToArticleSettings Article { get; } = new DevToArticleSettings();
    }

    /// <summary>
    /// This represents the app settings entity for dev.to article.
    /// </summary>
    public class DevToArticleSettings
    {
        /// <summary>
        /// Gets the HTML selector value.
        /// </summary>
        public virtual string Selector { get; } = Environment.GetEnvironmentVariable("DevTo__Article__Selector");

        /// <summary>
        /// Gets the page function expression value.
        /// </summary>
        public virtual string PageFunction { get; } = Environment.GetEnvironmentVariable("DevTo__Article__PageFunction");

        /// <summary>
        /// Gets the page function expression value.
        /// </summary>
        public virtual string IdPattern { get; } = Environment.GetEnvironmentVariable("DevTo__Article__IdPattern");
    }
}
