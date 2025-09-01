using EmployeesApp.Web.Models;
using EmployeesApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesApp.Web.Controllers;
public class EmployeesController : Controller
{
    static EmployeeService employeeService = new EmployeeService();

    [Route("")]
    public IActionResult Index()
    {
        Employee[] model = employeeService.GetAll();
        return View(model);
    }

    [HttpGet("/employee/{id}")]
    public IActionResult Details(int id)
    {
        var model = employeeService.GetById(id);
        return View(model);
    }

    [HttpGet("/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("/create")]
    public IActionResult Create(Employee employee)
    {

        if (!ModelState.IsValid)
            return View();

        var (success, errorMessage) = employeeService.RegisterNew(employee);
        if (!success)
        {
            ModelState.AddModelError("CreateEmployee", errorMessage ?? "Error adding new user");
            return View();
        }

        return RedirectToAction(nameof(Index));

    }


    [HttpPost("toggle-clock/{id}")]
    public IActionResult ToggleShiftClock(int id)
    {

        employeeService.TogglePunchClock(id);

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction(nameof(Index));


    }

}
