using Web.Data;
using Microsoft.AspNetCore.Identity;

namespace Web.Components.Account
{
    internal sealed class IdentityUserAccessor(UserManager<WebUser> userManager, IdentityRedirectManager redirectManager)
    {
        public async Task<WebUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }
    }
}
