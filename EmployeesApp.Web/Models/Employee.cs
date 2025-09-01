using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace EmployeesApp.Web.Models
{

    public class Employee
    {
        [JsonInclude]
        public int Id { get; internal set; }

        //[NoDigits(ErrorMessage = "Namnet får ej innehålla siffror!")]
        [Required(ErrorMessage = "Du måste ange namn!")]
        [Display(Name = "Förnamn", Prompt = "Ange förnamn.")]
        public required string FirstName { get; set; }

        //[NoDigits(ErrorMessage = "Namnet får ej innehålla siffror!")]
        [Required(ErrorMessage = "Du måste ange namn!")]
        [Display(Name = "Efternamn", Prompt = "Ange efternamn.")]
        public required string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required(ErrorMessage = "Du måste ange email!")]
        [EmailAddress(ErrorMessage = "Måste ange giltig emailadress!")]
        [Display(Name = "Email", Prompt = "Ange emailadress.")]
        public required string Email { get; set; }

        public bool ClockedIn { get; set; } = false;

        public Dictionary<DateOnly, List<WorkShift>> WorkShifts = new Dictionary<DateOnly, List<WorkShift>>();

        //public Employee(int id, string firstname, string lastname, string email)
        //{
        //    Id = id;
        //    FirstName = firstname;
        //    LastName = lastname;
        //}


    }
}
