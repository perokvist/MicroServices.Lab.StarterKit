using System.Diagnostics;
using System.Net;
using Microsoft.WindowsAzure.ServiceRuntime;

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
