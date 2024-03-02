using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenManagement.Controllers
{
    [Authorize(Roles ="Owner")]
    public class OwnerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
