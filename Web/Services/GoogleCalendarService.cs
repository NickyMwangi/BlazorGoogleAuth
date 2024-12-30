using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Web.Data;

namespace Web.Services;

public class GoogleCalendarService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<GoogleCalendarService> logger, UserManager<WebUser> UserManager)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<GoogleCalendarService> _logger = logger;
    private readonly UserManager<WebUser> _userManager = UserManager;

    public async Task<List<Google.Apis.Calendar.v3.Data.Event>> GetCalendarEventsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("No access token available for user.");
                return [];
            }
            var credential = GoogleCredential.FromAccessToken(accessToken);

            // Create a new Calendar API service using the access token
            var calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _configuration["Authentication:Google:ApplicationName"]
            });

            // Request calendar events
            var request = calendarService.Events.List("primary");
            request.TimeMinDateTimeOffset = DateTimeOffset.MinValue; // Start time
            request.MaxResults = 10; // Limit the number of events
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = await request.ExecuteAsync(cancellationToken);

            return events.Items.ToList();
        }
        catch(Exception exp)
        {
            _ = exp.Message;
            return [];
        }
    }

    private async Task<string> GetAccessTokenAsync()
    {
        // Assuming the user is authenticated and the token is stored in the authentication state
        var user = _httpContextAccessor.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        return accessToken ?? "";
    }
}
