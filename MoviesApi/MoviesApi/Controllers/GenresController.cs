using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Filters;
using MoviesApi.Helpers;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/genres")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]

    public class GenresController : ControllerBase
    {

        private readonly ILogger<GenresController> logger;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public GenresController(ILogger<GenresController> logger, ApplicationDbContext dbContext, IMapper mapper)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.mapper = mapper;

        }

        [HttpGet] // api/genres
        // [HttpGet("list")] api/genres/list
        // [HttpGet("/allgenres")] allgenres instead of api/genres
        // [ResponseCache(Duration = 60)]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // [ServiceFilter(typeof(MyActionsFilter))]
        public async Task<ActionResult<List<GenreDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {

            logger.LogWarning("hiiiiiiiiii"); // see log config in appsettings.json
            logger.LogInformation("hiiiiiiiiii");
            logger.LogError("hiiiiiiiiiiii");
            logger.LogDebug("hiiiiiiii");
            logger.LogTrace("hiiiiiiiii");
            logger.LogCritical("hiiiiiiiii");

            var querable = dbContext.Genres.AsQueryable();
            await HttpContext.insertParametersPaginationInHeader(querable);

            //throw new Exception(); // trigger exception filter
            var genres = await this.dbContext.Genres.OrderBy(x => x.Name).paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<GenreDTO>>(genres);
            }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {

         
            var genres = await this.dbContext.Genres.OrderBy(x => x.Name).ToListAsync();
            return mapper.Map<List<GenreDTO>>(genres);
        }
        //// [HttpGet("example")] api/genres/example?Id=2
        //// [HttpGet("{Id}/{param}")] // api/genres/2
        //[HttpGet("{Id:int}/{param1}")]
        //public ActionResult Get(int Id, string param1, [BindRequired] string param2, [BindNever] string param3, [FromHeader] string param4)
        //{

        //    if(!ModelState.IsValid) // No need if using [ApiController] above
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var genre = new Genre() { Id = 2, Name = "Drama" };

        //    if (genre == null)
        //    {
        //        return NotFound(); // ActionResult
        //    }

        //    return Ok(genre); // ok because return type is just  ActionResult and not  ActionResult<Genre>, we can return any type inside ok loke Ok("hi")
        //}

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            var genre = await dbContext.Genres.FirstOrDefaultAsync(item => item.Id == id);

            if (genre == null) return NotFound();

            return mapper.Map<GenreDTO>(genre);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody ] GenreCreateDTO genreCreateDTO)
        {
            var genre = mapper.Map<Genre>(genreCreateDTO);
            this.dbContext.Add(genre);
            await dbContext.SaveChangesAsync();

            return Ok(200);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreateDTO genreCreateDTO)
        {
            var genre = await dbContext.Genres.FirstOrDefaultAsync(item => item.Id == id);

            if (genre == null) return NotFound();

            genre =  mapper.Map(genreCreateDTO, genre);

            await dbContext.SaveChangesAsync();

            return NoContent();


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Genres.AnyAsync(item => item.Id == id);

            if (!exist) return NotFound();

            dbContext.Remove(new Genre() { Id = id });
            await dbContext.SaveChangesAsync();
            return NoContent();

        }
    }
}
