using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace INTERNMvc.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }

        [DisplayName("Date of Birth")]
        [Required]
        public DateTime DateofBirth { get; set; }


        [Required]
        public string Email { get; set; }

        [Required]
        public double Salary { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }


        }
    }
}
