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
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

           // var newUserId = user.Id;

            return RedirectToAction("index");

        }
        [Route("users/{id:int}")]
        public IActionResult Show(int id)
        {
            var user = _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.Messages)
                .First(); 
            return View(user);
        }
        [HttpPost]
        public IActionResult Message(int id, string content)
        {
            var user = _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.Messages)
                .First();
            var message = new Message(content);
            user.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Show", new {id = id});
        }
    }
}
