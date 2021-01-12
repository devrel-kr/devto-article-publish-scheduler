using System;
using System.Text;
using System.Threading.Tasks;

using DevRelKr.PublishDevTo.FunctionApp.Models;
using DevRelKr.PublishDevTo.FunctionApp.Services;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DevRelKr.PublishDevTo.FunctionApp
{
    /// <summary>
    /// This represents the activity trigger entity for scheduling.
    /// </summary>
    public class SchedulingActivityTrigger
    {
        private readonly IArticleService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingActivityTrigger" /> class.
        /// </summary>
        /// <param name="service"><see cref="IArticleService" /> instance.</param>
        public SchedulingActivityTrigger(IArticleService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Invokes the activity trigger for scheduling.
        /// </summary>
        /// <param name="input"><see cref="SchedulingRequest" /> instance.</param>
        /// <param name="context"><see cref="ExecutionContext" /> instance.</param>
        /// <param name="log"><see cref="ILogger" /> instance.</param>
        /// <returns>Returns <see cref="SchedulingResponse" /> instance.</returns>
        [FunctionName(nameof(SchedulingActivityTrigger.PublishArticleAsync))]
        public async Task<SchedulingResponse> PublishArticleAsync(
            [ActivityTrigger] SchedulingRequest input,
            ExecutionContext context,
            ILogger log)
        {
            var invocationId = context.InvocationId;

            log.LogInformation("Article publish activity has started.");
            log.LogInformation($"InvocationID: {invocationId}");

            var result = default(bool);
            var response = default(SchedulingResponse);
            try
            {
                result = await this._service
                                   .PublishArticleAsync(input);

                response = new SchedulingResponse()
                {
                    Published = result,
                    Message = $"Article published at {input.Schedule:yyyy-MM-ddTHH:mm:sszzzz}"
                };
            }
            catch(Exception ex)
            {
                log.LogError(ex.Message);
                log.LogError(ex.StackTrace);

                var sb = new StringBuilder()
                             .AppendLine($"Message: {ex.Message}")
                             .AppendLine($"StackTrace: {ex.StackTrace}");

                response = new SchedulingResponse()
                {
                    Published = false,
                    Message = sb.ToString()
                };
            }

            log.LogInformation($"Article has {(result ? string.Empty : "NOT ")}been published.");

            return response;
        }
   }
}
