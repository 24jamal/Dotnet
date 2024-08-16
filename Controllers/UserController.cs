using INTERNMvc.DAL;
using INTERNMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace INTERNMvc.Controllers
{
    public class UserController : Controller
    {
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
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
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
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = userDAL.ValidateUser(username, password);
            if (user != null)
            {

                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                ViewBag.Message = "Invalid username or password.";
                return View();
            }
        }


            private readonly string _connectionString = "Data Source=10.250.4.92,2021;Initial Catalog=Internship;Persist Security Info=True;User ID=Intern;Password=2^pYR9_|z41d;TrustServerCertificate=True";

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
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


    }
}