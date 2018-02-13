using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using FacebookClone.Models.Data;

namespace FacebookClone
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Hello(string message)
        {

            Trace.WriteLine(message);
            //Clients.All.hello();

            // set clients
            var clients = Clients.All;

            // call js function
            clients.test("this is a test");
        }


        public void Notify(string friend)
        {
            // init db
            Db db = new Db();

            // get friend id
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDto.Id;

            // get frined request count
            var frCount = db.Friends.Count(x => x.User2 == friendId && x.Active == false);

            // set client
            var clients = Clients.Others;

            // call js function
            clients.NotifyFriend(friend, frCount);
        
        }

    }
}