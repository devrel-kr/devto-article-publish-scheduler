using System;
using System.Threading;
using System.Threading.Tasks;

using DevRelKr.PublishDevTo.FunctionApp.Models;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace DevRelKr.PublishDevTo.FunctionApp
{
    public class SchedulingOrchestrationTrigger
    {
        // Set the threshold days to 28 days (4 weeks).
        private static TimeSpan threshold = new TimeSpan(28, 0, 0, 0);

        private readonly AppSettings _settings;

        public SchedulingOrchestrationTrigger(AppSettings settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        [FunctionName(nameof(SchedulingOrchestrationTrigger.SetScheduleOrchestrationAsync))]
        public async Task<SchedulingResponse> SetScheduleOrchestrationAsync(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestration,
            ExecutionContext context,
            ILogger log)
        {
            var invocationId = context.InvocationId;

            log.LogInformation("Scheduling orchestration was invoked.");
            log.LogInformation($"InvocationID: {invocationId}");

            var input = orchestration.GetInput<SchedulingRequest>();

            var maxDuration = this._settings.Scheduler.MaxDuration;
            if (maxDuration > threshold)
            {
                return new SchedulingResponse()
                {
                    Published = false,
                    Message = $"Maximum scheduling days cannot exceed {threshold.Days} from now."
                };
            }

            // Get the scheduled time
            var scheduled = input.Schedule.UtcDateTime;

            // Get the function initiated time.
            var initiated = orchestration.CurrentUtcDateTime;

            // Get the difference between now and schedule
            var datediff = (TimeSpan)(scheduled - initiated);

            // Complete if datediff is longer than the max duration
            if (datediff >= maxDuration)
            {
                return new SchedulingResponse()
                {
                    Published = false,
                    Message = $"The schedule is further than the maximum days of {maxDuration.Days} from now."
                };
            }

            await orchestration.CreateTimer(scheduled, CancellationToken.None);

            var activityFunctionName = nameof(SchedulingActivityTrigger.PublishArticleAsync);
            var output = await orchestration.CallActivityAsync<SchedulingResponse>(functionName: activityFunctionName, input: input);

            return output;
        }
    }
}
