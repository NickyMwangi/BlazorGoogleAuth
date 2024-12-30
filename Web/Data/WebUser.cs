using Microsoft.AspNetCore.Identity;

namespace Web.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class WebUser : IdentityUser
    {
        public string? AccessToken { get; set; }
    }
}
