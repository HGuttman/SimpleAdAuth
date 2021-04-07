using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAdsAuth.Data;
using SimpleAdsAuth.Web.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace SimpleAdsAuth.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString =
            "Data Source=.\\sqlexpress;Initial Catalog=SimpleAdsAuth;Integrated Security=true;";

        public IActionResult CreateLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateLogin(User user, string password)
        {
            DB db = new DB(_connectionString);
            db.AddUser(user, password);
            return Redirect("/account/login");
        }
        public IActionResult LogIn()
        {
            if(TempData["errorMessage"] != null)
            {
                ViewBag.Message = TempData["Error"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            DB db = new DB(_connectionString);
            var user = db.Login(email, password);
            if(user == null)
            {
                TempData["Error"] = "Invalid Email or Password";
                return Redirect("/account/login");
            }
            var claims = new List<Claim>
            {
                new Claim ("user", email)
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/newad");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }

    }
}
