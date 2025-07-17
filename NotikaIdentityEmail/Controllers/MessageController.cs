using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using NotikaIdentityEmail.Models.MessageViewModel;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers;

public class MessageController : Controller
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;

    public MessageController(EmailContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Inbox()   //Gelen Kutusu
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var values = (from m in _context.Messages
                      join u in _context.Users
                      on m.SenderEmail equals u.Email into userGroup
                      from sender in userGroup.DefaultIfEmpty()

                      join c in _context.Categories
                      on m.CategoryId equals c.CategoryId into categoryGroup
                      from category in categoryGroup.DefaultIfEmpty()

                      where m.ReceiveEmail == user.Email
                      select new MessageWithSenderInfoViewModel
                      {
                          MessageId = m.MessageId,
                          MessageDetail = m.MessageDetail,
                          Konu = m.Konu,
                          SendDate = m.SendDate,
                          SenderEmail = m.SenderEmail,
                          SenderName = sender.Name != null ? sender.Name : "Bilinmeyen",
                          SenderSurname = sender.Surname != null ? sender.Surname : "Kullanıcı",
                          CategoryName = category.CategoryName != null ? category.CategoryName : "Yok"
                      }).ToList();
        return View(values);
    }
    public async Task<IActionResult> Sendbox()   //Giden Kutusu
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var query = (from m in _context.Messages
                     join u in _context.Users
                     on m.ReceiveEmail equals u.Email into userGroup
                     from reciever in userGroup.DefaultIfEmpty()

                     join c in _context.Categories
                     on m.CategoryId equals c.CategoryId into categoryGroup
                     from category in categoryGroup.DefaultIfEmpty()
                     where m.SenderEmail == user.Email
                     select new MessageWithRecieverInfoViewModel
                     {
                         MessageId = m.MessageId,
                         MessageDetail = m.MessageDetail,
                         Konu = m.Konu,
                         SendDate = m.SendDate,
                         RecieverEmail = m.ReceiveEmail,
                         RecieverName = reciever.Name != null ? reciever.Name : "Bilinmeyen",
                         RecieverSurname = reciever.Surname != null ? reciever.Surname : "Kullanıcı",
                         CategoryName = category.CategoryName != null ? category.CategoryName : "Yok"
                     }).ToList();
        return View(query);
    }

    public async Task<IActionResult> MessageDetail(int id)
    {
        var result = await _context.Messages.Where(a => a.MessageId == id).FirstOrDefaultAsync();
        return View(result);
    }

    [HttpGet]
    public IActionResult ComposeMessage()
    {
        var categories = _context.Categories.ToList();
        ViewBag.v = categories.Select(c => new SelectListItem
        {
            Text = c.CategoryName,
            Value = c.CategoryId.ToString()
        });
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ComposeMessage(Entities.Message message)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        message.SenderEmail=user.Email;
        message.SendDate = DateTime.Now;
        message.IsRead = false;
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
        return RedirectToAction("Sendbox");
    }

    public async Task<IActionResult> GetMessageListByCategory(int id)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var values = (from m in _context.Messages
                      join u in _context.Users
                      on m.SenderEmail equals u.Email into userGroup
                      from sender in userGroup.DefaultIfEmpty()

                      join c in _context.Categories
                      on m.CategoryId equals c.CategoryId into categoryGroup
                      from category in categoryGroup.DefaultIfEmpty()

                      where m.ReceiveEmail == user.Email && m.CategoryId==id
                      select new MessageWithSenderInfoViewModel
                      {
                          MessageId = m.MessageId,
                          MessageDetail = m.MessageDetail,
                          Konu = m.Konu,
                          SendDate = m.SendDate,
                          SenderEmail = m.SenderEmail,
                          SenderName = sender.Name != null ? sender.Name : "Bilinmeyen",
                          SenderSurname = sender.Surname != null ? sender.Surname : "Kullanıcı",
                          CategoryName = category.CategoryName != null ? category.CategoryName : "Yok"

                      }).ToList();
        return View(values);

    }
}
