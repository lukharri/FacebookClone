using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using FacebookClone.Models.Data;
using System.Web.Mvc;

namespace FacebookClone
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        //public void Hello(string message)
        //{

        //    Trace.WriteLine(message);
        //    //Clients.All.hello();

        //    // set clients
        //    var clients = Clients.All;

        //    // call js function
        //    clients.test("this is a test");
        //}


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


        // Get number of pending friend requests
        public void GetFriendRequestCount()
        {
            Db db = new Db();

            UserDTO userDto = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            var friendRequests = db.Friends.Count(x => x.User2 == userId && x.Active == false);

            var clients = Clients.Caller;

            clients.updateFriendRequests(Context.User.Identity.Name, friendRequests);
        }


        public void GetFriendsCount(int friendId)
        {
            Db db = new Db();

            // get logged in user's ID
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            // get logged in user's friend count
            var friendCount1 = db.Friends
            .Count(x => x.User2 == userId && x.Active == true || x.User1 == userId && x.Active == true);

            // get friend's username
            UserDTO userDto2 = db.Users.Where(x => x.Id.Equals(friendId)).FirstOrDefault();
            var username = userDto2.Username;

            // get friend's friend count
            var friendCount2 = db.Friends
            .Count(x => x.User2 == friendId && x.Active == true || x.User1 == friendId && x.Active == true);

            // set clients and call JS function
            var clients = Clients.All;
            clients.updateFriendCount(Context.User.Identity.Name, username, friendCount1, friendCount2);
        }


        public void NotifyOfMessage(string friend)
        {
            Db db = new Db();

            // get friend
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDto.Id;

            // get message count
            var messageCount = db.Messages.Count(x => x.To == friendId && x.Read == false);

            // set clients
            var clients = Clients.Others;

            // call js function
            clients.msgcount(friend, messageCount);
        }
    }
}