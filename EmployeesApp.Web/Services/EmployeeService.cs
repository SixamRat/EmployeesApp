using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using EmployeesApp.Web.Models;

namespace EmployeesApp.Web.Services
{
    public class EmployeeService
    {
        private string _dirPath;
        private string _fileName = "EmployeeList.json";
        private string _filePath;
        private JsonSerializerOptions _json = new() { WriteIndented = true };

        private List<Employee> _employees;

        private int _nextId;

        public EmployeeService(string? baseDir = null)
        {
            _dirPath = Path.Combine(baseDir ?? AppContext.BaseDirectory, "json");
            _filePath = Path.Combine(_dirPath, _fileName);

            Directory.CreateDirectory(_dirPath);

            _employees = LoadFromFile() ?? Seed();

            _nextId = _employees.Any() ? _employees.Max(e => e.Id) + 1 : 1;

            SaveToFile();
        }

        private List<Employee>? LoadFromFile()
        {
            if (!File.Exists(_filePath)) return null;

            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Employee>>(json) ?? new();

            }
            catch (JsonException)
            {
                return null;

            }
        }

        // Metod med hårdkodade Seed-värden ifall filen inte kan laddas
        private List<Employee> Seed()
        {
            return new List<Employee>
            {
                new() { Id = 1, FirstName = "Jonas",   LastName = "Antonsson", Email = "Jonas@testmail.com" },
                new() { Id = 2, FirstName = "Bert",    LastName = "Johansson", Email = "Bert@testmail.com" },
                new() { Id = 3, FirstName = "Andreas", LastName = "Henriksson", Email = "Andreas@testmail.com" },
            };
        }

        private void SaveToFile()
        {
            Directory.CreateDirectory(_dirPath);

            var tmp = Path.Combine(_dirPath, _fileName + ".tmp");
            var bak = Path.Combine(_dirPath, _fileName + ".bak");

            var json = JsonSerializer.Serialize(_employees, _json);
            File.WriteAllText(tmp, json);

            if (File.Exists(_filePath))
            {
                // Byter ut befintlig fil och gör backup
                File.Replace(tmp, _filePath, bak);
            }
            else
            {
                // Första gången: flytta in tmp-filen som “riktig”
                File.Move(tmp, _filePath);
            }
        }


        public (bool success, string? errorMsg) RegisterNew(Employee employee)
        {
            // Kollar om emailen redan finns, case-insensitive
            bool emailExists = _employees
                .Any(e => string.Equals(e.Email, employee.Email, StringComparison.OrdinalIgnoreCase));

            if (emailExists)
                return (false, "Email already exists"); // avbryt och returnera false och errormessage

            employee.Id = _nextId++; // sätt ID och öka med 1

            _employees.Add(employee); // Lägg till nya employeen
            SaveToFile(); // Spara till json fil

            return (true, null); // returnera success = true och errormessage = null
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

        public void TogglePunchClock(int employeeId)
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
