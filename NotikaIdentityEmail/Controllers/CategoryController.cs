using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace NotikaIdentityEmail.Controllers;

public class CategoryController : Controller
{
    private readonly EmailContext _context;

    public CategoryController(EmailContext context)
    {
        _context = context;
    }

    public IActionResult CategoryList()
    {
        var token = Request.Cookies["jwtToken"];
        if(string.IsNullOrEmpty(token))
        {
            TempData["error"] = "Giriş Yapmalısınız";
            return RedirectToAction("UserLogin", "Login");
        }
        JwtSecurityToken jwt;
        try
        {
            var handler = new JwtSecurityTokenHandler();
            jwt=handler.ReadJwtToken(token);
        }
        catch
        {
            TempData["error"] = "Token Geçersiz";
            return RedirectToAction("UserLogin", "Login");
        }
        var city = jwt.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
        if(city!="Ankara")
        {
            return Forbid();
        }

        var values = _context.Categories.ToList();
        return View(values);
    }

    [HttpGet]
    public IActionResult Create() => View();


    [HttpPost]
    public IActionResult Create(Category category)
    {
        category.Status = true;
       _context.Categories.Add(category);
        _context.SaveChanges();
        return RedirectToAction("CategoryList");
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var value= _context.Categories.Find(id);
        return View(value);
    }

    [HttpPost]
    public IActionResult Update(Category category)
    {
        var values= _context.Categories.Update(category);
        _context.SaveChanges();
        return RedirectToAction("CategoryList");
    }

    public IActionResult Delete(int id)
    {
        var values= _context.Categories.Find(id);
        _context.Categories.Remove(values);
        _context.SaveChanges();
        return RedirectToAction("CategoryList");
    }
}
