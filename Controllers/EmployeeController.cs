using INTERNMVCNew.DAL;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Linq;
using INTERNMvc.DAL;
using INTERNMvc.Models;

namespace INTERNMVCNew.Controllers
{


    public class EmployeeController : Controller
    {

        private readonly Employee_DAL _dal;



        public EmployeeController(Employee_DAL dal)
        {
            _dal = dal;
        }




        [HttpGet]
        public IActionResult SingleView(string searchTerm = "", int page = 1, string sortColumn = "Id", string sortDirection = "asc")
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "User"); // Redirect to login if session is not set
            }

            var employees = _dal.GetAll();

            // Search functionality
            if (!string.IsNullOrEmpty(searchTerm))
            {
                employees = employees
                    .Where(e => e.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sorting
            switch (sortColumn)
            {
                case "Id":
                    employees = sortDirection == "asc" ? employees.OrderBy(e => e.Id).ToList() : employees.OrderByDescending(e => e.Id).ToList();
                    break;
                case "FullName":
                    employees = sortDirection == "asc" ? employees.OrderBy(e => e.FullName).ToList() : employees.OrderByDescending(e => e.FullName).ToList();
                    break;
                case "DateofBirth":
                    employees = sortDirection == "asc" ? employees.OrderBy(e => e.DateofBirth).ToList() : employees.OrderByDescending(e => e.DateofBirth).ToList();
                    break;
                case "Email":
                    employees = sortDirection == "asc" ? employees.OrderBy(e => e.Email).ToList() : employees.OrderByDescending(e => e.Email).ToList();
                    break;
                case "Salary":
                    employees = sortDirection == "asc" ? employees.OrderBy(e => e.Salary).ToList() : employees.OrderByDescending(e => e.Salary).ToList();
                    break;
                default:
                    employees = employees.OrderBy(e => e.Id).ToList();
                    break;
            }

            // Pagination
            int pageSize = 10;
            var pagedEmployees = employees.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = (int)Math.Ceiling(employees.Count / (double)pageSize);
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.SearchTerm = searchTerm;

            return View(pagedEmployees);
        }
        [HttpGet]
        public JsonResult GetEmployees(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            var employees = _dal.GetAll();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                employees = employees
                    .Where(e => e.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var pagedEmployees = employees.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new
            {
                data = pagedEmployees,
                totalRecords = employees.Count
            });
        }

        [HttpPost]
        public JsonResult DeleteEmployeeById(int id)
        {
            var result = _dal.Delete(id);
            return Json(new { success = result });
        }

        [HttpGet]
        public JsonResult GetEmployeeById(int id)
        {
            var employee = _dal.GetById(id);
            return Json(employee);
        }

        [HttpPost]
        public JsonResult UpdateEmployee(Employee employee)
        {
            var result = _dal.Update(employee);
            return Json(new { success = result });
        }



        [HttpGet]
        public IActionResult ExportToCsv()
        {
            var employees = _dal.GetAll();

            var csv = new StringBuilder();
            csv.AppendLine("Id,FullName,DateofBirth,Email,Salary");

            foreach (var employee in employees)
            {
                csv.AppendLine($"{employee.Id},{employee.FullName},{employee.DateofBirth:yyyy-MM-dd},{employee.Email},{employee.Salary}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());
            var result = new FileContentResult(csvBytes, "text/csv")
            {
                FileDownloadName = "employees.csv"
            };

            return result;
        }

        [HttpPost]
        public IActionResult Create(Employee model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Model data is invalid";

                }
                bool result = _dal.Insert(model);

                if (!result)
                {
                    TempData["errorMessage"] = "Unable to save the data";
                    return View();
                }
                TempData["successMessage"] = "Employee details saved";

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        // Analytics Dashboard Controls
        [HttpGet]
        public JsonResult GetAnalyticsData()
        {
            var employees = _dal.GetAll();

            // Calculate stats
            var salaryData = employees.Select(e => e.Salary).ToList();
            var ageData = employees.Select(e => (DateTime.Now.Year - e.DateofBirth.Year)).ToList();

            var averageSalary = salaryData.Average();
            var minSalary = salaryData.Min();
            var maxSalary = salaryData.Max();
            var minAge = ageData.Min();
            var maxAge = ageData.Max();

            // Build response object for analytics
            var analyticsData = new
            {
                salaryStats = new
                {
                    AverageSalary = averageSalary,
                    MinSalary = minSalary,
                    MaxSalary = maxSalary
                },
                ageStats = new
                {
                    MinAge = minAge,
                    MaxAge = maxAge
                },
                salaryDistribution = salaryData,
                ageDistribution = ageData
            };

            return Json(analyticsData);
        }

    }
}