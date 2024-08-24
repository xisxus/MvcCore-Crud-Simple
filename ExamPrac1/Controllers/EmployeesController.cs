
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamPrac1.Models;

namespace ExamPrac1.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;


        

        public EmployeesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var e = await _context.Employees.Include(e => e.Experiences).ToListAsync();
            return View(e);
        }

        public IActionResult AddExperiencePartial()
        {
           
            return PartialView("_ExperiencePartial", new ExperienceViewModel());
        }


        public IActionResult AddExperiencePartial2(int index)
        {
            ViewBag.Index = index;
            return PartialView("_ExperiencePartial2", new ExperienceViewModel());
        }



        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {

            return View(new EmpViewModel());
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpViewModel employee)
        {

            string picUrl = null;
            if (employee.ImageFile != null)
            {
                var imgName = Guid.NewGuid().ToString() + ".jpg";
                var filePath = Path.Combine(_env.WebRootPath, "img", imgName);

                using (var fileStrem= new FileStream(filePath, FileMode.Create))
                {
                    await employee.ImageFile.CopyToAsync(fileStrem);
                }

                picUrl = "/img/" + imgName;
            }

            var emp = new Employee
            {
                Name = employee.Name,
                Active = employee.Active,
                Salary = employee.Salary,
                JoinDate = employee.JoinDate,
                ImageUrl = picUrl
            };
            _context.Employees.Add(emp);
            _context.SaveChanges();

            var emplo = await _context.Employees
                .FirstOrDefaultAsync(m => m.ImageUrl == picUrl);

            foreach (var item in employee.Experiences)
            {
                var exp = new Experience
                {
                    EmployeeId = emplo.EmployeeId,
                    Tittle = item.Tittle,
                    Duration = item.Duration,
                };
                _context.Experiences.Add(exp);
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Experiences)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }


            var model = new EmpViewModel
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Active = employee.Active,
                JoinDate = employee.JoinDate,
                Salary = employee.Salary,
                ImageUrl = employee.ImageUrl,
                Experiences = employee.Experiences.Select(exp => new ExperienceViewModel
                {
                     
                    Tittle = exp.Tittle,
                    Duration = exp.Duration
                }).ToList()
            };

            return View(model);


            
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpViewModel employee)
        {
            //if (id != employee.EmployeeId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(employee);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!EmployeeExists(employee.EmployeeId))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}
            return View();
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
