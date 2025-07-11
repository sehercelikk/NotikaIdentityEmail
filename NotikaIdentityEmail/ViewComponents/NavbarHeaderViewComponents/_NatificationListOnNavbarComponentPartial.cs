using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;

namespace NotikaIdentityEmail.ViewComponents.NavbarHeaderViewComponents;

public class _NatificationListOnNavbarComponentPartial : ViewComponent
{
    private readonly EmailContext _context;

    public _NatificationListOnNavbarComponentPartial(EmailContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var values = _context.Natifications.ToList();
        return View(values);
    }
}
