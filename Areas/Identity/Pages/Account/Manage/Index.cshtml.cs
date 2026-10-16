using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaceDay7.Data;
using RaceDay7.Models;
using RaceDay7.Data;
using RaceDay7.Models;
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
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            public string Gender { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public string Address { get; set; }
            public string City { get; set; }
            public string Province { get; set; }

            [Display(Name = "Emergency Contact Name")]
            public string EmergencyContactName { get; set; }

            [Display(Name = "Emergency Contact Number")]
            public string EmergencyContactNumber { get; set; }

            [Display(Name = "Update Profile Image")]
            public IFormFile? ProfileImage { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            Username = user.UserName;

            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant != null)
            {
                if (!string.IsNullOrEmpty(participant.ProfileImagePath))
                    CurrentProfileImage = participant.ProfileImagePath;

                Input = new InputModel
                {
                    FirstName = participant.FirstName,
                    LastName = participant.LastName,
                    Gender = participant.Gender,
                    PhoneNumber = user.PhoneNumber,
                    Address = participant.Address,
                    City = participant.City,
                    Province = participant.Province,
                    EmergencyContactName = participant.EmergencyContactName,
                    EmergencyContactNumber = participant.EmergencyContactNumber
                };
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

            // Update Participant Profile
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (participant != null)
            {
                participant.FirstName = Input.FirstName;
                participant.LastName = Input.LastName;
                participant.Gender = Input.Gender;
                participant.Address = Input.Address;
                participant.City = Input.City;
                participant.Province = Input.Province;
                participant.EmergencyContactName = Input.EmergencyContactName;
                participant.EmergencyContactNumber = Input.EmergencyContactNumber;

                // Handle Profile Image Update
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
                    participant.ProfileImagePath = uniqueFileName;
                }

                _context.Participants.Update(participant);
                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}