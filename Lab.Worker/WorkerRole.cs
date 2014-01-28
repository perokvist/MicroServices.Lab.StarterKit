using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Lab.Worker.Infrastructure;
using Lab.Worker.Messages;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using RabbitMQ.Client;

namespace Lab.Worker
{
    public class WorkerRole : RoleEntryPoint
    {

        public override void Run()
        {
            Trace.TraceInformation("Lab.Worker entry point called", "Information");
            
            base.Run();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
