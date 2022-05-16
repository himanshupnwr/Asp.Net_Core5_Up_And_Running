using Microsoft.AspNetCore.Identity;

namespace Rocky_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string UserFullName { get; set; }
    }
}
