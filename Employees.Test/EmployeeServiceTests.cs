using System.Reflection.Metadata;
using System.Text.Json;
using EmployeesApp.Web.Models;
using EmployeesApp.Web.Services;
using Xunit.Abstractions;

namespace Employees.Test
{
    public class EmployeeServiceTests : IDisposable
    {

        private readonly EmployeeService _sut;
        private readonly ITestOutputHelper _output;
        private string _dirPath;
        private string _fileName = "EmployeeList.json";
        private string _filePath;
        private JsonSerializerOptions _json = new() { WriteIndented = true };


        public EmployeeServiceTests(ITestOutputHelper output)
        {
            _output = output;

            // Skapa en unik testmapp varje gång
            _dirPath = Path.Combine(Path.GetTempPath(), "emp-tests");

            if (Directory.Exists(_dirPath))
                Directory.Delete(_dirPath, recursive: true);

            Directory.CreateDirectory(_dirPath);

            _filePath = Path.Combine(_dirPath, _fileName);

            _sut = new EmployeeService(_dirPath);
        }


        [Fact]
        [Trait("Category", "EmployeeService_Constructor")]
        public void EmployeeService_Should_GetOrSet_EmployeeList()
        {
            _output.WriteLine("Retrieving list..");
            var list = _sut.GetAll();

            Assert.NotNull(list);
            Assert.NotEmpty(list);

            _output.WriteLine("List retrieval succeeded");
        }


        [Fact]
        [Trait("Category", "EmployeeService_RegisterNew")]
        public void RegisterNew_Should_Add_New_Employee_To_Collection()
        {

            Employee newEmployee = new()
            {
                FirstName = "Rasmus",
                LastName = "Svensson",
                Email = "t1@test.com"
            };

            var allBefore = _sut.GetAll().Length;

            var (success, errorMessage) = _sut.RegisterNew(newEmployee);

            var allAfter = _sut.GetAll();

            Assert.True(success);
            Assert.Null(errorMessage);
            Assert.Equal(allBefore + 1, allAfter.Length);

            Assert.Contains(allAfter, e =>
                e.FirstName == newEmployee.FirstName &&
                e.LastName == newEmployee.LastName &&
                e.Email == newEmployee.Email);
        }
        [Fact]
        [Trait("Category", "EmployeeService_RegisterNew")]
        public void RegisterNew_Should_Generate_Id_For_New_Employee()
        {

            Employee newEmployee = new Employee() { FirstName = "Rasmus", LastName = "Svensson", Email = "t3@x.com" };

            _sut.RegisterNew(newEmployee);

            Assert.True(newEmployee.Id > 0);
        }

        [Fact]
        [Trait("Category", "EmployeeService_RegisterNew")]
        public void RegisterNew_Should_Generate_Id_Max_PlusOne()
        {
            Employee newEmployee = new Employee() { FirstName = "Rasmus", LastName = "Svensson", Email = "t4@x.com" };


            int maxId = _sut.GetAll().Max(e => e.Id);

            _sut.RegisterNew(newEmployee);

            Assert.Equal(maxId + 1, newEmployee.Id);
        }

        [Fact]
        [Trait("Category", "EmployeeService_RegisterNew")]
        public void RegisterNew_Should_Generate_Distinct_Id()
        {

            Employee newEmployee1 = new Employee() { FirstName = "Rasmus", LastName = "Svensson", Email = "t5@x.com" };
            Employee newEmployee2 = new Employee() { FirstName = "Sven", LastName = "Jansson", Email = "sven-jansson@hotmail.com" };

            string regNewU = "Registering new user";

            _output.WriteLine(regNewU);
            var (success1, errorMessage1) = _sut.RegisterNew(newEmployee1);
            _output.WriteLine("newEmployee1 registration ");
            _output.WriteLine((success1 ? "succeeded" : "failed: " + "\n") + (errorMessage1));
            Assert.True(success1);

            _output.WriteLine(regNewU);
            var (success2, errorMessage2) = _sut.RegisterNew(newEmployee2);
            _output.WriteLine("newEmployee2 registration ");
            _output.WriteLine((success2 ? "succeeded" : "failed: " + "\n") + (errorMessage2));
            Assert.True(success2);

            var allAfter = _sut.GetAll();
            Assert.Distinct(allAfter.Select(e => e.Id));
        }

