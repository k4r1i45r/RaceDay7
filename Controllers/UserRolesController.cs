using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay7.Data;
using RaceDay7.Models;
using RaceDay7.Models.ViewModels;

namespace RaceDay7.Controllers
{
    [Authorize] // User must be logged in
    public class UserRolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserRolesController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: /UserRoles
        public async Task<IActionResult> Index()
        {
            // Verify current user is Admin
            var currentUser = await _userManager.GetUserAsync(User);
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (currentUser?.RoleId != adminRole?.Id) return RedirectToAction("Index", "Dashboard");

            var users = await _context.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var thisViewModel = new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = new List<string>()
                };

                // Fetch role name using our 1-to-1 RoleId
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
                if (role != null)
                {
                    thisViewModel.Roles.Add(role.Name);
                }
                else
                {
                    thisViewModel.Roles.Add("None");
                }

                userRolesViewModel.Add(thisViewModel);
            }

            return View(userRolesViewModel);
        }

        // GET: /UserRoles/Manage?userId=xxx
        public async Task<IActionResult> Manage(string userId)
        {
            // Verify current user is Admin
            var currentUser = await _userManager.GetUserAsync(User);
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (currentUser?.RoleId != adminRole?.Id) return RedirectToAction("Index", "Dashboard");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = await _context.Roles.ToListAsync();
            var model = new List<ManageUserRolesViewModel>();

            foreach (var role in roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    // Check if this is the user's current role (1-to-1)
                    IsSelected = (user.RoleId == role.Id)
                };
                model.Add(userRolesViewModel);
            }

            ViewBag.UserId = userId;
            ViewBag.UserName = user.UserName;
            return View(model);
        }

        // POST: /UserRoles/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Find the selected role from the ViewModel
            var selectedRole = model.FirstOrDefault(r => r.IsSelected);

            if (selectedRole == null)
            {
                ModelState.AddModelError("", "Please select a role.");
                return RedirectToAction("Manage", new { userId = userId });
            }

            // Update the RoleId directly (1-to-1)
            user.RoleId = selectedRole.RoleId;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cannot update user role.");
            return RedirectToAction("Manage", new { userId = userId });
        }
    }
}
