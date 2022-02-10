using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/theaters")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]

    public class TheatersController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public TheatersController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;

        }

        [HttpGet] 
        public async Task<ActionResult<List<TheaterDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var querable = dbContext.Theaters.AsQueryable();
            await HttpContext.insertParametersPaginationInHeader(querable);

            var theaters = await this.dbContext.Theaters.OrderBy(x => x.Name).paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<TheaterDTO>>(theaters);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<TheaterDTO>> Get(int id)
        {
            var theater = await dbContext.Theaters.FirstOrDefaultAsync(item => item.Id == id);

            if (theater == null) return NotFound();

            return mapper.Map<TheaterDTO>(theater);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TheaterCreateDTO theaterCreateDTO)
        {
            var theater = mapper.Map<Theater>(theaterCreateDTO);
            this.dbContext.Add(theater);
            await dbContext.SaveChangesAsync();

            return Ok(200);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] TheaterCreateDTO theaterCreateDTO)
        {
            var theater = await dbContext.Theaters.FirstOrDefaultAsync(item => item.Id == id);

            if (theater == null) return NotFound();

            theater = mapper.Map(theaterCreateDTO, theater);

            await dbContext.SaveChangesAsync();

            return NoContent();


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var theater = await dbContext.Theaters.FirstOrDefaultAsync(item => item.Id == id);

            if (theater == null) return NotFound();

            dbContext.Remove(theater);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
