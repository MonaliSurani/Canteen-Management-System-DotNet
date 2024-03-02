using CanteenManagement.Models;
using CanteenManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CanteenManagement.Controllers
{
    //[Authorize(Roles ="Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> Dashboard()
        {
            AdminMainView mainView = new AdminMainView();
            mainView.usersList  =await GetUsersList();
            return View(mainView);
        }

        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            //var adminUsers = _userManager.GetUsersInRoleAsync("Admin").Result;

            // Get all users except for those with the role "Admin"
            //var allUsersExceptAdmin = _dbContext.Users
            //    .Where(u => !adminUsers.Contains(u))
            //    .ToList()

            var usersWithRoles = await _userManager.Users.ToListAsync();
            List<UsersList> usersData =  usersWithRoles.Select(  u => new UsersList()
            {
                Id = u.Id,
                Name = u.UserName,
                Email=u.Email,
                PhoneNumber=u.PhoneNumber,
                Role = _userManager.GetRolesAsync(u).Result[0]
            }).ToList();

            //var result = await Task.WhenAll(usersData);
            return PartialView("_usersList",usersData);
        }

        private async Task<List<UsersList>> GetUsersList()
        {
            var adminUsers = _userManager.GetUsersInRoleAsync("Admin").Result;
            var usersWithRoles = await _userManager.Users.ToListAsync();
            List<UsersList> usersData = usersWithRoles.Where(x=> !adminUsers.Contains(x)).Select(u => new UsersList()
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                PhoneNumber=u.PhoneNumber,
                Role = _userManager.GetRolesAsync(u).Result[0]
            }).ToList();

            return usersData;
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(AddUser registerModel)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(registerModel.Email);

                if (userExist != null)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                ApplicationUser user = new ApplicationUser
                {
                    Email = registerModel.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = registerModel.PhoneNumber,
                    UserName = registerModel.Name
                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);

                if (!result.Succeeded)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                if (!await _roleManager.RoleExistsAsync(UserRole.Employee))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRole.Employee));
                }

                if (!await _roleManager.RoleExistsAsync(UserRole.Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
                }

                if (!await _roleManager.RoleExistsAsync(UserRole.Owner))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRole.Owner));
                }

                if (await _roleManager.RoleExistsAsync(registerModel.Role))
                {
                    await _userManager.AddToRoleAsync(user, registerModel.Role);
                }

                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return RedirectToAction("Dashboard", "Admin");
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction("Dashboard", "Admin");
        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            EditUser usr = new EditUser
            {
                Name = user.UserName,
                Email = user.Email,
                PhoneNumber=user.PhoneNumber
            };
            return PartialView("_UpdateUser", usr);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUser user)
        {
            ApplicationUser usr = await _userManager.FindByIdAsync(user.Id);
            usr.PhoneNumber = user.PhoneNumber;
            usr.UserName = user.Name;
            usr.Email = user.Email;

            var update = await _userManager.UpdateAsync(usr);
            return RedirectToAction("Dashboard", "Admin");
        }
    }
}
