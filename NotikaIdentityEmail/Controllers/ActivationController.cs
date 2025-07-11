using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;

namespace NotikaIdentityEmail.Controllers;
// xemw ayuu jvjb sjbb
public class ActivationController : Controller
{
    private readonly EmailContext _context;

    public ActivationController(EmailContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult UserActivation()
    {
        var email = TempData["EmailMove"].ToString();
        if(email !=null)
        {
            HttpContext.Session.SetString("email", email);
        }
        return View();
    }
    [HttpPost]
    public IActionResult UserActivation(int userCodeParameter)
    {
        string email = HttpContext.Session.GetString("email");
        var code = _context.Users.Where(a => a.Email == email).Select(y => y.ActivationCode).FirstOrDefault();
        if (userCodeParameter == code)
        {
            var value = _context.Users.Where(a => a.Email == email).FirstOrDefault();
            value.EmailConfirmed= true;
            _context.SaveChanges();
            return RedirectToAction("UserLogin", "Login");
        }
        return View();
    }
}
