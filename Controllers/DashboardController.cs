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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(string userId, string roleId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.RoleId = roleId;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}