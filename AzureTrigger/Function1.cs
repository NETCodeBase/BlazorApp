using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Channels;
using System.Linq;


namespace AzureTrigger
{
    public static class SqlTriggerBinding
    {
        // This will manage connections to SignalR
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequest req,
            [SignalRConnectionInfo(HubName = "EmployeesHub")]
            SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        //Azure SQL Trigger function
        //[FunctionName("EmployeesHub")]
        //public static Task Run(
        //        [SqlTrigger("[dbo].[Employee]", "DbConnection")] IReadOnlyList<SqlChange<ToDoItem>> changes,
        //        [SignalR(HubName = "EmployeesHub")] IAsyncCollector<SignalRMessage> signalrMessageForEmployees,
        //        ILogger log)
        //{
        //    log.LogInformation("SQL Changes: " + JsonConvert.SerializeObject(changes));

        //    return signalrMessageForEmployees.AddAsync(new SignalRMessage
        //    { Target = "employeeRefresh", Arguments = new[] { "Hi" } });

        //}


        [FunctionName("EmployeesHub")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SignalR(HubName = "EmployeesHub")] IAsyncCollector<SignalRMessage> signalrMessageForEmployees,
            ILogger log)
        {
            log.LogInformation("Azure function Triggered");

            string name = req.Query["name"];

            string title = req.Query["title"];

            string lanIds = req.Query["lanIds"];
            string[] lanIdsArray = lanIds.Split("|");

            foreach (string lanId in lanIdsArray)
            {
                await signalrMessageForEmployees.AddAsync(new SignalRMessage
                {
                    Target = lanId,
                    Arguments = new[] { name + "|" + title }
                });
            }

            if (lanIdsArray.Count() == 0)
            {
                await signalrMessageForEmployees.AddAsync(new SignalRMessage
                {
                    Target = "all",
                    Arguments = new[] { name + "|" + title }
                });
            }

            (string firstName, string lastName) = ("Sudheer", "Bets");
            return new OkObjectResult(new { firstName, lastName });
        }

        public class ToDoItem
        {
            public string Id { get; set; }
            public int Priority { get; set; }
            public string Description { get; set; }
        }
    }
}
