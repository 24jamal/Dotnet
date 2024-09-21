using INTERNMVCNew.DAL;
using Microsoft.AspNetCore.Mvc;
using INTERNMvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing;
using INTERNMvc.DAL;


namespace INTERNMVCNew.Controllers
{
    public class UserController : Controller
    {
        private static string captchaCode;
        public IActionResult Index()
        {
            return View();
        }
        private readonly UserDAL userDAL;

        public UserController(UserDAL userDAL)
        {
            this.userDAL = userDAL;
        }


        [HttpGet]
        public IActionResult Login()
        {
            // Clear any previous error messages
            TempData["errorMessage"] = "";
            return View();
        }

        [HttpPost]
        public IActionResult Login(User model, string captcha)
        {
            string storedCaptcha = HttpContext.Session.GetString("CaptchaCode");

            if (storedCaptcha != captcha)
            {
                TempData["errorMessage"] = "Invalid CAPTCHA.";
                return View();
            }

            var user = userDAL.ValidateUser(model.Username, model.Password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("SingleView", "Employee");
            }
            else
            {
                TempData["errorMessage"] = "Invalid username or password.";
                return View();
            }
        }

        private readonly string _connectionString = "Data Source=10.250.4.92,2021;Initial Catalog=Internship;Persist Security Info=True;User ID=Intern;Password=2^pYR9_|z41d;TrustServerCertificate=True";

        [HttpGet]
        public ActionResult ChangePassword()
        {
            // Retrieve the username from the session
            var username = HttpContext.Session.GetString("Username");

            // If the user is not logged in, redirect to the login page
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "User");
            }

            // Initialize the ChangePasswordModel with the Username from the session
            var model = new ChangePasswordModel
            {
                Username = username
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Verify current password and username
                    string query = "SELECT UserId FROM UsersJ WHERE Username = @Username AND Password = @CurrentPassword";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@CurrentPassword", model.CurrentPassword);

                    object userId = cmd.ExecuteScalar();

                    if (userId != null)
                    {
                        // Update to new password
                        query = "UPDATE UsersJ SET Password = @NewPassword WHERE UserId = @UserId";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@NewPassword", model.NewPassword);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        cmd.ExecuteNonQuery();
                        ViewBag.Message = "Password changed successfully!";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Current password or username is incorrect.");
                    }
                }
            }

            return View(model);


        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToAction("Login", "User"); // Redirect to Login page
        }

        [HttpGet]
        public IActionResult GenerateCaptcha()
        {
            int width = 100;
            int height = 40;
            var randomText = GenerateRandomText();

            using (var bitmap = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(bitmap))
            using (var font = new Font("Consolas", 18, FontStyle.Bold))
            using (var ms = new MemoryStream())
            {
                graphics.Clear(Color.Azure);
                graphics.DrawString(randomText, font, Brushes.Black, new PointF(10, 5));

                bitmap.Save(ms, ImageFormat.Png);

                HttpContext.Session.SetString("CaptchaCode", randomText);

                return File(ms.ToArray(), "image/png");
            }
        }

        private string GenerateRandomText()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new char[5];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }


        [HttpGet]
        public IActionResult Register()
        {
            GenerateCaptcha2(); // Generate and store the CAPTCHA code
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string captcha)
        {
            // Retrieve the CAPTCHA code from TempData
            string storedCaptcha = TempData["CaptchaCode"] as string;

            if (captcha != storedCaptcha)
            {
                ViewBag.Message = "CAPTCHA verification failed.";
                GenerateCaptcha2(); // Regenerate CAPTCHA on failure
                return View();
            }

            if (userDAL.IsUsernameExists(user.Username))
            {
                ViewBag.Message = "Username already exists.";
                GenerateCaptcha2(); // Regenerate CAPTCHA for the next request
                return View();
            }

            if (userDAL.IsEmailExists(user.Email))
            {
                ViewBag.Message = "Email already exists.";
                GenerateCaptcha2(); // Regenerate CAPTCHA for the next request
                return View();
            }

            if (ModelState.IsValid)
            {
                if (userDAL.RegisterUser(user))
                {
                    ViewBag.Message = "Registration successful!";
                }
                else
                {
                    ViewBag.Message = "Error during registration.";
                }
            }

            GenerateCaptcha2(); // Regenerate CAPTCHA for the next request
            return View();
        }

        [HttpGet]
        public IActionResult Captcha()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate"; // HTTP 1.1
            Response.Headers["Pragma"] = "no-cache"; // HTTP 1.0
            Response.Headers["Expires"] = "0"; // Proxies

            return File(GetCaptchaImage(), "image/png");
        }
        private void GenerateCaptcha2()
        {
            captchaCode = GenerateRandomText(6); // Generate random 6 characters
            TempData["CaptchaCode"] = captchaCode; // Store the CAPTCHA code securely
        }

        private byte[] GetCaptchaImage()
        {
            using var bitmap = new Bitmap(100, 40);
            using var graphics = Graphics.FromImage(bitmap);
            using var font = new Font("Consolas", 18, FontStyle.Bold | FontStyle.Italic);
            graphics.Clear(Color.Azure);

            var random = new Random();
            graphics.DrawString(captchaCode, font, Brushes.Black, 10, 10);

            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }

        private string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
