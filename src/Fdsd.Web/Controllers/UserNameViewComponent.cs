using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;

namespace Fdsd.Web.Controllers;

public class UserNameViewComponent : ViewComponent
{
    private readonly UserService _userService;

    public UserNameViewComponent(UserService userService) => _userService = userService;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var identity = User.Identity?.Name ?? "";
        var idx = identity.IndexOf('\\');
        var account = idx >= 0 ? identity.Substring(idx + 1) : identity;

        var user = await _userService.GetByEmpUserNmAsync(account);
        var name = user?.EmpName ?? account;
        return Content(name);
    }
}