        [Fact]
        [Trait("Category", "EmployeeService_RegisterNew")]
        public void RegisterNew_Should_Check_Existing_Email()
        {
            Employee newEmp1 = new()
            {
                FirstName = "Håkan",
                LastName = "Hallgren",
                Email = "samma@email.com"
            };
            Employee newEmp2 = new()
            {
                FirstName = "Linus",
                LastName = "Andersson",
                Email = "samma@email.com"
            };

            string regNewU = "Registering new user";

            _output.WriteLine(regNewU);
            var (success1, errorMessage1) = _sut.RegisterNew(newEmp1);
            _output.WriteLine("newEmp1 registration ");
            _output.WriteLine((success1 ? "succeeded" : "failed: " + "\n") + (errorMessage1));
            Assert.True(success1);
            Assert.Null(errorMessage1);

            _output.WriteLine(regNewU);
            var (success2, errorMessage2) = _sut.RegisterNew(newEmp2);
            _output.WriteLine("newEmp2 registration ");
            _output.WriteLine((success2 ? "succeeded" : "failed: " + "\n") + (errorMessage2));

            Assert.False(success2); // ska vara false/ej lyckas
            Assert.NotNull(errorMessage2); // Ska finnas errormessage 
        }


        [Fact]
        [Trait("Category", "EmployeeService_GetAll")]
        public void GetAll_Should_Return_All_Employees()
        {

            var result = _sut.GetAll();

            Assert.NotNull(result);
            Assert.Equal(3, result.Length); // min seed är 3st därför 3.


        }

        [Fact]
        [Trait("Category", "EmployeeService_GetById")]
        public void GetById_Should_Return_Employee_By_Id()
        {
            Employee newEmp1 = new()
            {
                FirstName = "Markus",
                LastName = "Hallgren",
                Email = "markus@email.com"
            };

            _output.WriteLine("Registering new user");
            var (success, errorMessage) = _sut.RegisterNew(newEmp1);
            _output.WriteLine("newEmp1 registration ");
            _output.WriteLine((success ? "succeeded" : "failed: " + "\n") + (errorMessage));
            
            if (success && errorMessage is null)
            {
                var result = _sut.GetById(newEmp1.Id);

                Assert.NotNull(result);
                Assert.Equal((newEmp1.Id, newEmp1.FullName, newEmp1.Email), (result.Id, result.FullName, result.Email));
                Assert.Same(newEmp1, result);
                
            }

        }

        [Fact]
        [Trait("Category", "EmployeeService_TogglePunchClock")]
        public void TogglePunchClock_FirstTime_Should_CreateShift_And_ClockIn()
        {
            Employee newEmp = new Employee { FirstName = "Olof", LastName = "Hermansson", Email = "oh@testmail.com" };

            var (success, errorMsg) = _sut.RegisterNew(newEmp);

            Assert.True(success);
            Assert.Null(errorMsg);

            _sut.TogglePunchClock(newEmp.Id);

            Assert.NotNull(newEmp.WorkShifts);

            var today = (DateOnly.FromDateTime(DateTime.Today));
            Assert.True(newEmp.WorkShifts.ContainsKey(today));

            Assert.NotNull(newEmp.WorkShifts[today]);

            var list = newEmp.WorkShifts[today];

            var firstShift = list[0];

            Assert.True(firstShift.ShiftStartTime.HasValue);
            Assert.Null(firstShift.ShiftEndTime);
            Assert.True(newEmp.ClockedIn);
        }

        [Fact]
        [Trait("Category", "EmployeeService_TogglePunchClock")]
        public void TogglePunchClock_WhenClockedIn_Should_SetEndTime_And_ClockOut()
        {
            Employee newEmp = new Employee { FirstName = "Olof", LastName = "Hermansson", Email = "oh@testmail.com" };

        }

        public void Dispose()
        {
            // Rensa bort katalog efter varje test
            if (Directory.Exists(_dirPath))
                Directory.Delete(_dirPath, recursive: true);
        }




        //[Fact]
        //[Trait("Category", "EmployeeService_Constructor")]
        //public void EmployeeService_Should_GetOrSet_json_Dir()
        //{
        //    _output.WriteLine("Checking for directory");

        //    Assert.True(Directory.Exists(_dirPath));

        //    _output.WriteLine("Directory found");



        //}

        //[Fact]
        //[Trait("Category", "EmployeeService_Constructor")]
        //public void EmployeeService_Should_GetOrSet_json_File()
        //{
        //    _output.WriteLine("Checking for json file");

        //    Assert.True(File.Exists(_filePath));

        //    _output.WriteLine("File found");


        //}

    }
}
