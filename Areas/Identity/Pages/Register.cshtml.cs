using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RaceDay7.Data;
using RaceDay7.Models;
using RaceDay7.Data;
using RaceDay7.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDay7.Areas.Identity.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // For file uploads

        public RegisterModel(
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

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            public string Gender { get; set; }

            [Display(Name = "Profile Image")]
            public IFormFile? ProfileImage { get; set; } // File upload

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
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Gender = (Gender)Enum.Parse(typeof(Gender), Input.Gender)
                };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
                    var role = await roleManager.FindByNameAsync(Input.Role);
                    if (role == null)
                    {
                        role = new IdentityRole { Name = Input.Role };
                        await roleManager.CreateAsync(role);
                    }

                    user.RoleId = role.Id;
                    await _userManager.UpdateAsync(user);

                    // Handle Profile Image Upload
                    string profileImagePath = "default-avatar.png"; // Fallback image
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
                        profileImagePath = uniqueFileName;
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
                            ProfileImagePath = profileImagePath, // Save image path
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