using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Lab.Worker.Infrastructure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Lab.Worker
{
    public class WorkerRole : RoleEntryPoint
    {

        public override void Run()
        {
            Trace.TraceInformation("Lab.Worker entry point called", "Information");
            
            //Demo code

            //Connection
            var rabbitConnectionFactory = new ConnectionFactory
            {
                Uri = "will be given to you"
            };
            var connection = rabbitConnectionFactory.CreateConnection();
            
            //Publish
            var publisher = MQ.CreatePublisherOn(connection, "lab", "service");
            publisher(Messages.ServiceOnlineEvent("serviceName", "Test Service", "you", "jayway.com", "github.com"));
            
            //Recive
            MQ.StartReceivingOn(connection, "lab", "service", "sub1", (meta, json) => Task.Run(() => Trace.TraceInformation("Handling message: {0}", json)));

            base.Run(); //Do not remove, when run retuns woker is recycled.
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
