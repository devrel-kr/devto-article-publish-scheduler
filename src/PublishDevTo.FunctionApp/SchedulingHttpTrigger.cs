using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using DevRelKr.PublishDevTo.FunctionApp.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DevRelKr.PublishDevTo.FunctionApp
{
    /// <summary>
    /// This represents the HTTP trigger entity for scheduling.
    /// </summary>
    public class SchedulingHttpTrigger
    {
        private readonly JsonSerializerSettings _serialiser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingActivityTrigger" /> class.
        /// </summary>
        /// <param name="serialiser"><see cref="JsonSerializerSettings" /> instance.</param>
        public SchedulingHttpTrigger(JsonSerializerSettings serialiser)
        {
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
        }

        /// <summary>
        /// Invokes the HTTP trigger to set schedule.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest" /> instance.</param>
        /// <param name="starter"><see cref="IDurableOrchestrationClient" /> instance.</param>
        /// <param name="context"><see cref="ExecutionContext" /> instance.</param>
        /// <param name="log"><see cref="ILogger" /> instance.</param>
        /// <returns>Returns the <see cref="IActionResult" /> representing the scheduling status.</returns>
        [FunctionName(nameof(SchedulingHttpTrigger.SetScheduleAsync))]
        public async Task<IActionResult> SetScheduleAsync(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "orchestrators/schedules")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ExecutionContext context,
            ILogger log)
        {
            var requestId = (string)req.HttpContext.Items["MS_AzureFunctionsRequestID"];
            var invocationId = context.InvocationId;

            log.LogInformation("Scheduler was invoked.");
            log.LogInformation($"RequestID: {requestId}");
            log.LogInformation($"InvocationID: {invocationId}");

            var orchestratorFunctionName = nameof(SchedulingOrchestrationTrigger.SetScheduleOrchestrationAsync);
            var input = default(SchedulingRequest);
            using (var reader = new StreamReader(req.Body))
            {
                var payload = await reader.ReadToEndAsync();
                input = JsonConvert.DeserializeObject<SchedulingRequest>(payload, this._serialiser);

                // https://github.com/Azure/azure-functions-durable-extension/issues/1138#issuecomment-585868647
                // Workaround to prevent "cannot access a closed stream" issue
                req.Body = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            }

            var instanceId = await starter.StartNewAsync(orchestratorFunctionName: orchestratorFunctionName, instanceId: null, input: input);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        /// <summary>
        /// Invokes the durable event for event scheduling status check.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="starter"><see cref="IDurableOrchestrationClient"/> instance.</param>
        /// <param name="context"><see cref="ExecutionContext" /> instance.</param>
        /// <param name="log"><see cref="ILogger" /> instance.</param>
        /// <returns>Returns the <see cref="IActionResult"/> instance representing the list of scheduling statuses.</returns>
        [FunctionName(nameof(SchedulingHttpTrigger.GetSchedulesAsync))]
        public async Task<IActionResult> GetSchedulesAsync(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "orchestrators/schedules")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ExecutionContext context,
            ILogger log)
        {
            var requestId = (string)req.HttpContext.Items["MS_AzureFunctionsRequestID"];
            var invocationId = context.InvocationId;

            log.LogInformation($"Getting the list of schedules...");
            log.LogInformation($"RequestID: {requestId}");
            log.LogInformation($"InvocationID: {invocationId}");

            var status = (await starter.GetStatusAsync())
                         .OrderByDescending(p => p.Input.Value<DateTime>("schedule"));
            var result = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(status, this._serialiser),
                StatusCode = (int) HttpStatusCode.OK,
                ContentType = "application/json"
            };

            log.LogInformation($"Retrieved the list of schedules.");

            return result;
        }

        /// <summary>
        /// Invokes the durable event for event scheduling status check.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="starter"><see cref="IDurableOrchestrationClient"/> instance.</param>
        /// <param name="instanceId">Orchestrator instance ID.</param>
        /// <param name="context"><see cref="ExecutionContext" /> instance.</param>
        /// <param name="log"><see cref="ILogger" /> instance.</param>
        /// <returns>Returns the <see cref="IActionResult"/> instance representing the scheduling status of the given instance ID.</returns>
        [FunctionName(nameof(SchedulingHttpTrigger.GetScheduleAsync))]
        public async Task<IActionResult> GetScheduleAsync(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "orchestrators/schedules/{instanceId}")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            string instanceId,
            ExecutionContext context,
            ILogger log)
        {
            var requestId = (string)req.HttpContext.Items["MS_AzureFunctionsRequestID"];
            var invocationId = context.InvocationId;

            log.LogInformation($"Getting the schedule of {instanceId}...");
            log.LogInformation($"RequestID: {requestId}");
            log.LogInformation($"InvocationID: {invocationId}");

            var status = await starter.GetStatusAsync(instanceId);
            var result = new ContentResult()
            {
                Content = JsonConvert.SerializeObject(status, this._serialiser),
                StatusCode = (int) HttpStatusCode.OK,
                ContentType = "application/json"
            };

            log.LogInformation($"Retrieved the schedule of '{instanceId}'.");

            return result;
        }
    }
}
