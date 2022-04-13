using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Task_4.Data;
using Task_4.Models;

namespace Task_4.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> userManager;

        public HomeController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return View(userManager.Users.ToList());
        }

        [HttpPost]
        [Authorize]
        public IActionResult Index(string[] selectedUsers)
        {
            if (selectedUsers.Length == 0) Console.WriteLine(".............");
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}