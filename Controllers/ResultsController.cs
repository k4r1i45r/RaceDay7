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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResultsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/results (Organiser captures result)
        [HttpPost]
        public async Task<IActionResult> CaptureResult([FromBody] Result result)
        {
            var user = await _userManager.GetUserAsync(User);
            var role = await _context.Roles.FindAsync(user?.RoleId);
            if (role?.Name != "Organiser") return Forbid("Only Organisers can capture results.");

            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Result captured successfully!" });
        }

        // GET: api/results/my-results (Participant views their own results)
        [HttpGet("my-results")]
        public async Task<ActionResult<IEnumerable<Result>>> GetMyResults()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant == null) return BadRequest("You are not registered as a Participant.");

            var results = await _context.Results
                .Include(r => r.Enrollment)
                .Where(r => r.Enrollment.FK_ParticipantID == participant.ParticipantID)
                .ToListAsync();

            return Ok(results);
        }
    }
}