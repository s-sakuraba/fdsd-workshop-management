using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Fdsd.Application.Master;

namespace Fdsd.Web.Authorization;

public class AdminRequirement : IAuthorizationRequirement { }

public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
{
    private readonly UserService _userService;

    public AdminAuthorizationHandler(UserService userService)
    {
        _userService = userService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {
        var identity = context.User.Identity?.Name ?? "";
        var idx = identity.IndexOf('\\');
        var account = idx >= 0 ? identity.Substring(idx + 1) : identity;

        if (!string.IsNullOrEmpty(account) && await _userService.IsAdministratorAsync(account))
        {
            context.Succeed(requirement);
        }
    }
}