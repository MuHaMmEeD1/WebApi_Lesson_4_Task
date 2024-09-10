using System.Linq.Expressions;
using WebApi_Lesson_4_Task.Dtos;

namespace WebApi_Lesson_4_Task.Repositories.Abstract
{
    public interface IMovieRepository
    {

        Task<List<MovieDto>> GetAllAsync();
        Task<bool> FindMovieAsync(string MovieId);
        Task AddAsync(MovieDto movieDto);
        Task SaveAsync();

    }
}
