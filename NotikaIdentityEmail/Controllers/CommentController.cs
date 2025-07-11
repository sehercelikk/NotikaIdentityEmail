using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.Data.Entity;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers;

public class CommentController : Controller
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CommentController(EmailContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult UserComments()
    {
        var result = _context.Comments.Include(a => a.AppUser).ToList();
        return View(result);
    }


    [Authorize(Roles ="Admin")]
    public IActionResult UserCommentList()
    {
        var values = _context.Comments.Include(a => a.AppUser).ToList();
        return View(values);
    }

    public PartialViewResult CreateComment()
    {
        return PartialView();
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment(Comment comment)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        comment.AppUserId = user.Id;
        comment.CommentDate = DateTime.Now;

        // Toxic Bert API analizi
        using (var client = new HttpClient())
        {
            var apiKey = "youreToken";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var translateRequestBody = new
                {
                    inputs = comment.Detail
                };
                var translateJson = JsonSerializer.Serialize(translateRequestBody);
                var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");

                var translateResponse = await client.PostAsync("https://api-inference.huggingface.co/models/Helsinki-NLP/opus-mt-tr-en", translateContent);
                var translateResponseString = await translateResponse.Content.ReadAsStringAsync();
                string englishText = comment.Detail;
                if (translateResponseString.TrimStart().StartsWith("["))
                {
                    var translateDoc = JsonDocument.Parse(translateResponseString);
                    englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString();
                }

                var toxicRequestBody = new
                {
                    inputs = englishText,
                };

                var toxicJson = JsonSerializer.Serialize(toxicRequestBody);
                var toxicContent = new StringContent(toxicJson, Encoding.UTF8, "application/json");

                var toxicResponse = await client.PostAsync("https://api-inference.huggingface.co/models/unitary/toxic-bert", toxicContent);

                var toxicResponseString = await toxicResponse.Content.ReadAsStringAsync();

                if (toxicResponseString.TrimStart().StartsWith("["))
                {
                    var toxicDoc = JsonDocument.Parse(toxicResponseString);
                    foreach (var item in toxicDoc.RootElement[0].EnumerateArray())
                    {
                        string label = item.GetProperty("label").GetString();
                        double score = item.GetProperty("score").GetDouble();

                        if (score > 0)
                        {
                            comment.CommentStatus = "Toksik Yorum";
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(comment.CommentStatus))
                {
                    comment.CommentStatus = "Yorum Onaylandı";
                }
            }
            catch (Exception ex)
            {
                comment.CommentStatus = "Onay Bekliyor";

            }

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("UserCommentList");
        }
    }

    public IActionResult DeleteComment(int id)
    {
        var value = _context.Comments.Find(id);
        _context.Comments.Remove(value);
        _context.SaveChanges();
        return RedirectToAction("UserCommentList");
    }

    public IActionResult CommentStatusChangeToToxic(int id)
    {
        var value = _context.Comments.Find(id);
        value.CommentStatus = "Toksik Yorum";
        _context.SaveChanges();
        return RedirectToAction("UserCommentList");
    }

    public IActionResult CommentStatusChangeToPasif(int id)
    {
        var value = _context.Comments.Find(id);
        value.CommentStatus = "Yorum Kaldırıldı";
        _context.SaveChanges();
        return RedirectToAction("UserCommentList");
    }
    public IActionResult CommentStatusChangeToActive(int id)
    {
        var value = _context.Comments.Find(id);
        value.CommentStatus = "Yorum Onaylandı";
        _context.SaveChanges();
        return RedirectToAction("UserCommentList");
    }

}
