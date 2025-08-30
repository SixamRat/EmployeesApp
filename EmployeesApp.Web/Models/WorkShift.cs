namespace EmployeesApp.Web.Models
{
    public class WorkShift
    {
        public int Id { get; set; }
        //public DateTime Date { get; set; }
        public TimeOnly? ShiftStartTime { get; set; }
        public TimeOnly? ShiftEndTime { get; set; }
        public TimeSpan? WorkShiftTime =>
        (ShiftStartTime, ShiftEndTime) is (TimeOnly s, TimeOnly e) ? e - s : null;
    }
}
