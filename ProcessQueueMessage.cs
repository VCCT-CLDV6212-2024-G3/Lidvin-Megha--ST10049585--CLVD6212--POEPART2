using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Queues;

namespace CLVD6212_POEPART2
{
    public static class ProcessQueueMessage
    {
        [FunctionName("ProcessQueueMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string queueName = req.Query["queueName"];
            string message = req.Query["message"];

            if (string.IsNullOrEmpty(queueName) || string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult("Queue name and message must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");
            var queueServiceClient = new QueueServiceClient(connectionString);
            var queueClient = queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);

            return new OkObjectResult("Message added to queue");
        }
    }
}
