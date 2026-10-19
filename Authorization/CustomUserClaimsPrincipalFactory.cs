using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RaceDay7.Data;
using RaceDay7.Models;
using Microsoft.EntityFrameworkCore;

namespace RaceDay7.Authorization
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly ApplicationDbContext _context;

        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options,
            ApplicationDbContext context)
            : base(userManager, roleManager, options)
        {
            _context = context;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Fetch the role from our 1-to-1 setup and add it as a Claim
            if (!string.IsNullOrEmpty(user.RoleId))
            {
                var role = await _context.Roles.FindAsync(user.RoleId);
                if (role != null)
                {
                    identity.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, role.Name));
                }
            }
            return identity;
        }
    }
}