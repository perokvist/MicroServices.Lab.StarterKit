using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

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

        public static Task RegisterServiceAsync()
        {
            throw new NotImplementedException();
        }
    }
}
