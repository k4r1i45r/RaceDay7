using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RaceDay7.Data;
using RaceDay7.Models;
using RaceDay7.Models.Interface;

namespace RaceDay7.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IProfileImageService _profileImageService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IProfileImageService profileImageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _profileImageService = profileImageService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ReturnUrl { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            public string Gender { get; set; }

            public IFormFile? ProfileImage { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Role { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync(Input.Role);
                    if (role == null)
                    {
                        role = new IdentityRole { Name = Input.Role };
                        await _roleManager.CreateAsync(role);
                    }

                    user.RoleId = role.Id;
                    await _userManager.UpdateAsync(user);

                    string profileImagePath = "default-avatar.png";
                    if (Input.ProfileImage != null)
                    {
                        var savedPath = await _profileImageService.SaveAsync(Input.ProfileImage);
                        if (savedPath != null) profileImagePath = savedPath;
                    }

                    if (Input.Role == "Participant")
                    {
                        var participant = new Participant
                        {
                            UserId = user.Id,
                            Email = Input.Email,
                            FirstName = Input.FirstName,
                            LastName = Input.LastName,
                            Gender = Input.Gender,
                            ProfileImagePath = profileImagePath,
                            Address = "Not Provided",
                            City = "Not Provided",
                            Province = "Not Provided",
                            PhoneNumber = "0000000000",
                            EmergencyContactName = "Not Provided",
                            EmergencyContactNumber = "0000000000",
                            DateOfBirth = DateTime.Now
                        };
                        _context.Participants.Add(participant);
                        await _context.SaveChangesAsync();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}