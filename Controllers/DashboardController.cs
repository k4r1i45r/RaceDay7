using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay7.Models;
using Microsoft.AspNetCore.Identity;
using RaceDay7.Data;
using Microsoft.EntityFrameworkCore;

namespace RaceDay7.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Login");

            string roleName = "Participant";
            if (!string.IsNullOrEmpty(user.RoleId))
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
                if (role != null) roleName = role.Name;
            }

            if (roleName == "Admin")
            {
                // Pass all users to the Admin Dashboard
                var allUsers = await _context.Users.ToListAsync();
                var allRoles = await _context.Roles.ToListAsync();
                ViewBag.Roles = allRoles;
                return View("AdminDashboard", allUsers);
            }
            else if (roleName == "Organiser")
            {
                return View("OrganiserDashboard");
            }
            else
            {
                return View("ParticipantDashboard");
            }
        }

        // POST: Update a user's role
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string roleId)
        {
            // Manually check if the current user is an Admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "Login");

            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (currentUser.RoleId != adminRole?.Id)
            {
                return RedirectToAction("Index"); // Non-admins get redirected
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.RoleId = roleId;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin viewing the Organiser dashboard
        [Authorize]
        public async Task<IActionResult> OrganiserView()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (currentUser?.RoleId != adminRole?.Id)
            {
                return RedirectToAction("Index"); // Only admins can access this
            }
            return View("OrganiserDashboard");
        }
    }
}