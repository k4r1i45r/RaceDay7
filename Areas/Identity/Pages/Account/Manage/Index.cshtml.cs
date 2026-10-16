using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RaceDay7.Models;
using RaceDay7.Data;
using Microsoft.EntityFrameworkCore;

namespace RaceDay7.Areas.Identity.Pages.Account.Manage
{
    public class ManageIndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ManageIndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; } = string.Empty;

            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;

            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            Username = user.UserName;

            // Check if user is a Participant or Organiser to load their profile data
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant != null)
            {
                FirstName = participant.FirstName;
                LastName = participant.LastName;
            }

            Input = new InputModel
            {
                PhoneNumber = user.PhoneNumber,
                FirstName = FirstName,
                LastName = LastName
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            // Update Phone Number
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Update Participant Profile (First Name, Last Name)
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant != null)
            {
                participant.FirstName = Input.FirstName;
                participant.LastName = Input.LastName;
                _context.Participants.Update(participant);
                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}