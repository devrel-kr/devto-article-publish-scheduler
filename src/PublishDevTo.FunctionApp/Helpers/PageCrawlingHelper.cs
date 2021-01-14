using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevRelKr.PublishDevTo.FunctionApp.Helpers
{
    /// <summary>
    /// This represents the helper entity to handle the web page scraping.
    /// </summary>
    public class PageCrawlingHelper : IPageCrawlingHelper
    {
        private readonly HttpClient _http;
        private readonly Regex _regex;

        public PageCrawlingHelper(HttpClient http, Regex regex)
        {
            this._http = http ?? throw new ArgumentNullException(nameof(http));
            this._regex = regex ?? throw new ArgumentNullException(nameof(regex));
        }

        /// <inheritdoc />
        public async Task<T> ScrapeAsync<T>(string pageUri)
        {
            return await this.ScrapeAsync<T>(new Uri(pageUri));
        }

        /// <inheritdoc />
        public async Task<T> ScrapeAsync<T>(Uri pageUri)
        {
            if (pageUri == null)
            {
                throw new ArgumentNullException(nameof(pageUri));
            }

            var html = await this._http.GetStringAsync(pageUri);
            var match = this._regex.Match(html);
            var value = (T)Convert.ChangeType(match.Groups[1].Value, typeof(T));

            return value;
        }
    }
}
