# BlazorGoogleAuth
The project has been developed using C# and Blazor. Blazor is the same with MVC and I am also familiar with MVC and the same would be done using MVC project.
# How I developed
- Created a blazor web app. For MVC i would create an MVC web app.
- Added Identity scaffold item with SQLite in the project.
- Configured Google Console credentials and OAuth Consent screen.
- Added the setting in the appSettings.json
- Added the AddGoogle dependency in the program.cs and set the required google options that's clientId and ClientSecret and the entity framework inbuilt identity did the rest using external login providers.
- -- For this to work well, I made some changed in **OnLoginCallbackAsync** in *ExternalLogin.razor* which is under *Components -> account -> manage*. Below is the new code;
```
private async Task OnLoginCallbackAsync()
    {
        // Sign in the user with this external login provider if the user already has a login.
        var result = await SignInManager.ExternalLoginSignInAsync(
            externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey,
            isPersistent: true,
            bypassTwoFactor: true);


        var toekn = externalLoginInfo.AuthenticationTokens.First().Value;
        var user = await UserManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
        if (result.Succeeded)
        {
            Logger.LogInformation(
                "{Name} logged in with {LoginProvider} provider.",
                externalLoginInfo.Principal.Identity?.Name,
                externalLoginInfo.LoginProvider);

            if (user != null)
            {
                user.AccessToken = toekn;
                await UserManager.UpdateAsync(user);
                var authProperties = new AuthenticationProperties { IsPersistent = true };
                authProperties.StoreTokens(externalLoginInfo.AuthenticationTokens);
                await SignInManager.SignInAsync(user, authProperties);
            }
            RedirectManager.RedirectTo(ReturnUrl);
        }
        else if (result.IsLockedOut)
        {
            RedirectManager.RedirectTo("Account/Lockout");
        }

        // If the user does not have an account, then ask the user to create an account.
        if (externalLoginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            Input.Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
        }
    }
```
This adds _access_token_ to user *AuthenticationProperties* so that one can easily access them via _HttpContext_ like in the code below in the GoogleCalendarService
```

    private async Task<string> GetAccessTokenAsync()
    {
        // Assuming the user is authenticated and the token is stored in the authentication state
        var user = _httpContextAccessor.HttpContext.User;

        //check user is authenticated
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        return accessToken ?? "";
    }
  ```
  - Created a folder named Services. __For a big project I would create this as a separate project__. I added GoogleCalendarService here.
  - Create a component called *GoogleCalendar.razor*. In MVC the would be a cointroller and a View.
