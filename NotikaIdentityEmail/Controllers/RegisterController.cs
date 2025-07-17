using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers;

public class RegisterController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    public RegisterController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    [HttpGet]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(RegesterUserViewModel model)
    {
        Random rnd=new Random();
        int code=rnd.Next(100000,1000000);
        AppUser appUser = new AppUser()
        {
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email,
            UserName = model.UserName,
            ActivationCode = code,
        };
        var result = await _userManager.CreateAsync(appUser, model.Password);
        if (result.Succeeded)
        {
            // xemw ayuu jvjb sjbb
            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin","yldzzseher6@gmail.com");

            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User",model.Email);
            mimeMessage.To.Add(mailboxAddressTo);
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = "Hesap doğrulama işlemi için gerekli aktivasyon kodu: " + code;
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = "Notika Identity Activation Code";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.gmail.com", 587, false);
            smtpClient.Authenticate("yldzzseher6@gmail.com", "xemwayuujvjbsjbb");
            smtpClient.Send(mimeMessage);
            smtpClient.Disconnect(true);

            TempData["EmailMove"] = model.Email;

            return RedirectToAction("UserActivation", "Activation");
        }
        else
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
        }
        return View();
    }
}
