using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.ViewComponents;

public class _HeaderUserLayoutComponentPartial : ViewComponent
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;

    public _HeaderUserLayoutComponentPartial(UserManager<AppUser> userManager, EmailContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userValue = await _userManager.FindByNameAsync(User.Identity.Name);
        var userMail = userValue.Email;
        var userEmailCount= _context.Messages.Where(a=>a.ReceiveEmail==userMail).Count();
        ViewBag.UserEmailCount = userEmailCount;
        ViewBag.notificationCount=_context.Natifications.Count();
        return View();
    }
}
