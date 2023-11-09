using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace contactPro2.Controllers
{
    public abstract class CPBaseController : Controller
    {
            protected string? _userId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}
