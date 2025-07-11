using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.ForgetPasswordModels;

namespace NotikaIdentityEmail.Controllers;

public class PasswordChangeController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public PasswordChangeController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
    {
        var user = await _userManager.FindByEmailAsync(forgetPasswordViewModel.Email);
        string passwordResetToken= await _userManager.GeneratePasswordResetTokenAsync(user);
        var passwordTokenLink = Url.Action("ResetPassword","PasswordChange", new
        {
            userId=user.Id,
            token=passwordResetToken

        },HttpContext.Request.Scheme);

        MimeMessage mimeMessage=new MimeMessage();

        MailboxAddress mailboxAddressFrom = new MailboxAddress("NotikaAdmin","ylddyildiz@gmail.com");
        mimeMessage.From.Add(mailboxAddressFrom);
        
        MailboxAddress mailboxAddressTo= new MailboxAddress("User",forgetPasswordViewModel.Email);
        mimeMessage.To.Add(mailboxAddressTo);

        var bodyBuilder = new BodyBuilder();
        bodyBuilder.TextBody = passwordTokenLink;
        mimeMessage.Body = bodyBuilder.ToMessageBody();
        mimeMessage.Subject = "Şifre Değişiklik Talebi";
        SmtpClient smtpClient = new SmtpClient();
        smtpClient.Connect("smtp.gmail.com", 587, false);
        smtpClient.Authenticate("yldzzseher6@gmail.com", "xemwayuujvjbsjbb");
        smtpClient.Send(mimeMessage);
        smtpClient.Disconnect(true);
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string userId, string token)
    {
        TempData["UserId"]= userId;
        TempData["Token"]= token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        var userId = TempData["UserId"];
        var token = TempData["Token"];
        if(userId==null || token==null)
        {
            ViewBag.v = "Hata Oluştu";
        }
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var result = await _userManager.ResetPasswordAsync(user, token.ToString(),model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction("UserLogin","Login");
        }
        return View();

    }
}
