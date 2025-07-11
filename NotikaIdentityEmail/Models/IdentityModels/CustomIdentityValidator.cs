using Microsoft.AspNetCore.Identity;

namespace NotikaIdentityEmail.Models.IdentityModels;

public class CustomIdentityValidator : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError()
        {
            Code = "PasswordTooShort",
            Description = $"Şifreniz en az {length} karkater içermelidir"
        };

    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError()
        {
            Code = "PasswordRequiresLower",
            Description = "Şifreniz en az 1 tane küçük harf içermeli"
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError()
        {
            Code = "PasswordRequiresUpper",
            Description = "Şifreniz en az 1 tane büyük harf içermeli"
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError()
        {
            Code = "PasswordRequiresDigit",
            Description = "Şifre ne az 1 tane rakam içermelidir"
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError()
        {
            Code = "PasswordRequiresNonAlphanumeric",
            Description = "Şifreniz en az 1 tane alfanumeric ifade içermelidir"
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError()
        {
            Code = "DuplicateUserName",
            Description = $"{userName} kullanıcı adı zaten mevcut!"
        };
    }
}
