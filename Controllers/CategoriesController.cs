using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay7.Data;
using RaceDay7.Models;

namespace RaceDay7.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<bool> IsOrganiser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return false;
            var role = await _context.Roles.FindAsync(user.RoleId);
            return role?.Name == "Organiser";
        }

        // GET: api/categories (Anyone can view)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // POST: api/categories (Only Organisers)
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Category>> CreateCategory(Category newCategory)
        {
            if (!await IsOrganiser()) return Forbid("Only Organisers can create categories.");
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return Ok(newCategory);
        }

        // PUT: api/categories/5 (Only Organisers)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(int id, Category updatedCategory)
        {
            if (!await IsOrganiser()) return Forbid("Only Organisers can update categories.");
            if (id != updatedCategory.CategoryID) return BadRequest();
            _context.Entry(updatedCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/categories/5 (Only Organisers)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!await IsOrganiser()) return Forbid("Only Organisers can delete categories.");
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}