using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portalia.Auth0Migration.Contracts;
using Portalia.Auth0Migration.Models;

namespace Portalia.Auth0Migration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly PortaliaContext _context;

        public ProfileController(PortaliaContext context)
        {
            _context = context;
        }       

        [HttpGet("{email}", Name = "GetProfile")]
        public async Task<ActionResult<ProfileContract>> GetByEmail(string email)
        {
            var profile = await _context.AspNetUsers
                    .Where(x => x.Email == email)
                    .Select(user => new ProfileContract(user.Id, user.UserName, user.FirstName, user.LastName, user.PasswordHash))
                    .SingleOrDefaultAsync();
            if (profile == null)
            {
                return NotFound();
            }
            return profile;
        }
    }
}