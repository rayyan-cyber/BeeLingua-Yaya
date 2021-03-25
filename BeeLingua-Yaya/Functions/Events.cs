using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeeLingua_Yaya.Functions
{
    public static class Events
    {
        [FunctionName("Events")]
        public static async Task Run(
            [EventHubTrigger("lesson.notification", Connection = "evhBLTutorialConnection")] EventData[] events,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)

        {
            var exceptions = new List<Exception>();
            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var input = JsonConvert.DeserializeObject<NotificationLesson[]>(messageBody);

                    var eventTobeInserted = new NotificationLesson
                    {
                        Id = Guid.NewGuid().ToString(),
                        MessageBodyEvent = messageBody,
                        Subject = input[0].Subject,
                        Data = input[0].Data,
                        EventType = input[0].EventType,
                        EventTime = input[0].EventTime,
                        Topic = input[0].Topic
                    };
                    using var rep = new Repository.Repositories.NotificationLessonRepository(documentClient);
                    var data = await rep.CreateAsync(eventTobeInserted);

                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }
                catch (Exception e)
                {
                   exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
