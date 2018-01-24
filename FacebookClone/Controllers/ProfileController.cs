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
    }
}