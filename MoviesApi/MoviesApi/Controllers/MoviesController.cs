using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

    [Microsoft.AspNetCore.Mvc.Route("api/movies")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
    public class MoviesController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly string container = "movies";

        public MoviesController(ApplicationDbContext dbContext, IMapper mapper, IFileStorageService fileStorageService, UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            this.userManager = userManager;

        }

        //[HttpGet]
        //public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        //{


        //    var querable = dbContext.Actors.AsQueryable();
        //    await HttpContext.insertParametersPaginationInHeader(querable);

        //    var actors = await this.dbContext.Actors.OrderBy(x => x.Name).paginate(paginationDTO).ToListAsync();
        //    return mapper.Map<List<ActorDTO>>(actors);
        //}


        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<ActorDTO>> Get(int id)
        //{
        //    var actor = await dbContext.Actors.FirstOrDefaultAsync(item => item.Id == id);

        //    if (actor == null) return NotFound();

        //    return mapper.Map<ActorDTO>(actor);
        //}

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] MovieCreateDTO movieCreateDTO)
        {

            var movie = mapper.Map<Movie>(movieCreateDTO);

            if (movieCreateDTO.Poster != null) movie.Poster = await fileStorageService.SaveFile(container, movieCreateDTO.Poster);

            PopulateActorOrder(movie);

            dbContext.Movies.Add(movie);
            await dbContext.SaveChangesAsync();

            return movie.Id;
        }

        private void PopulateActorOrder(Movie movie)
        {
            if(movie.MoviesActors != null)
            {
                for(int i=0; i< movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }

        }

        [HttpGet("PostGet")]
        public async Task<MoviesTheatersGenresDTO> Get()
        {
            var genres = await dbContext.Genres.ToListAsync();
            var theaters = await dbContext.Theaters.ToArrayAsync();


            var genresDTO = mapper.Map<List<GenreDTO>>(genres);
            var theatersDTO = mapper.Map<List<TheaterDTO>>(theaters);

            return new MoviesTheatersGenresDTO() { Genres = genresDTO, Theaters = theatersDTO };


        }

        [HttpGet("LandingPage")]
        [AllowAnonymous]
        public async Task<ActionResult<LandingPageDTO>> GetLandingPage()
        {
            var movies = await dbContext.Movies.ToListAsync();
            var inTheaters = movies.Where(x => x.InTheaters).OrderBy(x => x.ReleaseDate).Take(5).ToList();
            var upcomingReleases = movies.Where(x => x.ReleaseDate > new DateTime()).OrderBy(x => x.ReleaseDate).Take(5).ToList();
            var inTheaterMovies = mapper.Map<List<MovieDTO>>(inTheaters);
            var upcomingReleasesMovies = mapper.Map<List<MovieDTO>>(upcomingReleases);


            return new LandingPageDTO() { InTheaters = inTheaterMovies, UpcomingReleases = upcomingReleasesMovies };


        }

        [HttpGet("PutGet/{id:int}")]
        public async Task<ActionResult<MoviePutGetDTO>> PutGet(int id)
        {
            var movieResult = await Get(id);

            if (movieResult.Result is NotFoundResult) return NotFound();

            var movie = movieResult.Value;
            var genresSelectedIds = movie.Genres.Select(x => x.Id).ToList();
            var nonSelectedGenres = await dbContext.Genres.Where(x => !genresSelectedIds.Contains(x.Id)).ToListAsync();

            var theatersSelectedIds = movie.Theaters.Select(x => x.Id).ToList();
            var nonSelectedTheaters = await dbContext.Theaters.Where(x => !theatersSelectedIds.Contains(x.Id)).ToListAsync();

            var nonSelectedGenresDTOs = mapper.Map<List<GenreDTO>>(nonSelectedGenres);
            var nonSelectedTheatersDTOs = mapper.Map<List<TheaterDTO>>(nonSelectedTheaters);

            var response = new MoviePutGetDTO();

            response.Movie = movie;
            response.NonSelectedGenres = nonSelectedGenresDTOs;
            response.NonSelectedTheaters = nonSelectedTheatersDTOs;
            response.SelectedGenres = movie.Genres;
            response.SelectedTheaters = movie.Theaters;
            response.Actors = movie.Actors;

            return response;


        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await dbContext.Movies.Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MoviesTheaters).ThenInclude(x => x.Theater)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null) return NotFound();

            var averageVote = 0.0;
            var userVote = 0;

            if(await dbContext.Ratings.AnyAsync(x => x.MovieId == id))
            {
                averageVote = await dbContext.Ratings.Where(x => x.MovieId == id).AverageAsync(x => x.Rate);

                if(HttpContext.User.Identity.IsAuthenticated) // should have the [Authorize] on the method or the controller to be able to access this info in Identity
                {
                    var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email").Value;
                    var user = await userManager.FindByEmailAsync(email);
                    var userId = user.Id;

                    var ratingDb = await dbContext.Ratings.FirstOrDefaultAsync(x => x.MovieId == id && x.UserId == userId);

                    if(ratingDb != null)
                    {
                        userVote = ratingDb.Rate;
                    }
                }
            }


            var dto = mapper.Map<MovieDTO>(movie);
            dto.AverageVote = averageVote;
            dto.UserVote = userVote;
            dto.Actors = dto.Actors.OrderBy(x => x.Order).ToList();

            return dto;

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreateDTO movieCreateDTO)
        {

            var movie = await dbContext.Movies
                .Include(x => x.MoviesGenres)
                .Include(x => x.MoviesTheaters)
                .Include(x => x.MoviesActors)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (movie == null) return NotFound();

            movie = mapper.Map(movieCreateDTO, movie);
            if (movieCreateDTO.Poster != null) movie.Poster = await fileStorageService.EditFile(container, movieCreateDTO.Poster, movie.Poster);

            PopulateActorOrder(movie);

            await dbContext.SaveChangesAsync();

            return NoContent();


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            var movie = await dbContext.Movies.FirstOrDefaultAsync(item => item.Id == id);

            if (movie == null) return NotFound();

            dbContext.Remove(movie);
            await dbContext.SaveChangesAsync();
            await fileStorageService.DeleteFile(movie.Poster,container);
            return NoContent();
        }

        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<MovieDTO>>> Get([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var movieIquerable = dbContext.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(filterMoviesDTO.Title)) {
                movieIquerable = movieIquerable.Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }

            if (filterMoviesDTO.InTeaters) {
                movieIquerable = movieIquerable.Where(x => x.InTheaters);
            }
            if (filterMoviesDTO.UpcomingReleases)
            {
                movieIquerable = movieIquerable.Where(x => x.ReleaseDate > DateTime.Today);
            }
            if (filterMoviesDTO.GenreId != 0)
            {
                movieIquerable = movieIquerable.Where(x => x.MoviesGenres.Select(x => x.GenreId).Contains(filterMoviesDTO.GenreId));
               
            }
            await HttpContext.insertParametersPaginationInHeader(movieIquerable);
            var movies = await movieIquerable.OrderBy(x => x.Title).paginate(filterMoviesDTO.paginationDTO).ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }
    }
}
