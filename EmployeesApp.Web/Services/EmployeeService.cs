using EmployeesApp.Web.Models;

namespace EmployeesApp.Web.Services
{
    public class EmployeeService
    {
        static List<Employee> _employees = new List<Employee> {
        new Employee{Id = 1, Name = "Jonas", Email = "Jonas@testmail.com"},
        new Employee{Id = 2, Name = "Bert", Email = "Bert@testmail.com"},
        new Employee{Id = 3, Name = "Andreas", Email = "Andreas@testmail.com"}
        };

        public void RegisterNew(Employee employee)
        {
            employee.Id = _employees.Count > 0 ? _employees.Max(p => p.Id) + 1 : 1;

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

            var today = DateOnly.FromDateTime(DateTime.Today);
            var timeNow = TimeOnly.FromDateTime(DateTime.Now);

            var e = GetById(employeeId);

            e.WorkShifts ??= new(); // Är Dictionaryn null, skapa ny.

            var list = GetOrCreateList(e.WorkShifts, today);

            if (list.Count == 0)
            {
                list.Add(new WorkShift()
                {
                    Id = 1
                });
            }

            if (e.ClockedIn)
            {

                var currentWorkShift = list
                    .FirstOrDefault(ws => !ws.ShiftEndTime.HasValue);

                if (currentWorkShift != null)
                {
                    currentWorkShift.ShiftEndTime = timeNow;
                    e.ClockedIn = false;
                    return;
                }

            }
            if (!e.ClockedIn)
            {

                if (list.All(ws => ws.ShiftEndTime.HasValue))
                {
                    list.Add(new WorkShift()
                    {
                        Id = list.Max(ws => ws.Id) + 1,
                        ShiftStartTime = timeNow
                    });
                    e.ClockedIn = true;
                    return;
                }
                else
                {
                    var currentShift = list
                        .FirstOrDefault(ws => !ws.ShiftStartTime.HasValue);

                    if (currentShift != null)
                    {
                        currentShift.ShiftStartTime = timeNow;
                        e.ClockedIn = true;
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



        //public int ToggleShiftClock(int id)
        //{
        //    DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        //    TimeOnly timeNow = TimeOnly.FromDateTime(DateTime.Now);

        //    Employee employee = GetById(id);

        //    employee.workTimes ??= new(); // Skapa ny Dictionary för workTimes om den inte redan finns

        //    if (!employee.workTimes.ContainsKey(today)) // Om datumet inte finns, skapa Key + new WorkTime-list
        //    {
        //        employee.workTimes.Add(today, new List<WorkTime>());
        //    }

        //    employee.workTimes[today] ??= new();

        //    List<WorkTime> workTimes = employee.workTimes[today];

        //    int workShifts = workTimes.Count;

        //    if (workShifts == 0)
        //    {
        //        workTimes.Add(new WorkTime()
        //        {
        //            Id = 1
        //        });
        //    }

        //    if (workTimes.All(wt => wt.ShiftStartTime.HasValue && wt.ShiftEndTime.HasValue))
        //    {
        //        workTimes.Add(new WorkTime()
        //        {
        //            Id = workTimes.Max(ws => ws.Id) + 1
        //        });
        //    }

        //    int currentWorkTimeId = 0;

        //    foreach (WorkTime workTime in workTimes)
        //    {

        //        if (!employee.ClockedIn && !workTime.ShiftStartTime.HasValue)
        //        {
        //            workTime.ShiftStartTime = timeNow;
        //            employee.ClockedIn = true;
        //            currentWorkTimeId = workTime.Id;
        //            break;
        //        }

        //        if (employee.ClockedIn && workTime.ShiftStartTime.HasValue && !workTime.ShiftEndTime.HasValue)
        //        {
        //            workTime.ShiftEndTime = timeNow;
        //            employee.ClockedIn = false;
        //            currentWorkTimeId = workTime.Id;
        //            break;

        //        }

        //    }
        //    return currentWorkTimeId;


        //}

        //public void ClockOut(DateTime endTime, int id)
        //{
        //    var employee = GetById(id);

        //    if (employee.ClockedIn) employee.ClockedIn = false;

        //    employee.workTimes[endTime.Date].ShiftEndTime = TimeOnly.FromTimeSpan(endTime.TimeOfDay);

        //    //return employee.ClockedIn;

        //}

    }
}
