using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Chat;

namespace Company.Function
{
    public static class Devdayapi_cs
    {
        [FunctionName("Devdayapi_cs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string promptText = data?.text ?? string.Empty;

            AzureOpenAIClient client = new(
                new Uri(Environment.GetEnvironmentVariable("OPENAI_ENDPOINT")),
                new DefaultAzureCredential()
            );
            ChatClient chatClient = client.GetChatClient(Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME"));
            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage(Prompts.SystemMessage),
                    new UserChatMessage(promptText)

                ]);

            return new OkObjectResult(completion.Content[0].Text);
        }
    }
}
