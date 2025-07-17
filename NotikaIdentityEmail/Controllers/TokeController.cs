using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Models.JWTModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotikaIdentityEmail.Controllers;

public class TokeController : Controller
{
    private readonly JwtSettingsModel _jwtSettingModel;

    public TokeController(IOptions<JwtSettingsModel> jwtSettingModel)
    {
        _jwtSettingModel = jwtSettingModel.Value;
    }

    [HttpGet]
    public IActionResult Generate()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Generate(SimpleUserViewModel model)
    {
        var claim = new[]
        {
            new Claim("name",model.Name),
            new Claim("surname",model.Surname),
            new Claim("city",model.City),
            new Claim("username",model.UserName),
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
        model.Token=new JwtSecurityTokenHandler().WriteToken(token);
        return View(model);
    }

}
