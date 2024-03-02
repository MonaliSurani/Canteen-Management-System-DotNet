using Azure;
using CanteenManagement.Models;
using CanteenManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CanteenManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager= signInManager;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register registerModel)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(registerModel.Email);

                if (userExist != null)
                {
                    return View();
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
                    return View();
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

                if (await _roleManager.RoleExistsAsync(UserRole.Employee))
                {
                    await _userManager.AddToRoleAsync(user, UserRole.Employee);
                }

                //if (await _roleManager.RoleExistsAsync(UserRole.Admin))
                //{
                //    await _userManager.AddToRoleAsync(user, UserRole.Admin);
                //}

                //if (await _roleManager.RoleExistsAsync(UserRole.Owner))
                //{
                //    await _userManager.AddToRoleAsync(user, UserRole.Owner);
                //}


               return RedirectToAction("Login","Account");
            }
            else
            {
                return View();
            } 
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginModel.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                    var userRole = await _userManager.GetRolesAsync(user);
                    //HttpContext.Session.SetString("UserRole", userRole[0]);
                    await _signInManager.SignInAsync(user, false);
                    if (userRole.Any(x=>x.Contains("Admin")))
                    {
                        return RedirectToAction("Dashboard","Admin");
                    }
                    else if(userRole.Any(x => x.Contains("Owner")))
                    {
                        return RedirectToAction("Dashboard", "Owner");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Employee");
                    }
                }
                return View();
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }
    }
}
