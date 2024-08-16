using System.Net.Mail;
using System.Net;

namespace INTERNMvc.Services
{
    public class EmailService
    {
        public void SendForgotPasswordEmail(string toEmail, string username, string password)
        {
            var fromEmail = "jamal.mylapay@gmail.com";
            var fromPassword = "bszicxoiypcuzbwc";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Forgot Password",
                Body = $"Hello {username},\n\nYour password is: {password}\n\nRegards,\nMylapay LLC, India.",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(toEmail);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
}
    }
