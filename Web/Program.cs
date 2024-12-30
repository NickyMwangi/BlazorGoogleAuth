using Web.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Components.Account;
using Web.Data;
using Web.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityContext") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found."); ;

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddScoped<GoogleCalendarService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    }).AddGoogle(googleOpts =>
    {
        googleOpts.ClientId = builder.Configuration["Authentication:Google:client_id"];
        googleOpts.ClientSecret = builder.Configuration["Authentication:Google:client_secret"];
        googleOpts.Scope.Add("https://www.googleapis.com/auth/calendar.readonly");
        googleOpts.SaveTokens = true;
    })
    .AddIdentityCookies();

builder.Services.AddIdentityCore<WebUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<WebUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
// Enable authentication and authorization middleware
app.UseAuthentication();  // Authenticate requests
app.UseAntiforgery();
app.UseAuthorization();   // Authorize requests

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints(); ;

app.Run();
