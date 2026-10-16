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
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Helper to check if current user is Organiser
        private async Task<bool> IsOrganiser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return false;
            var role = await _context.Roles.FindAsync(user.RoleId);
            return role?.Name == "Organiser";
        }

        // GET: api/events (Anyone can view)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();
            return eventItem;
        }

        // POST: api/events (Only Organisers)
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(Event newEvent)
        {
            if (!await IsOrganiser()) return Forbid("Only Organisers can create events.");
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventID }, newEvent);
        }

        // PUT: api/events/5 (Only Organisers)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, Event updatedEvent)
        {
            if (!await IsOrganiser()) return Forbid("Only Organisers can update events.");
            if (id != updatedEvent.EventID) return BadRequest();

            _context.Entry(updatedEvent).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { if (!EventExists(id)) return NotFound(); else throw; }
            return NoContent();
        }

        // DELETE: api/events/5 (Only Organisers)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!await IsOrganiser()) return Forbid("Only Organisers can delete events.");
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return NotFound();

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool EventExists(int id) => _context.Events.Any(e => e.EventID == id);
    }
}
