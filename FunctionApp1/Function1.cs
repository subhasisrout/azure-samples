using System;
using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            var client = new HttpClient();
            var token = new DefaultAzureCredential().GetToken(new TokenRequestContext(new[] { "api://c7c788a5-f95b-41d1-b677-ca2116fcf073" }));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var response = await client.GetAsync("https://securewebappdemo-exfhbqbtcvbugjaa.canadacentral-01.azurewebsites.net/weatherforecast");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API response: {content}");
            }
            else
            {
                _logger.LogError($"API call failed with status code: {response.StatusCode}");
            }
        }
    }
}
