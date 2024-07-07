using DevicesApp.Core.IConfiguration;
using DevicesApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DevicesApp.Middlewares;

public class ActionLoggingMiddleware(RequestDelegate next, ILogger<ActionLoggingMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ActionLoggingMiddleware> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            throw;
        }

        if (context.User.Identity?.IsAuthenticated == true)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var userIdentifier = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

                var user = await userManager.FindByNameAsync(userIdentifier);

                if (user is not null)
                {
                    var userId = user.UserId;
                    var userName = user.UserName ?? "Unknown";
                    var userRole = context.User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";
                    var action = $"{context.Request.Method} {context.Request.Path}";
                    var entity = context.Request.Path.Value?.Split('/').Skip(2).FirstOrDefault() ?? "Unknown";
                    var statusCode = context.Response.StatusCode;

                    var actionLog = new ActionLog
                    {
                        UserId = userId.ToString(),
                        UserName = userName,
                        UserRole = userRole,
                        Action = action,
                        Entity = entity,
                        StatusCode = statusCode,
                        Timestamp = DateTime.UtcNow
                    };

                    await unitOfWork.ActionLogs.Add(actionLog);

                    await unitOfWork.CompleteAsync();
                }
            }
        }
    }
}
