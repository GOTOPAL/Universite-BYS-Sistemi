using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using qBYS.Models;

namespace qBYS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Login sayfasını gösteren aksiyon
        public IActionResult Login()
        {




            return View();
        }

        // Şifremi Unuttum sayfasını gösteren aksiyon
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Şifre sıfırlama linki gönderme aksiyonu
        [HttpPost]
        public IActionResult SendResetLink(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                // Gerçek bir e-posta gönderimi yapılabilir (örnek mesaj eklenmiştir)
                ViewBag.Message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            }
            else
            {
                ViewBag.ErrorMessage = "E-posta adresi ile eşleşen bir kullanıcı bulunamadı.";
            }

            return View("ForgotPassword");
        }

        // Kullanıcı girişini kontrol eden aksiyon
        [HttpPost]
        public async Task<IActionResult> LoginUser(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ViewBag.ErrorMessage = "Kullanıcı adı, şifre veya rol boş olamaz.";
                return View("Login");
            }

            // Kullanıcıyı kontrol et
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Role == role);

            if (user != null)
            {
                // Şifre doğrulama (örnek: Hash'li şifre karşılaştırması)
                if (user.PasswordHash != password) // Şifre doğrulama mekanizması eklemelisiniz
                {
                    ViewBag.ErrorMessage = "Şifre yanlış.";
                    return View("Login");
                }

                // Session'a UserID ekle
                HttpContext.Session.SetString("UserID", user.UserID.ToString());

                if (role == "Student")
                {
                    // Öğrenci ID'sini oturuma ekle (RelatedID direkt kullanılabilir)
                    if (user.RelatedID.HasValue)
                    {
                        HttpContext.Session.SetString("StudentID", user.RelatedID.Value.ToString());

                    }
                    else
                    {
                        ViewBag.ErrorMessage = "İlgili öğrenci bilgisi bulunamadı.";
                        return View("Login");
                    }

                    return RedirectToAction("StudentPanel", "Student");
                }
                else if (role == "Advisor")
                {
                    // Advisor ID'sini oturuma ekle
                    if (user.RelatedID.HasValue)
                    {
                        HttpContext.Session.SetString("AdvisorID", user.RelatedID.Value.ToString());
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "İlgili danışman bilgisi bulunamadı.";
                        return View("Login");
                    }

                    return RedirectToAction("AdvisorPanel", "Advisor");
                }
            }

            // Kullanıcı bulunamazsa
            ViewBag.ErrorMessage = "Geçersiz kullanıcı adı, şifre veya rol.";
            return View("Login");
        }
    }
}
