using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents;

public class _MessageSidebarComponentPartial : ViewComponent
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _manager;

    public _MessageSidebarComponentPartial(EmailContext context, UserManager<AppUser> manager)
    {
        _context = context;
        _manager = manager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _manager.FindByNameAsync(User.Identity.Name);
        ViewBag.sendMessageCount =  _context.Messages.Where(a => a.SenderEmail == user.Email).Count();
        ViewBag.recieverMessageCount =  _context.Messages.Where(a => a.ReceiveEmail == user.Email).Count();
        return View();
    }
}

