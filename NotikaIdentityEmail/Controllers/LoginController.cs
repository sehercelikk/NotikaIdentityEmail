using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Models.JWTModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers;

public class LoginController : Controller
{
    private readonly SignInManager<AppUser> _signInmanager;
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSettingsModel _jwtSettingModel;

    public LoginController(SignInManager<AppUser> signInmanager, EmailContext context, UserManager<AppUser> userManager, IOptions<JwtSettingsModel> jwtSettings)
    {
        _signInmanager = signInmanager;
        _context = context;
        _userManager = userManager;
        _jwtSettingModel = jwtSettings.Value;
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

        if (!isConfirm.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı Pasif Durumda, Giriş Yapamaz");
            return View(model);
        }

        SimpleUserViewModel simpleUserViewModel = new SimpleUserViewModel()
        {
            City = isConfirm.City,
            Email = isConfirm.Email,
            Id = isConfirm.Id,
            Name = isConfirm.Name,
            Surname = isConfirm.Surname,
            UserName = isConfirm.UserName
        };

        var result = await _signInmanager.PasswordSignInAsync(model.UserName, model.Password, true, true);
        if (result.Succeeded)
        {
            var token = GenerateJwtToken(simpleUserViewModel);
            Response.Cookies.Append("jwtToken",token, new CookieOptions
            {
                HttpOnly = true,
                Secure=true,
                SameSite=SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettingModel.ExpireMinutes)
            });
            
            return RedirectToAction("EditProfile", "Profile");
        }
        ModelState.AddModelError(string.Empty, "Kullanıcı adı veya Şifre yanlış");
        return View(model);

    }

    public string GenerateJwtToken(SimpleUserViewModel model)
    {
        var claim = new[]
        {
            new Claim("name",model.Name),
            new Claim("surname",model.Surname),
            new Claim("city",model.City),
            new Claim("username",model.UserName),
            new Claim(ClaimTypes.NameIdentifier,model.Id),
            new Claim(ClaimTypes.Email,model.Email),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettingModel.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettingModel.Issuer,
            audience: _jwtSettingModel.Audience,
            claims: claim,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettingModel.ExpireMinutes),
            signingCredentials: creds);
        //model.Token = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtSecurityTokenHandler().WriteToken(token);
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
