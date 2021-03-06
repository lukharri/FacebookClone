﻿using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacebookClone.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }

        // POST: Profile/LiveSearch
        [HttpPost]
        public JsonResult LiveSearch(string searchVal)
        {
            // init db
            Db db = new Db();

            // create list of users based on search value
            // users Username should contain the searched value and not include themselves
            List<LiveSearchUserViewModel> usernames = db.Users.Where(x => x.Username.Contains(searchVal)
            && x.Username !=  User.Identity.Name).ToArray().Select(x => new LiveSearchUserViewModel(x)).ToList();

            // return JSON
            return Json(usernames);
        }


        // POST: Profile/AddFriend
        [HttpPost]
        public void AddFriend(string friend)
        {
            // init db
            Db db = new Db();

            // get logged in user's id
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            // get friend to be id
            UserDTO userDto2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDto2.Id;

            // add dto
            FriendDto friendDto = new FriendDto();

            friendDto.User1 = userId;
            friendDto.User2 = friendId;
            friendDto.Active = false;

            db.Friends.Add(friendDto);
            db.SaveChanges();
        }


        // POST: /profile/DisplayFriendRequests
        [HttpPost]
        public JsonResult DisplayFriendRequests()
        {
            // init db
            Db db = new Db();

            // get user id
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            // create list of friend requests
            List<FriendRequestViewModel> friendRequests = db.Friends
                .Where(x => x.User2 == userId && x.Active == false)
                .ToArray()
                .Select(x => new FriendRequestViewModel(x))
                .ToList();

            // init list of users
            List<UserDTO> users = new List<UserDTO>();

            foreach(var request in friendRequests)
            {
                var user = db.Users.Where(x => x.Id == request.User1).FirstOrDefault();
                users.Add(user);
            }

            // return json
            return Json(users);
        }


        // POST: /profile/AcceptFriendRequest
        [HttpPost]
        public void AcceptFriendRequest(int friendId)
        {
            Db db = new Db();

            UserDTO userDto = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            FriendDto friends = db.Friends.Where(x => x.User1 == friendId && x.User2 == userId).FirstOrDefault();
            friends.Active = true;
            db.SaveChanges();
        }


        // POST: /profile/DeclineFriendRequest
        [HttpPost]
        public void DeclineFriendRequest(int friendId)
        {
            Db db = new Db();

            UserDTO userDto = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            FriendDto friends = db.Friends.Where(x => x.User1 == friendId && x.User2 == userId).FirstOrDefault();
            db.Friends.Remove(friends);
            db.SaveChanges();
        }


        // POST: /profile/SendMessage
        [HttpPost]
        public void SendMessage(string friend, string message)
        {
            Db db = new Db();

            // get user id
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            // get friend id
            UserDTO userDto2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDto2.Id;

            MessageDto messageDto = new MessageDto();
            messageDto.From = userId;
            messageDto.To = friendId;
            messageDto.Message = message;
            messageDto.DateSent = DateTime.Now;
            messageDto.Read = false;

            db.Messages.Add(messageDto);
            db.SaveChanges();

        }


        // POST: /profile/DisplayUnreadMessages
        [HttpPost]
        public JsonResult DisplayUnreadMessages()
        {
            Db db = new Db();

            // get user id
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDto.Id;

            // create list of unread messages
            List<MessageViewModel> messages = db.Messages.Where(x => x.To == userId && x.Read == false)
                .ToArray().Select(x => new MessageViewModel(x)).ToList();

            // make unread read
            db.Messages.Where(x => x.To == userId && x.Read == false).ToList().ForEach(x => x.Read = true);
            db.SaveChanges();

            // return json
            return Json(messages);
        }


        // POST: /profile/UpdateWallMessage
        [HttpPost]
        public void UpdateWallMessage(int id, string message)
        {
            // init db
            Db db = new Db();

            // update wall
            WallDto wall = db.Wall.Find(id);

            wall.Message = message;
            wall.DateEdited = DateTime.Now;

            db.SaveChanges();

        }
    }
}