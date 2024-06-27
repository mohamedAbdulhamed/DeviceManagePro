using DevicesApp.Core.IConfiguration;
using DevicesApp.Data;
using DevicesApp.Models;
using System.Security.Claims;

namespace DevicesApp.Middlewares;

public class ActionLoggingMiddleware(RequestDelegate next, ILogger<ActionLoggingMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ActionLoggingMiddleware> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.User.Identity?.IsAuthenticated == true)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
                var userName = context.User.Identity.Name ?? "Anonymous";
                var userRole = context.User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";
                var action = context.Request.Method + " " + context.Request.Path;
                var entity = context.Request.Path.Value?.Split('/')[2] ?? "Unknown";

                var actionLog = new ActionLog
                {
                    UserId = userId,
                    UserName = userName,
                    UserRole = userRole,
                    Action = action,
                    Entity = entity,
                };

                await unitOfWork.ActionLogs.Add(actionLog);
                await unitOfWork.CompleteAsync();
            }
        }
    }
}
