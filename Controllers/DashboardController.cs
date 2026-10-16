// POST: Update a user's role
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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