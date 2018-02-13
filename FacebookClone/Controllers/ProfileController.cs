using FacebookClone.Models.Data;
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
    }
}