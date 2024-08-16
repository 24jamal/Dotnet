using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace INTERNMvc.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

    }
}
