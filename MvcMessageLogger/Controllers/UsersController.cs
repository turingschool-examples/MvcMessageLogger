using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMessageLogger.DataAccess;
using MvcMessageLogger.Models;

namespace MvcMessageLogger.Controllers
{
    public class UsersController : Controller
    {
        private readonly MvcMessageLoggerContext _context;

        public UsersController(MvcMessageLoggerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Details(int id)
        {
            var u = _context.Users.Include(e => e.Messages).Where(e => e.Id == id).Single();
            return View(u);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]//CREATE
        public IActionResult Index(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            return Redirect("/users");//needs to change to Details after the action is added.
        }
    }
}
