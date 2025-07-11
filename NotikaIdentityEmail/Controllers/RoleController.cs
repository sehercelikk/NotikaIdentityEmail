using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers;

[Authorize]
public class RoleController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    [HttpGet]
    public async Task<IActionResult> RoleList()
    {
        var result = await _roleManager.Roles.ToListAsync();
        return View(result);
    }

    [HttpGet]
    public IActionResult CreateRole()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
    {
        IdentityRole role = new IdentityRole
        {
            Name = model.RoleName,
        };

        await _roleManager.CreateAsync(role);
        return RedirectToAction("RoleList");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var result = await _roleManager.Roles.FirstOrDefaultAsync(a=>a.Id==id);
        await _roleManager.DeleteAsync(result);
        return RedirectToAction("RoleList");
    }

    [HttpGet]
    public async Task<IActionResult> UpdateRole(string id)
    {
        var result = await _roleManager.Roles.FirstOrDefaultAsync(a => a.Id == id);
        UpdateRoleViewModel model = new UpdateRoleViewModel()
        {
            RoleId=result.Id,
            RoleName=result.Name
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRole(UpdateRoleViewModel model )
    {
        var values = await _roleManager.Roles.FirstOrDefaultAsync(a => a.Id == model.RoleId);
        values.Name = model.RoleName;
        await _roleManager.UpdateAsync(values);
        return RedirectToAction("RoleList");
    }

    public async Task<IActionResult> UserList()
    {
        var result = await _userManager.Users.ToListAsync();
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> AssignRole(string id)
    {
        var user= await _userManager.Users.FirstOrDefaultAsync(a=>a.Id == id);
        TempData["userId"] = user.Id;
        var roles=await _roleManager.Roles.ToListAsync();
        var userRoles = await _userManager.GetRolesAsync(user);
        List<RoleAssignViewModel> roleAssignViewModels = new List<RoleAssignViewModel>();
        foreach (var item in roles)
        {
            RoleAssignViewModel model = new RoleAssignViewModel();
            model.RoleId = item.Id;
            model.RoleName = item.Name;
            model.RoleExist = userRoles.Contains(item.Name);
            roleAssignViewModels.Add(model);
        }
        return View(roleAssignViewModels);

    }

    [HttpPost]
    public async Task<IActionResult> AssignRole(List<RoleAssignViewModel> model)
    {
        var userId = TempData["userId"].ToString();
        var user=await _userManager.Users.FirstOrDefaultAsync(a=>a.Id==userId);
        foreach (var item in model)
        {
            if(item.RoleExist)
            {
                await _userManager.AddToRoleAsync(user, item.RoleName);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, item.RoleName);
            }
        }
        return RedirectToAction("UserList");
    }
}
