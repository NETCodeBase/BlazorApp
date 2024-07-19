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
        [FunctionName("EmployeesHub")]
        public static Task Run(
                [SqlTrigger("[dbo].[Employee]", "DbConnection")] IReadOnlyList<SqlChange<ToDoItem>> changes,
                [SignalR(HubName = "EmployeesHub")] IAsyncCollector<SignalRMessage> signalrMessageForEmployees,
                ILogger log)
        {
            log.LogInformation("SQL Changes: " + JsonConvert.SerializeObject(changes));

            return signalrMessageForEmployees.AddAsync(new SignalRMessage
            { Target = "employeeRefresh", Arguments = new[] { changes } });

        }
    }

    public class ToDoItem
    {
        public string Id { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
    }
}
