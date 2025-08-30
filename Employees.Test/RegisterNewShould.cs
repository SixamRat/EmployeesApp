using EmployeesApp.Web.Models;
using EmployeesApp.Web.Services;

namespace Employees.Test
{
    public class RegisterNewShould
    {
        
        
        [Fact]
        public void Not_Exist_In_Initial_Collection()
        {
            EmployeeService sut = new EmployeeService();

            Employee newEmployee = new Employee() { Name = "Rasmus", Email = "rasmus-lackberg@hotmail.com" };

            var allEmployeesBefore = sut.GetAll();

            Assert.DoesNotContain(allEmployeesBefore, e => e.Name == newEmployee.Name && e.Email == newEmployee.Email);
        }

        [Fact]
        public void Add_New_Employee_To_Collection()
        {
            EmployeeService sut = new EmployeeService();

            Employee newEmployee = new Employee() { Name = "Rasmus", Email = "rasmus-lackberg@hotmail.com" };

            var allEmployeesBefore = sut.GetAll();

            sut.RegisterNew(newEmployee);

            var allEmployeesAfter = sut.GetAll();

            Assert.Contains(allEmployeesAfter, e => e.Name == newEmployee.Name && e.Email == newEmployee.Email);
        }

        [Fact]
        public void Increment_Collection_Length()
        {
            EmployeeService sut = new EmployeeService();

            Employee newEmployee = new Employee() { Name = "Rasmus", Email = "rasmus-lackberg@hotmail.com" };

            var allEmployeesBefore = sut.GetAll();
            int initLength = allEmployeesBefore.Length;

            sut.RegisterNew(newEmployee);

            var allEmployeesAfter = sut.GetAll();

            Assert.Equal(initLength + 1, allEmployeesAfter.Length);
        }

        [Fact]
        public void Generate_Id_For_New_Employee()
        {
            EmployeeService sut = new EmployeeService();

            Employee newEmployee = new Employee() { Name = "Rasmus", Email = "rasmus-lackberg@hotmail.com" };

            sut.RegisterNew(newEmployee);

            Assert.True(newEmployee.Id > 0);
        }

        [Fact]
        public void Generate_Id_Max_PlusOne()
        {
            var sut = new EmployeeService();
            Employee newEmployee = new Employee() { Name = "Rasmus", Email = "rasmus-lackberg@hotmail.com" };

            int maxId = sut.GetAll().Max(e => e.Id);

            sut.RegisterNew(newEmployee);

            Assert.Equal(maxId + 1, newEmployee.Id);
        }

        [Fact]
        public void Have_Distinct_Id()
        {
            EmployeeService sut = new EmployeeService();

            Employee newEmployee1 = new Employee() { Name = "Rasmus", Email = "rasmus-lackberg@hotmail.com" };
            Employee newEmployee2 = new Employee() { Name = "Sven", Email = "sven-lackberg@hotmail.com" };

            sut.RegisterNew(newEmployee1);
            sut.RegisterNew(newEmployee2);

            var allEmployeesAfter = sut.GetAll();
            Assert.Distinct(allEmployeesAfter.Select(e => e.Id));
        }






        //[Fact]
        //public void RegisterNew_Full_Test()
        //{
        //    EmployeeService sut = new EmployeeService();

        //    Employee newEmployee = new Employee() { Name = "Rasmus", Email = "rasmus-lackberg@hotmail.com" };

        //    var allEmployeesBefore = sut.GetAll();

        //    Assert.DoesNotContain(newEmployee, allEmployeesBefore);
        //    Assert.DoesNotContain(allEmployeesBefore, e => e.Name == newEmployee.Name && e.Email == newEmployee.Email);

        //    int initLength = allEmployeesBefore.Length;

        //    sut.RegisterNew(newEmployee);

        //    var allEmployeesAfter = sut.GetAll();

        //    Assert.Equal(initLength + 1, allEmployeesAfter.Length);
        //    Assert.Contains(allEmployeesAfter, e => e.Name == newEmployee.Name && e.Email == newEmployee.Email);
        //    Assert.True(newEmployee.Id > 0);
        //    Assert.Distinct(allEmployeesAfter.Select(e => e.Id));

        //}


        [Fact]
        public void GetAll_Returns_All_Employees()
        {
            EmployeeService sut = new EmployeeService();

            var result = sut.GetAll();

            Assert.NotNull(result);
            Assert.Equal(3, result.Length);


        }
    }
}
