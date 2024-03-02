using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManagement.Controllers
{
    [Authorize(Roles ="Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public IActionResult Dashboard()
        {
            var items = _dbContext.Item.ToList();
            return View(items);
        }
    }
}
