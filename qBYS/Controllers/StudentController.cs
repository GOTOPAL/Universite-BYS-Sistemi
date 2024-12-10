using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace qBYS.Controllers
{
    public class StudentController : Controller
    {
        private readonly HttpClient _httpClient;

        public StudentController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Öğrenci Panelini Görüntüleme
        public async Task<IActionResult> StudentPanel()
        {
            var studentIdString = HttpContext.Session.GetString("StudentID");

            if (string.IsNullOrEmpty(studentIdString))
            {
                // Oturum yoksa giriş sayfasına yönlendirme
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(studentIdString, out int studentId))
            {
                return BadRequest("Geçersiz öğrenci ID'si.");
            }

            // Öğrenci bilgilerini API'den çek
            var response = await _httpClient.GetAsync($"https://localhost:7268/api/Students/{studentId}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Öğrenci bilgileri yüklenemedi.";
                return View("Error");
            }

            var studentJson = await response.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<JObject>(studentJson);

            // Danışman bilgilerini al
            var advisor = student["advisor"];
            ViewBag.Student = student;
            if (advisor != null)
            {
                ViewBag.Advisor = new
                {
                    FullName = advisor["fullName"]?.ToString(),
                    Title = advisor["title"]?.ToString(),
                    Department = advisor["department"]?.ToString()
                };
            }
            else
            {
                ViewBag.Advisor = null;
            }

            // Dersleri API'den çek
            var courseResponse = await _httpClient.GetAsync("https://localhost:7268/api/Courses");
            if (!courseResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Ders bilgileri yüklenemedi. Lütfen tekrar deneyin.";
                return View("StudentPanel");
            }
            var courseJson = await courseResponse.Content.ReadAsStringAsync();
            var courses = JsonConvert.DeserializeObject<List<dynamic>>(courseJson);

            // Dersleri ViewBag'e aktar
            ViewBag.Courses = courses;



            return View("StudentPanel");


        }



        [HttpGet]
        public async Task<IActionResult> CourseSelection()
        {
            try
            {
                // API'den dersleri al
                var response = await _httpClient.GetAsync("https://localhost:7268/api/Courses");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "API'den ders bilgileri alınamadı.";
                    return View("CourseSelection");
                }

                // JSON yanıtını al ve deserialize et
                var coursesJson = await response.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<dynamic>>(coursesJson);

                if (courses == null || !courses.Any())
                {
                    ViewBag.ErrorMessage = "Ders listesi bulunamadı.";
                    return View("CourseSelection");
                }

                ViewBag.Courses = courses; // Ders listesini ViewBag'e aktar
                return View("CourseSelection"); // Ders seçimi sayfasını göster
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                ViewBag.ErrorMessage = "Bir hata oluştu.";
                return View("CourseSelection");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCourseSelections(List<int> selectedCourseIds)
        {
            if (selectedCourseIds == null || selectedCourseIds.Count == 0)
            {
                TempData["CourseSelectionMessage"] = "Lütfen en az bir ders seçin.";
                return RedirectToAction("StudentPanel");
            }

            var studentIdString = HttpContext.Session.GetString("StudentID");
            if (string.IsNullOrEmpty(studentIdString) || !int.TryParse(studentIdString, out int studentId))
            {
                TempData["CourseSelectionMessage"] = "Oturum bilgisi alınamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var courseId in selectedCourseIds)
            {
                var nonConfirmedSelection = new
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    SelectedAt = DateTime.Now
                };

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7268/api/NonConfirmedSelections", nonConfirmedSelection);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["CourseSelectionMessage"] = "Seçiminiz kaydedilemedi. Lütfen tekrar deneyin.";
                    return RedirectToAction("StudentPanel");
                }
            }

            TempData["CourseSelectionMessage"] = "Ders seçiminiz başarıyla danışman onayına gönderildi!";

            return RedirectToAction("StudentPanel");
        }











        [HttpPost]
        public async Task<IActionResult> UpdateStudent(int id, string newEmail, string newPassword, int? advisorID)
        {
            var studentIdString = HttpContext.Session.GetString("StudentID");
            var userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(studentIdString) || !int.TryParse(studentIdString, out int studentId))
            {
                TempData["ErrorMessage"] = "Oturum bilgisi alınamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Account");
            }

            if (studentId != id)
            {
                TempData["ErrorMessage"] = "Geçersiz işlem. ID eşleşmiyor.";
                return RedirectToAction("StudentPanel");
            }

            // Students API'den mevcut öğrenci bilgilerini al
            var response = await _httpClient.GetAsync($"https://localhost:7268/api/Students/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Öğrenci bilgileri alınamadı.";
                return RedirectToAction("StudentPanel");
            }

            var studentJson = await response.Content.ReadAsStringAsync();
            var student = JsonConvert.DeserializeObject<JObject>(studentJson);

            // AdvisorID'yi kontrol et ve mevcut değeri koru
            var updatedAdvisorID = advisorID ?? student["advisor"]?["advisorID"]?.ToObject<int?>();

            // Güncellenmiş JSON'u oluştur
            var updatedStudent = new
            {
                StudentID = id,
                FirstName = student["firstName"]?.ToString(),
                LastName = student["lastName"]?.ToString(),
                Email = string.IsNullOrEmpty(newEmail) ? student["email"]?.ToString() : newEmail,
                AdvisorID = updatedAdvisorID,
                NonConfirmedSelections = student["nonConfirmedSelections"] ?? new JArray()
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedStudent), System.Text.Encoding.UTF8, "application/json");

            // Students API'ye PUT isteği gönder
            var updateResponse = await _httpClient.PutAsync($"https://localhost:7268/api/Students/{id}", jsonContent);

            if (!updateResponse.IsSuccessStatusCode)
            {
                var errorContent = await updateResponse.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Öğrenci bilgileri güncellenirken bir hata oluştu: {errorContent}";
                return RedirectToAction("StudentPanel");
            }

            // Users API'de email ve/veya şifre güncellemesi
            if (!string.IsNullOrEmpty(userIdString) && (!string.IsNullOrEmpty(newEmail) || !string.IsNullOrEmpty(newPassword)))
            {
                var userResponse = await _httpClient.GetAsync($"https://localhost:7268/api/Users/{userIdString}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                    return RedirectToAction("StudentPanel");
                }

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<JObject>(userJson);

                var updatedUser = new
                {
                    UserID = user["userID"],
                    Username = user["username"]?.ToString(),
                    PasswordHash = !string.IsNullOrEmpty(newPassword) ? newPassword : user["passwordHash"]?.ToString(),
                    Email = !string.IsNullOrEmpty(newEmail) ? newEmail : user["email"]?.ToString(),
                    Role = user["role"]?.ToString(),
                    RelatedID = user["relatedID"]
                };

                var userJsonContent = new StringContent(JsonConvert.SerializeObject(updatedUser), System.Text.Encoding.UTF8, "application/json");

                var updateUserResponse = await _httpClient.PutAsync($"https://localhost:7268/api/Users/{userIdString}", userJsonContent);

                if (!updateUserResponse.IsSuccessStatusCode)
                {
                    var errorContent = await updateUserResponse.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Kullanıcı bilgileri güncellenirken bir hata oluştu: {errorContent}";
                    return RedirectToAction("StudentPanel");
                }
            }

            TempData["SuccessMessage"] = "Bilgiler başarıyla güncellendi.";
            return RedirectToAction("StudentPanel");
        }
    }
}
