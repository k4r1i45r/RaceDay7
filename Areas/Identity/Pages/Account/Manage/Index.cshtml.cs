using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaceDay7.Data;
using RaceDay7.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay7.Areas.Identity.Pages.Account.Manage
{
    public class ManageIndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ManageIndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public string Username { get; set; } = string.Empty;
        public string CurrentProfileImage { get; set; } = "default-avatar.png";

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;

            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;

            public string Gender { get; set; } = string.Empty;

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; } = string.Empty;

            public string Address { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Province { get; set; } = string.Empty;

            [Display(Name = "Emergency Contact Name")]
            public string EmergencyContactName { get; set; } = string.Empty;

            [Display(Name = "Emergency Contact Number")]
            public string EmergencyContactNumber { get; set; } = string.Empty;

            [Display(Name = "Update Profile Image")]
            public IFormFile? ProfileImage { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            Username = user.UserName ?? string.Empty;

            // Populate ApplicationUser properties
            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender.ToString(),
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                ProfileImage = null // We don't load the image into the form, just the path
            };

            if (!string.IsNullOrEmpty(user.ProfileImagePath))
                CurrentProfileImage = user.ProfileImagePath;

            // Check if they have additional Participant details
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant != null)
            {
                Input.Address = participant.Address;
                Input.City = participant.City;
                Input.Province = participant.Province;
                Input.EmergencyContactName = participant.EmergencyContactName;
                Input.EmergencyContactNumber = participant.EmergencyContactNumber;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            // Update Phone Number in Identity
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            }

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            if (Enum.TryParse<Gender>(Input.Gender, out var genderEnum))
            {
                user.Gender = genderEnum;
            }

            // Handle Profile Image Update on ApplicationUser
            if (Input.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfileImage.CopyToAsync(fileStream);
                }
                user.ProfileImagePath = uniqueFileName;
            }

            // Save all changes to the ApplicationUser (Identity table)
            await _userManager.UpdateAsync(user);
            // ---------------------------------------------------------

            // Update Participant specific details (Address, City, Province, Emergency Contacts)
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant != null)
            {
                participant.Address = Input.Address;
                participant.City = Input.City;
                participant.Province = Input.Province;
                participant.EmergencyContactName = Input.EmergencyContactName;
                participant.EmergencyContactNumber = Input.EmergencyContactNumber;

                // Optional: Keep FirstName, LastName in Participant updated as well for consistency if they exist elsewhere
                participant.FirstName = Input.FirstName;
                participant.LastName = Input.LastName;
                participant.Gender = Input.Gender;

                _context.Participants.Update(participant);
                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}