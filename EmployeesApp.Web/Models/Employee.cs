using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeesApp.Web.Models
{

    public class Employee
    {
        public int Id { get; set; }

        [NoDigits(ErrorMessage = "Namnet får ej innehålla siffror!")]
        [Required(ErrorMessage = "Du måste ange namn!")]
        [Display(Name = "Namn", Prompt = "Ange ditt förnamn.")]
        public required string FirstName { get; set; }

        [NoDigits(ErrorMessage = "Namnet får ej innehålla siffror!")]
        [Required(ErrorMessage = "Du måste ange namn!")]
        [Display(Name = "Namn", Prompt = "Ange ditt förnamn.")]
        public required string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required(ErrorMessage = "Du måste ange email!")]
        [EmailAddress(ErrorMessage = "Måste ange giltig emailadress!")]
        [Display(Name = "Email", Prompt = "Ange din emailadress.")]
        public required string Email { get; set; }

        public bool ClockedIn { get; set; } = false;

        public Dictionary<DateOnly, List<WorkShift>> WorkShifts = new Dictionary<DateOnly, List<WorkShift>>();

        
    }
}
