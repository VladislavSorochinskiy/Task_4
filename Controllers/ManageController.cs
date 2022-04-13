using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task_4.Data;

namespace Task_4.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public ManageController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string[] selectedUsers)
        {
            foreach (var id in selectedUsers)
            {
                User user = await userManager.FindByIdAsync(id);
                await SignOutUser(user);
                await userManager.DeleteAsync(user);
            }
         
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Block(string[] selectedUsers)
        {
            foreach (var id in selectedUsers)
            {
                User user = await userManager.FindByIdAsync(id);
                await SetBlockedStatus(user, true);
                await SignOutUser(user);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(string[] selectedUsers)
        {
            foreach (var id in selectedUsers)
            {
                User user = await userManager.FindByIdAsync(id);
                await SetBlockedStatus(user, false); 
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task SetBlockedStatus(User user, bool isBlocked)
        {
            user.IsBlocked = isBlocked;
            await userManager.UpdateAsync(user);
        }

        private async Task SignOutUser(User user)
        {
            User currentUser = await userManager.GetUserAsync(HttpContext.User);

            if (user.Id.Equals(currentUser.Id))
            {
                await signInManager.SignOutAsync();
            }
            else
            {
                await userManager.UpdateSecurityStampAsync(user);
            }
        }
    }
}
