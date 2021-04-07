using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAdsAuth.Data;
using SimpleAdsAuth.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
            "Data Source=.\\sqlexpress;Initial Catalog=SimpleAdsAuth;Integrated Security=true;";

        public IActionResult Index()
        {
           DB db = new DB(_connectionString);
            IEnumerable<Ad> ads = db.GetAds();
            var currentUserId = GetCurrentUserId();
            var vm = new IndexViewModel
            {
                ads = ads.Select(ad => new AdViewModel
                {
                    ad = ad,
                    canDelete = currentUserId != null && ad.UserId == currentUserId
                }).ToList()
            };
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var userId = GetCurrentUserId();
            ad.UserId = userId.Value;
            DB db = new DB(_connectionString);
            db.AddAd(ad);
            return Redirect("/");
        }
        [Authorize]
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            DB db = new DB(_connectionString);
            var userIdForAd = db.GetUserIdForAd(id);
            var currentUserId = GetCurrentUserId().Value;
            if(currentUserId == userIdForAd)
            {
                db.DeleteAd(id);
            }
            return Redirect("/");
        }
        public IActionResult MyAccount()
        {
            DB db = new DB(_connectionString);
            var userId = GetCurrentUserId().Value;
            return View(db.GetAdsForUser(userId));
        }
        private int? GetCurrentUserId()
        {
            var userDb = new DB(_connectionString);
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }
            var user = userDb.GetByEmail(User.Identity.Name);
            if (user == null)
            {
                return null;
            }

            return user.Id;
        }
    }
}
