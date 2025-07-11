using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents;

public class _CategoryListSidebarComponentPartial : ViewComponent
{
    private readonly EmailContext _context;

    public _CategoryListSidebarComponentPartial(EmailContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var result = _context.Categories.ToList();
        return View(result);
    }
}
