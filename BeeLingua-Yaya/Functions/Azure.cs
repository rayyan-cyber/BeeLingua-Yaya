using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeeLingua_Yaya.Functions
{
    public static class Azure
    {
        [FunctionName("GetAllLesson")]
        public static async Task<IActionResult> GetAllLesson(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            try
            {
                var query = new SqlQuerySpec("SELECT * FROM c");
                var pk = new PartitionKey("Lesson");
                var options = new FeedOptions() { PartitionKey = pk };
                var data = documentClient.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri("Course", "Lesson"), query, options);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }


        [FunctionName("GetLessonById")]
        public static IActionResult GetLessonById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Course/Lesson/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "Course",
                collectionName: "Lesson",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless",
                Id = "{id}",
                PartitionKey = "Lesson")] Class.Lesson lessonList,
            ILogger log)
        {
            try
            {
                ObjectResult result;
                string responMessage = default;
                responMessage = "Data Not Found!!!";
                result = new BadRequestObjectResult(responMessage);

                if (lessonList != null)
                {
                    result = new BadRequestObjectResult(lessonList);
                }
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }


        }

        [FunctionName("DeleteLessonById")]
        public static async Task<IActionResult> DeleteLessonById(
            [HttpTrigger(AuthorizationLevel.Function, "get", "delete", Route = "Course/DeleteLessonCode/{id}")] HttpRequestMessage req,
            [CosmosDB(
                databaseName: "Course",
                collectionName: "Lesson",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless",
                Id = "{id}",
                PartitionKey = "Lesson")] Document document,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            try
            {
                ObjectResult result;
                string responMessage = default;
                responMessage = "Data not deleted, because Data Not Found!!!";
                result = new BadRequestObjectResult(responMessage);

                if (document != null)
                {
                    await documentClient.DeleteDocumentAsync(document.SelfLink, new RequestOptions() { PartitionKey = new PartitionKey("Lesson") });
                    responMessage = "Data has been deleted";
                    result = new BadRequestObjectResult(responMessage);
                }
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

        }

        [FunctionName("UpdateLessonByLessonCode")]
        public static IActionResult UpdateLessonByLessonCode(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Course/UpdateLessonCode/{lessonCode}")] HttpRequestMessage req,
            [CosmosDB(
                databaseName: "Course",
                collectionName: "Lesson",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless",
                SqlQuery = "select * from Lesson r where r.lessonCode = {lessonCode}")] IEnumerable<Class.Lesson> lessonList,
            [CosmosDB(
                databaseName: "Course",
                collectionName: "Lesson",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] out Class.Lesson lessonData,
            ILogger log)
        {
            lessonData = null;
            try
            {
                ObjectResult result;
                string responMessage = default;
                responMessage = "Data not updated, because Data Not Found!!!";
                result = new BadRequestObjectResult(responMessage);
                if (lessonList.Any())
                {
                    lessonData = req.Content.ReadAsAsync<Class.Lesson>().Result;
                    responMessage = "Data has been updated";
                    result = new BadRequestObjectResult(responMessage);
                }
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("InsertLesson")]
        public static IActionResult InsertLesson(
            [HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post), Route = null)] HttpRequestMessage req,
            [CosmosDB(
                databaseName: "Course",
                collectionName: "Lesson",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] out Class.Lesson lessonData,
            TraceWriter log)
        {
            lessonData = null;
            try
            {
                ObjectResult result;
                string responMessage = default;

                lessonData = req.Content.ReadAsAsync<Class.Lesson>().Result;

                responMessage = "Please provide a value";
                result = new BadRequestObjectResult(responMessage);

                if (lessonData != null)
                {
                    if (!string.IsNullOrEmpty(lessonData.LessonCode))
                    {
                        responMessage = $"{lessonData.LessonCode} has been created";
                        result = new BadRequestObjectResult(responMessage);
                    }
                }
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

