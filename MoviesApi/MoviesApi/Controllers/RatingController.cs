using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{

    [ApiController]
    [Route("api/ratings")]
    public class RatingController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        public RatingController(ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            this.context = context;
            this.userManager = userManager;

        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<ActionResult> Post([FromBody] RatingDTO ratingDTO)
        {
            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
            var user = await userManager.FindByEmailAsync(email);
            var userId = user.Id;
            var currentRate = await context.Ratings.Where(x => x.MovieId == ratingDTO.MovieId && x.UserId == userId).FirstOrDefaultAsync();

            if (currentRate == null)
            {
                var rating = new Rating();
                rating.MovieId = ratingDTO.MovieId;
                rating.UserId = userId;
                rating.Rate = ratingDTO.Rating;
                context.Add(rating);


            } else
            {
                currentRate.Rate = ratingDTO.Rating;

            }
            context.SaveChanges();

            return NoContent();

        }
    }
}
