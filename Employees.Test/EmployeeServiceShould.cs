using EmployeesApp.Web.Models;
using EmployeesApp.Web.Services;

namespace Employees.Test
{
    public class EmployeeServiceShould
    {
        [Fact]
        public void RegisterNewEmployee()
        {
            EmployeeService sut_Service = new EmployeeService();

            Employee sut_Employee = new Employee() { Name = "Rasmus", Email = "rasmus-lackberg@hotmail.com"};

            sut_Service.RegisterNew(sut_Employee);

            Assert.Contains(sut_Employee, sut_Service.GetAll());
        }
    }
}
