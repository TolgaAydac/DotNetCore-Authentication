using Authentication.Models;
using FluentValidation;

namespace Authentication.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kullanıcı Adı Gerekli.")
            .Length(3, 20)
            .WithMessage("Kullanıcı adı 3 ile 20 karakter arasında olmalıdır.")
            .Matches("^[0-9A-Za-z]+$").WithMessage("Kullanıcı adı sadece alfanümerik karakter (A-Z , a-z , 0-9) içerebilir.");

            RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Geçerli bir Email adresi giriniz.")
            .Length(5, 20).WithMessage("Email 5 ile 20 karakter arasında olmalıdır.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email .com gibi geçerli bir alan adı içermelidir.");

            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre Gerekli.")
            .Length(5, 20)
            .Matches(@"(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>])").WithMessage("Minimum bir büyük harf,sayısal değer ve özel karakter içermelidir.");

            RuleFor(x => x.Birthday)
            .NotEmpty().WithMessage("Doğum tarihi gerekli.")
            .Must(date => DateTime.TryParse(date.ToString(), out _))
            .WithMessage("Tarih formatı hatalı.")
            .Must(date => date <= DateTime.Now.AddYears(-10))
            .WithMessage("Doğum tarihi 10 seneden yeni olamaz.");

            RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Cinsiyet zorunludur.");

        }
    }
}