using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorApp.Api
{
    public static class HelloYou
    {
        [FunctionName("HelloYou")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "secured/HelloYou")] HttpRequest req,
            ILogger log,
            ClaimsPrincipal principal)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (principal == null || !principal.Identity.IsAuthenticated || !principal.IsInRole("admin"))
            {
                log.LogWarning("Request was not authenticated");
                return new BadRequestObjectResult("Request was not authenticated.");
            }

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This SECURED HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This SECURED HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
