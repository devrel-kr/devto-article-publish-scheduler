using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Aliencube.Forem.DevTo;
using Aliencube.Forem.DevTo.Models;

using DevRelKr.PublishDevTo.FunctionApp.Helpers;
using DevRelKr.PublishDevTo.FunctionApp.Models;

namespace DevRelKr.PublishDevTo.FunctionApp.Services
{
    /// <summary>
    /// This represents the service entity to handle the article.
    /// </summary>
    public class ArticleService : IArticleService
    {
        private readonly AppSettings _settings;
        private readonly DEVAPIbeta _dev;
        private readonly IPageCrawlingHelper _page;
        private readonly IMarkdownHelper _markdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleService" /> class.
        /// </summary>
        /// <param name="settings"><see cref="AppSettings" /> instance.</param>
        /// <param name="dev"><see cref="IDEVAPIbeta" /> instance.</param>
        /// <param name="page"><see cref="IPageCrawlingHelper" /> instance.</param>
        /// <param name="markdown"><see cref="IMarkdownHelper" /> instance.</param>
        public ArticleService(AppSettings settings, IDEVAPIbeta dev, IPageCrawlingHelper page, IMarkdownHelper markdown)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._dev = (dev as DEVAPIbeta) ?? throw new ArgumentNullException(nameof(dev));
            this._page = page ?? throw new ArgumentNullException(nameof(page));
            this._markdown = markdown ?? throw new ArgumentNullException(nameof(markdown));
        }

        /// <inheritdoc />
        public async Task<bool> PublishArticleAsync(SchedulingRequest req)
        {
            if (req == null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            var articleId = int.TryParse(await this._page
                                                   .ScrapeAsync<string>(req.PreviewUri),
                                         out int result) ? result : 0;
            if (articleId <= 0)
            {
                return false;
            }

            this._dev.HttpClient.DefaultRequestHeaders.Add("api-key", this._settings.DevTo.ApiKey);

            var article = ((await this._dev
                                      .GetUserUnpublishedArticlesAsync()) as IEnumerable<ArticleMe>)
                          .SingleOrDefault(p => p.Id == articleId);

            if (article == null)
            {
                return false;
            }

            var segments = await this._markdown
                                     .SplitMarkdownDocumentAsync(article.BodyMarkdown);

            var frontmatter = await this._markdown
                                        .DeserialiseFrontMatterAsync(segments.First());
            frontmatter.Published = true;

            var serialised = await this._markdown
                                       .SerialiseFrontMatterAsync(frontmatter);

            var markdown = await this._markdown
                                     .MergeMarkdownDocumentAsync(serialised, segments.Last());

            var updated = new ArticleUpdateArticle()
            {
                BodyMarkdown = markdown,
            };

            await this._dev
                      .UpdateArticleAsync(articleId, new ArticleUpdate(updated));

            return true;
        }
    }
}
