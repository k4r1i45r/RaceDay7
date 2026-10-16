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
    [Authorize] // User must be logged in
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/enrollments (Participant enters an event)
        [HttpPost]
        public async Task<IActionResult> Enroll([FromBody] Enrollment enrollment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Find the Participant record linked to this User
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant == null) return BadRequest("You must be a registered Participant to enroll.");

            // Check if already enrolled
            var existing = await _context.Enrollments.FirstOrDefaultAsync(e => e.FK_ParticipantID == participant.ParticipantID && e.FK_EventID == enrollment.FK_EventID);
            if (existing != null) return BadRequest("You are already enrolled in this event.");

            enrollment.FK_ParticipantID = participant.ParticipantID;
            enrollment.EnrollmentDate = DateTime.Now;
            enrollment.Status = "Enrolled";

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Successfully enrolled!", EnrollmentId = enrollment.EnrollmentID });
        }

        // GET: api/enrollments/event/5 (Organiser views all enrollments for their event)
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEventEnrollments(int eventId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role?.Name != "Organiser") return Forbid("Only Organisers can view event enrollments.");

            var enrollments = await _context.Enrollments
                .Where(e => e.FK_EventID == eventId)
                .Include(e => e.Participant)
                .ToListAsync();

            return Ok(enrollments);
        }
    }
}