using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.DTOs;
using MoviesApi.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, ApplicationDbContext dbContext, IMapper mapper) {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet("listUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]

        public async Task<ActionResult<List<UserDTO>>> GetListUsers([FromQuery] PaginationDTO paginationDTO)
        {
            var querable = dbContext.Users.AsQueryable();
            await HttpContext.insertParametersPaginationInHeader(querable);
            var users = await querable.OrderBy(x => x.Email).paginate(paginationDTO).ToListAsync();

            return mapper.Map<List<UserDTO>>(users);
        }


        [HttpPost("makeAdmin")]
       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]

        public async Task<ActionResult> MakeAdmin([FromBody] string userId)
        {

            var user = await userManager.FindByIdAsync(userId);

            await userManager.AddClaimAsync(user, new Claim("role", "admin"));

            return NoContent();

        }

        [HttpPost("removeAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]

        public async Task<ActionResult> RemoveAdmin([FromBody] string userId)
        {

            var user = await userManager.FindByIdAsync(userId);

            await userManager.RemoveClaimAsync(user, new Claim("role", "admin"));

            return NoContent();

        }

        [HttpPost("create")]
        public async Task<ActionResult<AuthResponse>> Create([FromBody] UserCredentials userCredentials)
        {
            var user = new IdentityUser { Email = userCredentials.Email, UserName = userCredentials.Email };
            var result = await userManager.CreateAsync(user, userCredentials.Password);

            if(result.Succeeded)
            {
                return await buildToken(userCredentials);
            } else
            {
                return BadRequest(result.Errors);
            }


        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] UserCredentials userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await buildToken(userCredentials);
            }
            else
            {
                return BadRequest("Incorrect credentials"); // dont send detailed error for security reasons
            }


        }
        private async Task<AuthResponse> buildToken(UserCredentials userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email)
        };

            var user = await userManager.FindByNameAsync(userCredentials.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);
            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new AuthResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };

        }
    }
}
