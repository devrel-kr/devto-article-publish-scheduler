using System.Threading.Tasks;

using DevRelKr.PublishDevTo.FunctionApp.Models;

namespace DevRelKr.PublishDevTo.FunctionApp.Services
{
    /// <summary>
    /// This provides interfaces to <see cref="ArticleService" /> class.
    /// </summary>
    public interface IArticleService
    {
        /// <summary>
        /// Publishes article.
        /// </summary>
        /// <param name="req"><see cref="SchedulingRequest" /> instance.</param>
        /// <returns>Returns <c>True</c>, if published successfully; otherwise returns <c>False</c>.</returns>
        Task<bool> PublishArticleAsync(SchedulingRequest req);
    }
}
