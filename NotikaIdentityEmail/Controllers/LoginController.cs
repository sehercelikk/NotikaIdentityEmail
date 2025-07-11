using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers;

public class LoginController : Controller
{
    private readonly SignInManager<AppUser> _signInmanager;
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;

    public LoginController(SignInManager<AppUser> signInmanager, EmailContext context, UserManager<AppUser> userManager)
    {
        _signInmanager = signInmanager;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult UserLogin()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> UserLogin(LoginUserViewModel model)
    {
        var isConfirm = _context.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();

        if (isConfirm == null)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı");
            return View(model);
        }
        if (!isConfirm.EmailConfirmed)
        {
            ModelState.AddModelError(string.Empty, "Lütfen size gönderilen bağlantı adresinden Email adresinizi onaylayın.");
            return View(model);
        }

        var result = await _signInmanager.PasswordSignInAsync(model.UserName, model.Password, true, true);
        if (result.Succeeded)
        {
            return RedirectToAction("EditProfile", "Profile");
        }
        ModelState.AddModelError(string.Empty, "Kullanıcı adı veya Şifre yanlış");
        return View(model);

    }


    [HttpGet]
    public IActionResult LoginWithGoogle()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ExternalLogin(string provider, string? returnUrl=null)
    {
        var redirectUrl = Url.Action("ExternalLoginCallBack", "Login", new {returnUrl});
        var prop = _signInmanager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(prop, provider);
    }

    [HttpPost]
    public async Task<IActionResult> ConfigureExternalAuthenticationProperties(string? returnUrl=null, string? remoteError=null)
    {
        returnUrl ??= Url.Content("~/");
        if(remoteError == null)
        {
            ModelState.AddModelError("", $"External Provider Error: {remoteError}");
            return RedirectToAction("UserLogin");
        }
        var info = await _signInmanager.GetExternalLoginInfoAsync();
        if(info == null)
        {
            return RedirectToAction("UserLogin");
        }

        var result = await _signInmanager.ExternalLoginSignInAsync(info.LoginProvider,info.ProviderKey,false);
        if(result.Succeeded)
        {
            return RedirectToAction("Message","Inbox");
        }
        else
        {
            var email=info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new AppUser
            {
                UserName = email,
                Email = email,
                Name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Google",
                Surname = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User"
            };


            var identityResult=await _userManager.CreateAsync(user);
            if(identityResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInmanager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Message", "Inbox");

            }
            return RedirectToAction("UserLogin");
        }

    }
}
