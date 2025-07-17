using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers;

public class UserController : Controller
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;

    public UserController(UserManager<AppUser> manager, EmailContext context)
    {
        _userManager = manager;
        _context = context;
    }

    public async Task<IActionResult> UserList()
    {
        var result = await _userManager.Users.ToListAsync();
        return View(result);
    }

    public async Task<IActionResult> DeactiveUserCount(string id)
    {
        var user= await _userManager.FindByIdAsync(id);
        user.IsActive = false;
        await _userManager.UpdateAsync(user);
        return RedirectToAction("UserList");
    }

    public async Task<IActionResult> ActiveUserCount(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        user.IsActive = true;
        await _userManager.UpdateAsync(user);
        return RedirectToAction("UserList");
    }
}
