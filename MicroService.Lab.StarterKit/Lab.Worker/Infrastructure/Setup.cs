using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Lab.Worker.Messages;

namespace Lab.Worker.Infrastructure
{
    public static class Setup
    {
        public static IBus CreateBus()
        {
            var bus = RabbitHutch.CreateBus("connectionGoesHere");
            bus.Connected += () => Trace.TraceInformation("Lab.Worker bus connected", "Information");
            bus.Disconnected += () => Trace.TraceInformation("Lab.Worker bus disconnected", "Information");
            return bus;
        }

        public static async Task RegisterServiceAsync()
        {
            //{"body":{"createdBy":"Johan","entryPoint":"http://someurl1.com","name":"service1"},"streamId":"bfa15a72-13c0-4908-ac90-d1ad8ebc281b","createdAt":1389971146921,"type":"ServiceOnlineEvent","meta":{}}"
            //http://www.asp.net/web-api/overview/web-api-clients/calling-a-web-api-from-a-net-client

            using (var client = new HttpClient())
            {
                var e = ServiceOnlineEvent.Create("you", "url", "servicename", new Guid(), DateTime.UtcNow);
                await client.PostAsJsonAsync("serviceUri", e);
            }
            
        }
    }
}
