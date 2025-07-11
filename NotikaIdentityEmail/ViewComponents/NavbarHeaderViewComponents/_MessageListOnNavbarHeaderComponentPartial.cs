using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.MessageViewModel;

namespace NotikaIdentityEmail.ViewComponents.NavbarHeaderViewComponents;

public class _MessageListOnNavbarHeaderComponentPartial : ViewComponent
{
    private readonly UserManager<AppUser> _userManager;
    private readonly EmailContext _context;

    public _MessageListOnNavbarHeaderComponentPartial(UserManager<AppUser> userManager, EmailContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userValue = await _userManager.FindByNameAsync(User.Identity.Name);
        var userMail = userValue.Email;

        //var userM = PredicateBuilder.New<Message>();
        //userM.And(a => a.ReceiveEmail == userMail.ToString() && a.IsRead==false);

        var query =from message in _context.Messages/*.Where(userM)*/
                    join user in _context.Users
                    on message.SenderEmail equals user.Email
                    where message.ReceiveEmail==userMail && message.IsRead==false
                    select new MessageListWithUserInfoViewModel
                    {
                        FullName=user.Name+" "+ user.Surname,
                        MessageDetail=message.MessageDetail,
                        ProfileImageUrl=user.ImageUrl,
                        SendDate=message.SendDate
                    };
        return View(query.ToList());
    }
}
