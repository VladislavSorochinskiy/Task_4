using Microsoft.AspNetCore.Identity;

namespace Task_4.Data
{
    public class User : IdentityUser
    {
        public string RegisterDate { get; set; }
        public string? LastLogin { get; set; }
        public string Status { get; set; }
        public bool IsBlocked { get; set; }

        public User()
        { }
    }
}
