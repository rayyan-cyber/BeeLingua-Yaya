using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeeLingua_Yaya.Functions
{
    public static class Nexus
    {
        //NEXUS
        [FunctionName("GetLessonAll")]
        public static async Task<IActionResult> GetLessonAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            try
            {
                using var rep = new Repository.Repositories.ClassRepository(documentClient);
                var data = await rep.GetAsync();
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetLessonByIDNexus")]
        public static async Task<IActionResult> GetLessonByIDNexus(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "LessonNexus/GetById/{id}/{pk}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string id,
            string pk,
            ILogger log)
        {
            try
            {
                using var rep = new Repository.Repositories.ClassRepository(documentClient);
                var data = await rep.GetAsync(predicate: p => p.Id == id, partitionKeys: new Dictionary<string, string> { { "LessonCode", pk } });
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        
        [FunctionName("CreateLessonNexus")]
        public static async Task<IActionResult> CreateLessonNexus(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LessonNexus/Create")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            try
            {
                string reqBody = await new StreamReader(req.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<Models.Lesson>(reqBody);

                var dataToBeUpdated = new Models.Lesson
                {
                    LessonCode = input.LessonCode,
                    Description = input.Description
                };

                using var rep = new Repository.Repositories.ClassRepository(documentClient);
                var data = await rep.CreateAsync(dataToBeUpdated);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("UpdateLessonNexus")]
        public static async Task<IActionResult> UpdateLessonNexus(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "LessonNexus/Update/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string id,
            ILogger log)
        {
            try
            {
                string reqBody = await new StreamReader(req.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<Models.Lesson>(reqBody);

                var dataToBeUpdated = new Models.Lesson
                {
                    Id = id,
                    LessonCode = input.LessonCode,
                    Description = input.Description
                };

                using var rep = new Repository.Repositories.ClassRepository(documentClient);
                var data = await rep.UpdateAsync(id, dataToBeUpdated);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("DeleteLessonNexus")]
        public static async Task<IActionResult> DeleteLessonNexus(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "LessonNexus/Delete/{id}/{pk}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string id,
            string pk,
            ILogger log)
        {
            try
            {
                using var rep = new Repository.Repositories.ClassRepository(documentClient);
                await rep.DeleteAsync(id, partitionKeys: new Dictionary<string, string> { { "LessonCode", pk } });
                return new OkObjectResult("Deleted");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

        }
    }
}

