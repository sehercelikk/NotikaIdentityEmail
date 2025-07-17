using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<AppUser> _manager;

    public ProfileController(UserManager<AppUser> manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var values = await _manager.FindByNameAsync(User.Identity.Name);
        UserEditViewModel userEditViewModel = new UserEditViewModel();
        userEditViewModel.Name = values.Name;
        userEditViewModel.Surname = values.Surname;
        userEditViewModel.PhoneNumber = values.PhoneNumber;
        userEditViewModel.ImageUrl = values.ImageUrl;
        userEditViewModel.City = values.City;
        userEditViewModel.UserName = values.UserName;
        userEditViewModel.Email = values.Email;
        return View(userEditViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(UserEditViewModel model)
    {
        if (model.Password == model.PasswordConfirm)
        {
            var user = await _manager.FindByNameAsync(User.Identity.Name);
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.PhoneNumber = model.PhoneNumber;
            user.ImageUrl = model.ImageUrl;
            user.City = model.City;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PasswordHash = _manager.PasswordHasher.HashPassword(user, model.Password);
            await _manager.UpdateAsync(user);
        }
        return View();
    }
}
