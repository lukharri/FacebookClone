﻿using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModels.Account;
using FacebookClone.Models.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FacebookClone.Controllers
{
    public class AccountController : Controller
    {
        // GET: /
        public ActionResult Index()
        {
            // confirm user is not logged in
            string username = User.Identity.Name;

            if (!string.IsNullOrEmpty(username))
                return Redirect("~/" + username);
            
            return View();
        }


        // POST: Account/CreateAccount
        [HttpPost]
        public ActionResult CreateAccount(UserViewModel model, HttpPostedFileBase imageFile)
        {
            // init db
            Db db = new Db();

            // check model state
            if(!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // make sure username is unique
            if(db.Users.Any(x => x.Username.Equals(model.Username)))
            {
                ModelState.AddModelError("", "Username" + model.Username + " is taken.");
                model.Username = "";
                return View("Index", model);
            }

            // create user dto
            UserDTO userDTO = new UserDTO()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                Username = model.Username,
                Password = model.Password
            };

            // add to DTO
            db.Users.Add(userDTO);

            // save 
            db.SaveChanges();

            // get inserted id
            int userId = userDTO.Id;

            // login user 
            FormsAuthentication.SetAuthCookie(model.Username, false);

            // set uploads directory
            var uploadsDir = new DirectoryInfo(string.Format("{0}Uploads", Server.MapPath(@"\")));

            // check if file was uploaded
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                // get extension
                string ext = imageFile.ContentType.ToLower();

                // verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                    return View("Index", model);

                }

                // set image name - the user's id w/the jpg extension
                string imageName = userId + "jpg";

                // set image path
                var path = string.Format("{0}\\{1}", uploadsDir, imageName);

                // save image
                imageFile.SaveAs(path);
            }

            // add to wall
            WallDto wall = new WallDto();

            wall.Id = userId;
            wall.Message = "";
            wall.DateEdited = DateTime.Now;

            db.Wall.Add(wall);
            db.SaveChanges();

            // redirect
            return Redirect("~/" + model.Username);
        }


        // GET: /{username}
        [Authorize]
        public ActionResult Username(string username = "")
        {
            // init db
            Db db = new Db();

            // check if user exists
            if(!db.Users.Any(x => x.Username.Equals(username)))
            {
                return Redirect("Index");
            }

            // viewbag username
            ViewBag.Username = username;

            // get logged in user's username
            string user = User.Identity.Name;

            // viewbag user's full name
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
            ViewBag.FullName = userDTO.FirstName + " " + userDTO.LastName;

            // get user's id
            int userId = userDTO.Id;

            // ViewBag user's id
            ViewBag.UserId = userId;

            // get viewing full name
            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            ViewBag.ViewingFullName = userDTO2.FirstName + " " + userDTO2.LastName;

            // get username's image
            ViewBag.UsernameImage = userDTO2.Username + ".jpg";

            // viewbag user type
            string userType = "guest";

            if (username.Equals(user))
                userType = "owner";

            ViewBag.UserType = userType;

            // check friend status
            if(userType == "guest")
            {
                UserDTO user1 = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
                int id1 = user1.Id;

                UserDTO user2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
                int id2 = user2.Id;

                FriendDto friend1 = db.Friends.Where(x => x.User1 == id1 && x.User2 == id2).FirstOrDefault();
                FriendDto friend2 = db.Friends.Where(x => x.User2 == id1 && x.User1 == id2).FirstOrDefault();

                if(friend1 == null && friend2 == null)
                {
                    ViewBag.NotFriends = "True";
                }

                if (friend1 != null)
                {
                    if (!friend1.Active)
                    {
                        ViewBag.NotFriends = "Pending";
                    }
                }

                if (friend2 != null)
                {
                    if (!friend2.Active)
                    {
                        ViewBag.NotFriends = "Pending";
                    }
                }

            }

            // get friend request count
            var friendRequests = db.Friends.Count(x => x.User2 == userId && x.Active == false);

            if(friendRequests > 0)
            {
                ViewBag.FriendRequest = friendRequests;
            }


            /*** GET FRIENDS COUNT ***/
            // get Id of user whose page is being viewed
            UserDTO userDto = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            int usernameId = userDto.Id;

            var friendCount = db.Friends
                .Count(x => x.User2 == usernameId && x.Active == true || x.User1 == usernameId && x.Active == true);

            ViewBag.FriendCount = friendCount;

            // Get message count
            var messageCount = db.Messages.Count(x => x.To == userId && x.Read == false);
            ViewBag.MessageCount = messageCount;

            // viewBag user wall
            WallDto wall = new WallDto();
            ViewBag.WallMessage = db.Wall.Where(x => x.Id == userId).Select(x => x.Message).FirstOrDefault();

            // ViewBag friend walls
            List<int> friendsIds1 = db.Friends.Where(x => x.User1 == userId && x.Active == true)
                .ToArray().Select(x => x.User2).ToList();

            List<int> friendsIds2 = db.Friends.Where(x => x.User2 == userId && x.Active == true)
                .ToArray().Select(x => x.User1).ToList();

            List<int> allFriendIds = friendsIds1.Concat(friendsIds2).ToList();

            List<WallViewModel> walls = db.Wall.Where(x => allFriendIds.Contains(x.Id))
                .ToArray().OrderByDescending(x => x.DateEdited).Select(x => new WallViewModel(x)).ToList();

            ViewBag.Walls = walls;

            return View();
        }


        // GET: Account/logout
        [Authorize]
        public ActionResult Logout()
        {
            // sign out
            FormsAuthentication.SignOut();

            // Redirect
            return View("Index");
        }


        public ActionResult LoginPartial()
        {
            return PartialView("_LoginPartial");
        }


        // POST: account/login
        [HttpPost]
        public string Login(string username, string password)
        {
            // init db
            Db db = new Db();

            // check if user exists
            if(db.Users.Any(x => x.Username.Equals(username) && x.Password.Equals(password)))
            {
                // log in
                FormsAuthentication.SetAuthCookie(username, false);
                return "ok";
            }
            else
            {
                return "problem";
            }
        }
    }
}