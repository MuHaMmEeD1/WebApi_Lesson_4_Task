using Microsoft.EntityFrameworkCore;
using WebApi_Lesson_4_Task.Dtos;

namespace WebApi_Lesson_4_Task.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<MovieDto> Movies { get; set; } 

    }
}
