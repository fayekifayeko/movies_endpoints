using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi
{
    public class ApplicationDbContext : IdentityDbContext
    {
  
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Actor> Actors { get; set; }

        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public DbSet<MoviesTheaters> MoviesTheaters { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesActors>()
                .HasKey(x => new { x.MovieId, x.ActorId});
            modelBuilder.Entity<MoviesGenres>()
                .HasKey(x => new { x.MovieId, x.GenreId });
            modelBuilder.Entity<MoviesTheaters>()
               .HasKey(x => new { x.MovieId, x.TheaterId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
