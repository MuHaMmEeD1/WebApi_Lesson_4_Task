using Microsoft.EntityFrameworkCore;
using WebApi_Lesson_4_Task.Data;
using WebApi_Lesson_4_Task.Dtos;
using WebApi_Lesson_4_Task.Repositories.Abstract;

namespace WebApi_Lesson_4_Task.Repositories.Concreat
{
    public class MovieReopsitory : IMovieRepository
    {
        private readonly MovieDbContext _context;

        public MovieReopsitory(MovieDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(MovieDto movieDto)
        {
            await _context.AddAsync(movieDto);
        }

        public async Task<bool> FindMovieAsync(string MovieId)
        {

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.MoviId == MovieId);
            return await Task.FromResult(movie != null);
        }

        public async Task<List<MovieDto>> GetAllAsync()
        {
            return await _context.Movies.ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
