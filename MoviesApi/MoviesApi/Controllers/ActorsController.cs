using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
    [Microsoft.AspNetCore.Mvc.Route("api/actors")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]

    public class ActorsController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly string containerName = "actors";

        public ActorsController( ApplicationDbContext dbContext, IMapper mapper, IFileStorageService fileStorageService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {


            var querable = dbContext.Actors.AsQueryable();
            await HttpContext.insertParametersPaginationInHeader(querable);

            var actors = await this.dbContext.Actors.OrderBy(x => x.Name).paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<ActorDTO>>(actors);
        }

       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var actor =  dbContext.Actors.FirstOrDefaultAsync(item => item.Id == id);

            if (actor == null) return NotFound();

            return mapper.Map<ActorDTO>(actor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreateDTO actorCreateDTO)
        {

            var actor = mapper.Map<Actor>(actorCreateDTO);

            if (actorCreateDTO.Picture != null) actor.Picture = await fileStorageService.SaveFile(containerName, actorCreateDTO.Picture);

            dbContext.Actors.Add(actor);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreateDTO actorCreateDTO)
        {

            var actor = await dbContext.Actors.FirstOrDefaultAsync(item => item.Id == id);

            if (actor == null) return NotFound();

            actor = mapper.Map(actorCreateDTO, actor);
            if (actorCreateDTO.Picture != null) actor.Picture = await fileStorageService.EditFile(containerName, actorCreateDTO.Picture, actor.Picture);

            await dbContext.SaveChangesAsync();

            return NoContent();


        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {

            var actor = await dbContext.Actors.FirstOrDefaultAsync(item => item.Id == id);

            if (actor == null) return NotFound();

            dbContext.Remove(actor);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("searchByName/{query}")]
        public async Task<ActionResult<List<SearchActorDTO>>> Get(string query)
        {

            if (string.IsNullOrEmpty(query)) return new List<SearchActorDTO>();

            return await dbContext.Actors
                .Where(x => x.Name.Contains(query))
                .OrderBy(x => x.Name)
                .Select(x => new SearchActorDTO() { Id = x.Id, Name = x.Name, Picture = x.Picture })
                .Take(5)
                .ToListAsync();
        }

        
    }
}
