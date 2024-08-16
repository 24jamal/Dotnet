using INTERNMvc.DAL;
using INTERNMvc.Models;
using INTERNMvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace INTERNMvc.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly EmailService _emailService;

        public ForgotPasswordController()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=test;Integrated Security=True;TrustServerCertificate=True;";
            _userRepository = new UserRepository(connectionString);
            _emailService = new EmailService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username)
        {
            try
            {
                var (email, password) = _userRepository.GetUserCredentials(username);
                _emailService.SendForgotPasswordEmail(email, username, password);
                ViewBag.Message = "Forgot password email sent successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return View();

        }
    }
}
