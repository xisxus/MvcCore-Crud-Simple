using ExamPrac1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ExamPrac1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

       

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {

            var emp = _context.Employees.ToList();
            ViewBag.Min = emp.Min(s => s.Salary);
            ViewBag.Max = emp.Max(s => s.Salary);
            ViewBag.Total = emp.Sum(s => s.Salary);
            ViewBag.Avg = emp.Average(s => s.Salary);
            ViewBag.Count = emp.Count();
            return View(emp);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

