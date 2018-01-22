using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace FacebookClone
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}