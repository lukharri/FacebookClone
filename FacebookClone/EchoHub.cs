using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using FacebookClone.Models.Data;
using System.Web.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;

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


        public void NotifyOfMessageOwner()
        {
            Db db = new Db();

            // get logged in user's id
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            // get message count
            var messageCount = db.Messages.Count(x => x.To == userId && x.Read == false);

            // set clients
            var clients = Clients.Caller;

            // call js function
            clients.msgcount(Context.User.Identity.Name, messageCount);

        }


        public  override Task OnConnected()
        {
            // test log - log user connection
            // Trace.WriteLine("here i am..." + Context.ConnectionId);

            Db db = new Db();

            // get logged in user's id
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            // get connection id
            string connId = Context.ConnectionId;

            // add online dto
            if(!db.OnlineUser.Any(x => x.Id == userId))
            {
                OnlineUserDto onlineUser = new OnlineUserDto();

                onlineUser.Id = userId;
                onlineUser.ConnectionId = connId;

                db.OnlineUser.Add(onlineUser);
                db.SaveChanges();

            }


            // get all onnline ids
            List<int> onlineIds = db.OnlineUser.ToArray().Select(x => x.Id).ToList();

            // get friend ids
            List<int> friendsIds1 = db.Friends.Where(x => x.User1 == userId && x.Active == true)
                .ToArray().Select(x => x.User2).ToList();

            List<int> friendsIds2 = db.Friends.Where(x => x.User2 == userId && x.Active == true)
                .ToArray().Select(x => x.User1).ToList();

            List<int> allFriendIds = friendsIds1.Concat(friendsIds2).ToList();

            // get final set of ids
            List<int> resultList = onlineIds.Where((i) => allFriendIds.Contains(i)).ToList();

            // create a dictionary of friend ids and usernames
            Dictionary<int, string> friends = new Dictionary<int, string>();

            foreach(var id in resultList)
            {
                var users = db.Users.Find(id);
                string friend = users.Username;

                if (!friends.ContainsKey(id))
                {
                    friends.Add(id, friend); 
                }
            }

            var transformed = from key in friends.Keys
                              select new { id = key, friend = friends[key] };
            string json = JsonConvert.SerializeObject(transformed);

            // set clients
            var clients = Clients.Caller;

            // call js function
            clients.getonlinefriends(Context.User.Identity.Name, json);

            UpdateChat();

            // return
            return base.OnConnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            // test log - log user logging out of account
            Trace.WriteLine("disconnected..."   + Context.ConnectionId);


            // init db
            Db db = new Db();

            // get logged in user's id
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;
            
            // remove from db
            if(db.OnlineUser.Any(x => x.Id.Equals(userId)))
            {
                OnlineUserDto onlineUser = db.OnlineUser.Find(userId);
                db.OnlineUser.Remove(onlineUser);
                db.SaveChanges();
            }

            // update chat - display newly logged in friends
            UpdateChat();

            // Return
            return base.OnDisconnected(stopCalled);
        }


        public void UpdateChat()
        {
            // init db
            Db db = new Db();

            // get all online ids
            List<int> onlineIds = db.OnlineUser.ToArray().Select(x => x.Id).ToList();

            // loop through online ids and get friends
            foreach (var userId in onlineIds)
            {
                // get username
                UserDTO user = db.Users.Find(userId);
                var username = user.Username;

                // get all friend ids
                List<int> friendsIds1 = db.Friends.Where(x => x.User1 == userId && x.Active == true)
                    .ToArray().Select(x => x.User2).ToList();

                List<int> friendsIds2 = db.Friends.Where(x => x.User2 == userId && x.Active == true)
                    .ToArray().Select(x => x.User1).ToList();

                List<int> allFriendIds = friendsIds1.Concat(friendsIds2).ToList();

                // get final set of ids
                List<int> resultList = onlineIds.Where((i) => allFriendIds.Contains(i)).ToList();

                // create a dictionary of friend ids anad user names
                Dictionary<int, string> friends = new Dictionary<int, string>();

                foreach (var id in resultList)
                {
                    var users = db.Users.Find(id);
                    string friend = users.Username;

                    if (!friends.ContainsKey(id))
                    {
                        friends.Add(id, friend);
                    }
                }

                var transformed = from key in friends.Keys
                                  select new { id = key, friend = friends[key] };
                string json = JsonConvert.SerializeObject(transformed);

                // set clients
                var clients = Clients.All;

                // call js function
                clients.updatechat(username, json);
            }
        }
    }
}