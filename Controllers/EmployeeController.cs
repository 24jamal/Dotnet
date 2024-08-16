using INTERNMvc.DAL;
using INTERNMvc.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System.Text;


namespace INTERNMvc.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly Employee_DAL _dal;

        public EmployeeController(Employee_DAL dal)
        {
            _dal = dal;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                employees = _dal.GetAll();
            }

            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }


        [HttpPost]
        public IActionResult Create(Employee model)
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Employee employee = _dal.GetById(id);
                if (employee.Id == 0)
                {
                    TempData["errorMessage"] = $"Employee details not found with Id  {id}";
                    return RedirectToAction("Index");

                }

                return View(employee);
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult Edit(Employee model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Model data is Invalid";
                    return View();
                }
                bool result = _dal.Update(model);

                if (!result)
                {
                    TempData["errorMessage"] = "Unable to update the data";
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








        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                Employee employee = _dal.GetById(id);
                if (employee.Id == 0)
                {
                    TempData["errorMessage"] = $"Employee details not found with Id  {id}";
                    return RedirectToAction("Index");

                }

                return View(employee);
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(Employee model)
        {
            try
            {


                bool result = _dal.Delete(model.Id);

                if (!result)
                {
                    TempData["errorMessage"] = "Unable to delete the data";
                    return View();
                }
                TempData["successMessage"] = "Employee details deleted";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }


        public ActionResult Index(int? page)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                employees = _dal.GetAll(); // Retrieve all employees
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            int pageSize = 10; // Number of records per page
            int pageNumber = (page ?? 1); // Default to the first page if not provided

            // Paginate the list
            var pagedEmployees = employees.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Set pagination details for the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(employees.Count / (double)pageSize);

            return View(pagedEmployees);
        }

        public ActionResult GetEmployees(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            List<Employee> employees = _dal.GetAll();

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

        [HttpGet]
        public IActionResult ExportToCsv()
        {
            try
            {
                List<Employee> employees = _dal.GetAll();
                StringBuilder csvContent = new StringBuilder();

                // Add CSV headers
                csvContent.AppendLine("ID,FullName,DateOfBirth,Email,Salary");

                // Add employee data
                foreach (var employee in employees)
                {
                    csvContent.AppendLine($"{employee.Id},{employee.FullName},{employee.DateofBirth.ToString("yyyy-MM-dd")},{employee.Email},{employee.Salary}");
                }

                byte[] csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(csvBytes, "text/csv", "Employees.csv");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public IActionResult AdvancedAnalytics()
        {
            // Get all employees
            List<Employee> employees = _dal.GetAll();

            // Calculate age for each employee
            var ageGroups = employees
                .Select(e => DateTime.Now.Year - e.DateofBirth.Year)
                .GroupBy(age => age)
                .Select(group => new { Age = group.Key, Count = group.Count() })
                .ToList();


            // Pass the age groups to the view
            ViewBag.AgeGroups = ageGroups;


            return View();
        }


    }
}
