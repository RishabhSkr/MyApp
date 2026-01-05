using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BackendAPI.Data;
using System.Security.Claims;

namespace BackendAPI.Services.Auth
{
    public class AccessRequirement : IAuthorizationRequirement { }

    public class AccessControlHandler : AuthorizationHandler<AccessRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public AccessControlHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null) return;

            // 1. URL Path se Route nikalna (api/RawMaterial/Create -> RawMaterial/Create)
            var path = httpContext.Request.Path.Value?.Trim('/');
            if (string.IsNullOrEmpty(path)) return;

            var segments = path.Split('/');
            // api/Controller/Action" structure 
            string endpointRoute = segments.Length >= 3 ? $"{segments[1]}/{segments[2]}" : "";

            // 2. Token se RoleId nikalna
            var roleIdStr = context.User.FindFirstValue(ClaimTypes.Role);

            if (int.TryParse(roleIdStr, out int roleId))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    
                    // 3. Database Check
                    var hasAccess = await db.RolePermissions
                        .AnyAsync(rp => rp.RoleId == roleId && rp.RouteName == endpointRoute);

                    if (hasAccess)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
            context.Fail(); // Agar permission nahi mili
        }
    }
}
