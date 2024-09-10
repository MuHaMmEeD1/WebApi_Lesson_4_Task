using System.ComponentModel.DataAnnotations;

namespace WebApi_Lesson_4_Task.Dtos
{
    public class MovieDto
    {
        public string? Title { get; set; }
        [Key]
        public string? MoviId { get; set; }

    }
}
