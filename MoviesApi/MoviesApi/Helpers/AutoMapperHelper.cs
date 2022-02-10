using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helpers
{
    public class AutoMapperHelper : Profile
    {
        public AutoMapperHelper(GeometryFactory geometryFactory)
        {
            CreateMap<GenreDTO, Genre>().ReverseMap();
            CreateMap<GenreCreateDTO, Genre>().ReverseMap();

            CreateMap<ActorDTO, Actor>().ReverseMap();
            CreateMap<ActorCreateDTO, Actor>()
                .ForMember(x => x.Picture, options => options.Ignore());
            CreateMap<Theater, TheaterDTO>()
                .ForMember(x => x.langitude, dto => dto.MapFrom(prop => prop.Location.Y))
                .ForMember(x => x.longitude, dto => dto.MapFrom(prop => prop.Location.X));

            CreateMap<TheaterCreateDTO, Theater>()
               .ForMember(x => x.Location, dto => dto.MapFrom(prop => geometryFactory.CreatePoint(new Coordinate(prop.longitude, prop.langitude))));

            CreateMap<MovieCreateDTO, Movie>()
              .ForMember(x => x.Poster, opt => opt.Ignore())
              .ForMember(x => x.MoviesGenres, opt => opt.MapFrom(MapMoviesGenres))
              .ForMember(x => x.MoviesTheaters, opt => opt.MapFrom(MapMoviesTheaters))
              .ForMember(x => x.MoviesActors, opt => opt.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDTO>()
              .ForMember(x => x.Genres, opt => opt.MapFrom(MapMoviesGenres))
              .ForMember(x => x.Theaters, opt => opt.MapFrom(MapMoviesTheaters))
              .ForMember(x => x.Actors, opt => opt.MapFrom(MapMoviesActors));

            CreateMap<IdentityUser, UserDTO>();
        }

        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<GenreDTO>();

            if (movie.MoviesGenres == null) return result;

            foreach (var genre in movie.MoviesGenres)
            {
                result.Add(new GenreDTO() { Id = genre.GenreId, Name = genre.Genre.Name });
            }
            return result;
        }

        private List<TheaterDTO> MapMoviesTheaters(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<TheaterDTO>();

            if (movie.MoviesTheaters == null) return result;

            foreach (var theater in movie.MoviesTheaters)
            {
                result.Add(new TheaterDTO() { Id = theater.TheaterId, Name = theater.Theater.Name, langitude= theater.Theater.Location.Y, longitude = theater.Theater.Location.X});
            }
            return result;
        }

        private List<ActorDTO> MapMoviesActors(Movie movie, MovieDTO movieDTO)
        {
            var result = new List<ActorDTO>();

            if (movie.MoviesActors == null) return result;

            foreach (var actor in movie.MoviesActors)
            {
                result.Add(new ActorDTO() { Id = actor.ActorId, Name = actor.Actor.Name, Biography = actor.Actor.Biography, character = actor.Character, DateOfBirth = actor.Actor.DateOfBirth, Order =actor.Order, Picture = actor.Actor.Picture });
            }
            return result;
        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();

            if (movieCreateDTO.GenresIds == null) return result;

            foreach(var id in movieCreateDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }
            return result;
        }

        private List<MoviesTheaters> MapMoviesTheaters(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var result = new List<MoviesTheaters>();

            if (movieCreateDTO.TheatersIds == null) return result;

            foreach (var id in movieCreateDTO.TheatersIds)
            {
                result.Add(new MoviesTheaters() { TheaterId = id });
            }
            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreateDTO movieCreateDTO, Movie movie)
        {
            var result = new List<MoviesActors>();

            if (movieCreateDTO.Actors == null) return result;

            foreach (var actor in movieCreateDTO.Actors)
            {
                result.Add(new MoviesActors() { ActorId = actor.Id, Character = actor.character });
            }
            return result;
        }
    }
}
