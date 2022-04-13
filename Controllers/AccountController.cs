using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task_4.Data;
using Task_4.Models.AccountModels;

namespace Task_4.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Name, RegisterDate = DateTime.Now.ToString(), LastLogin = DateTime.Now.ToString(), Status = "Online" };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByNameAsync(model.Name);
                bool isBlocked = IsBlocked(user);

                var result = await signInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, false);
                if (result.Succeeded && !isBlocked)
                {
                    user.Status = "Online";
                    user.LastLogin = DateTime.Now.ToString();
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login or password!");
                }
            }
            return View(model);
        }

        private bool IsBlocked(User user)
        {
            if (user == null)
            {
                ModelState.AddModelError("", "User doesn't exsist!");
                return true;
            }

            if (user.IsBlocked)
            {
                ModelState.AddModelError("", "You are blocked!");
                return true;
            }       
            else return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
/*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            User user = await userManager.GetUserAsync(HttpContext.User);

            if(user != null) await SetUserStatusAsync("Offline");

            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }*/

        private async Task SetUserStatusAsync(string status)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            user.Status = status;
            await userManager.UpdateAsync(user);
        }
    }
}