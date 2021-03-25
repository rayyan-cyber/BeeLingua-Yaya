using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeeLingua_Yaya.Functions
{
    public static class EventManual
    {

        [FunctionName("CreateDataLesson")]
        public static async Task<IActionResult> CreateDataLesson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "Course",
                collectionName: "Lesson",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] IAsyncCollector<Class.Lesson> lessonData,
            [EventGrid(TopicEndpointUri = "eventGridEndPoint", TopicKeySetting ="eventGridEndKey")] IAsyncCollector<EventGridEvent> events,
            ILogger log)
        {
            try
            {
                string reqBody = await new StreamReader(req.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<Class.Lesson>(reqBody);

                var dataToBeInserted = new Class.Lesson
                {
                    LessonCode = input.LessonCode,
                    Description = input.Description,
                    PartitionKey = input.PartitionKey
                };


                var eventTobeInserted = new NotificationLesson
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = "Create/",
                    Data = dataToBeInserted,
                    EventType = "BeeLingua_Yaya",
                    EventTime = DateTime.UtcNow
                };

                var eventData = new List<NotificationLesson>();
                eventData.Add(eventTobeInserted);

                var dataToEventGrid = new EventGridEvent(eventTobeInserted.Id, eventTobeInserted.Subject, eventData, eventTobeInserted.EventType, eventTobeInserted.EventTime, "1.0");

                await lessonData.AddAsync(dataToBeInserted);
                await events.AddAsync(dataToEventGrid);

                log.LogInformation($"C# Http function processed a dataToEventGrid: {eventData}");
                return new OkObjectResult(dataToEventGrid);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

