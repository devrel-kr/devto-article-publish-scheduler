using System;
using System.Threading.Tasks;

namespace DevRelKr.PublishDevTo.FunctionApp.Helpers
{
    /// <summary>
    /// This provides interfaces to <see cref="PageCrawlingHelper" /> class.
    /// </summary>
    public interface IPageCrawlingHelper
    {
        /// <summary>
        /// Scrapes the web page with given selector and page function.
        /// </summary>
        /// <param name="pageUri">Web page URI to scrape.</param>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <returns>Returns the scraped value.</returns>
        Task<T> ScrapeAsync<T>(string pageUri);

        /// <summary>
        /// Scrapes the web page with given selector and page function.
        /// </summary>
        /// <param name="pageUri">Web page URI to scrape.</param>
        /// <typeparam name="T">Type of return value.</typeparam>
        /// <returns>Returns the scraped value.</returns>
        Task<T> ScrapeAsync<T>(Uri pageUri);
    }
}
