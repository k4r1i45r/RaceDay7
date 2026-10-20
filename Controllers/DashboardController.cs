using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay7.Data;
using RaceDay7.Models;
using RaceDay7.Models.ViewModels;
using System.Threading.Tasks;

namespace RaceDay7.Controllers;

[Authorize] // Any logged-in user can access the dashboard routing
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return Challenge();

        // Fetch the role directly from the database using the 1-to-1 RoleId
        string roleName = "Participant"; // Default
        if (!string.IsNullOrEmpty(user.RoleId))
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
            if (role != null) roleName = role.Name;
        }

        // Route based on the exact role name from the database
        if (roleName == "Admin" || roleName == "Organiser")
        {
            return View("Organiser", await BuildOrganiserAsync(user));
        }

        return View("Participant", await BuildParticipantAsync(user));
    }

    // ------------------------------------------------------------------ Organiser / Admin View
    private async Task<RaceDay7.Models.ViewModels.OrganiserDashboardViewModel> BuildOrganiserAsync(ApplicationUser user)
    {
        var now = DateTime.Now;

        var events = await _db.Events
            .AsNoTracking()
            .Where(e => e.OrganiserId == user.Id) // Admins will only see events they created. If you want Admins to see ALL events, change this to .Where(e => true)
            .Select(e => new OrganiserEventRow
            {
                Id = e.Id,
                Name = e.Name,
                Date = e.Date,
                Location = e.Location,
                EventType = e.EventType,
                CategoryCount = e.Categories.Count(),
                EnrolmentCount = e.Enrolments.Count(),
                ResultCount = e.Enrolments.Count(en => en.Result != null)
            })
            .ToListAsync();

        var recent = await _db.Enrolments
            .AsNoTracking()
            .Where(en => en.Event!.OrganiserId == user.Id)
            .OrderByDescending(en => en.EnrolledOn)
            .Take(6)
            .Select(en => new RecentEnrolmentRow
            {
                EventId = en.EventId,
                EventName = en.Event!.Name,
                ParticipantName = en.Participant!.FirstName + " " + en.Participant.LastName,
                CategoryName = en.Category!.Name,
                EnrolledOn = en.EnrolledOn
            })
            .ToListAsync();

        return new OrganiserDashboardViewModel
        {
            DisplayName = user.FirstName ?? "Admin",
            TotalEvents = events.Count,
            UpcomingEvents = events.Count(e => e.Date > now),
            TotalEnrolments = events.Sum(e => e.EnrolmentCount),
            ResultsCaptured = events.Sum(e => e.ResultCount),
            Upcoming = events.Where(e => e.Date > now).OrderBy(e => e.Date).Take(5).ToList(),
            AwaitingResults = events
                .Where(e => e.Date <= now && e.EnrolmentCount > e.ResultCount)
                .OrderByDescending(e => e.Date).ToList(),
            RecentEnrolments = recent
        };
    }

    // ---------------------------------------------------------------- Participant View
    private async Task<RaceDay7.Models.ViewModels.ParticipantDashboardViewModel> BuildParticipantAsync(ApplicationUser user)
    {
        var now = DateTime.Now;

        var mine = await _db.Enrolments
            .AsNoTracking()
            .Where(en => en.ParticipantId == user.Id)
            .Select(en => new ParticipantEnrolmentRow
            {
                EventId = en.EventId,
                EventName = en.Event!.Name,
                Date = en.Event.Date,
                Location = en.Event.Location,
                EventType = en.Event.EventType,
                DistanceKm = en.Event.DistanceKm,
                CategoryName = en.Category!.Name,
                FinishTime = en.Result != null ? en.Result.FinishTime : (TimeSpan?)null,
                Position = en.Result != null ? en.Result.Position : (int?)null
            })
            .ToListAsync();

        var upcoming = mine.Where(m => m.Date > now).OrderBy(m => m.Date).ToList();
        var withResults = mine.Where(m => m.Position.HasValue).OrderByDescending(m => m.Date).ToList();

        var enrolledEventIds = mine.Select(m => m.EventId).ToList();
        var open = await _db.Events
            .AsNoTracking()
            .Where(e => e.Date > now && e.Categories.Any() && !enrolledEventIds.Contains(e.Id))
            .OrderBy(e => e.Date)
            .Take(5)
            .Select(e => new OpenEventRow
            {
                Id = e.Id,
                Name = e.Name,
                Date = e.Date,
                Location = e.Location,
                EventType = e.EventType,
                DistanceKm = e.DistanceKm
            })
            .ToListAsync();

        return new ParticipantDashboardViewModel
        {
            DisplayName = user.FirstName ?? "Participant",
            UpcomingCount = upcoming.Count,
            CompletedCount = mine.Count(m => m.Date <= now),
            ResultsCount = withResults.Count,
            BestPosition = withResults.Count > 0 ? withResults.Min(r => r.Position) : null,
            NextEvent = upcoming.FirstOrDefault(),
            Upcoming = upcoming.Take(5).ToList(),
            RecentResults = withResults.Take(5).ToList(),
            OpenEvents = open
        };
    }
}