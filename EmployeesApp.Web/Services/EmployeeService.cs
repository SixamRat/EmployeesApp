using EmployeesApp.Web.Models;

namespace EmployeesApp.Web.Services
{
    public class EmployeeService
    {
        List<Employee> _employees = new List<Employee> {
        new Employee{Id = 1, FirstName = "Jonas", LastName = "Antonsson", Email = "Jonas@testmail.com"},
        new Employee{Id = 2, FirstName = "Bert", LastName = "Johansson", Email = "Bert@testmail.com"},
        new Employee{Id = 3, FirstName = "Andreas", LastName = "Henriksson", Email = "Andreas@testmail.com"}
        };

        public (bool success, string? errorMsg) RegisterNew(Employee employee)
        {
            bool newUserRegistered = false;
            string? errorMsg = null;
            employee.Id = _employees.Count > 0 ? _employees.Max(p => p.Id) + 1 : 1;

            bool fullNameIsUnique = true;

            if (_employees.Count > 0)
            {
                fullNameIsUnique = _employees.Any(e => e.FullName != employee.FullName);

            }
            if (!fullNameIsUnique)
            {
                errorMsg = "User already exists";
                return (newUserRegistered, errorMsg);
            }



            _employees.Add(employee);


        }

        public Employee[] GetAll()
        {
            return _employees.ToArray();
        }

        public Employee GetById(int id)
        {
            Employee employee = _employees.Single(e => e.Id == id);
            return employee;
        }

        public void ToggleShiftClock(int employeeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today); // Dagens datum i DateOnly
            var timeNow = TimeOnly.FromDateTime(DateTime.Now); // Tiden nu i TimeOnly

            var employee = GetById(employeeId); // Hämta employee utifrån ID

            employee.WorkShifts ??= new(); // Är Dictionaryn null, skapa ny.

            var list = GetOrCreateList(employee.WorkShifts, today); // Egen privat metod

            if (list.Count == 0) // Om listan är tom, skapa ett nytt skift/arbetspass med ID 1
            {
                list.Add(new WorkShift()
                {
                    Id = 1
                });
            }

            if (employee.ClockedIn) // Om employee är instämplad
            {

                // Försök hitta ett öppet skift(endast starttid)
                var openShift = list 
                    .FirstOrDefault(ws => !ws.ShiftEndTime.HasValue); 

                // Om den hittar öppet skift, sätt sluttid till timeNow och clocka ut
                if (openShift is not null) 
                {
                    openShift.ShiftEndTime = timeNow;
                    employee.ClockedIn = false;
                }
                return;

            }

            if (!employee.ClockedIn) // Om employee ej är instämplad
            {

                // Om alla skift är stängda(har sluttid),
                // - skapa nytt skift och sätt starttid till timeNow
                if (list.All(ws => ws.ShiftEndTime.HasValue))
                {
                    list.Add(new WorkShift()
                    {
                        Id = list.Max(ws => ws.Id) + 1,
                        ShiftStartTime = timeNow
                    });
                    employee.ClockedIn = true;
                    return;
                }
                else // Om ett skift är öppet men ej har starttid (dvs endast första ggn),
                    //   - sätt starttid till timeNow och stämpla in.
                {
                    var currentShift = list
                        .FirstOrDefault(ws => !ws.ShiftStartTime.HasValue);

                    if (currentShift != null)
                    {
                        currentShift.ShiftStartTime = timeNow;
                        employee.ClockedIn = true;
                        return;
                    }
                }
            }
        }
        
        private static List<WorkShift> GetOrCreateList(Dictionary<DateOnly, List<WorkShift>> dict, DateOnly day)
        {
            if (!dict.TryGetValue(day, out var list))
                dict[day] = list = new List<WorkShift>();
            return list;
        }

    }
}
