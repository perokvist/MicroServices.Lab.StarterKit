using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab.Worker.Messages
{
    //{"body":{"createdBy":"Johan","entryPoint":"http://someurl1.com","name":"service1"},
    //"streamId":"bfa15a72-13c0-4908-ac90-d1ad8ebc281b","createdAt":1389971146921,
    //"type":"ServiceOnlineEvent","meta":{}}"
    
    public class ServiceOnlineEvent
    {
        public static ServiceOnlineEvent Create(string createdBy, string entryPoint, string name, Guid streamId,
            DateTime createdAt)
        {
            var e = new ServiceOnlineEvent();
            e.body.createdBy = createdBy;
            e.body.entryPoint = entryPoint;
            e.body.name = name;
            e.body.streamId = streamId;
            e.body.createdAt = createdAt;
            e.body.type = "ServiceOnlineEvent";
            return e;
        }

        public dynamic body { get; set; }
        public dynamic meta { get; set; }

    }
}
