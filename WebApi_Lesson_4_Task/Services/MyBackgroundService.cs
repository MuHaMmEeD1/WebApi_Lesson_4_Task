using System.Net.Http.Headers;
using System.Text.Json;
using WebApi_Lesson_4_Task.Dtos;
using WebApi_Lesson_4_Task.Repositories.Abstract;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

public class MyBackgroundService : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private int _page = 1;
    private char _firstLetter = 'a';
    private bool _xusisiVezyet = false;

    public MyBackgroundService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;

        var apiKey = _configuration.GetSection("ApiKey").Value;
        if (!string.IsNullOrEmpty(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_xusisiVezyet)
                {
                    _page += 1;
                    _xusisiVezyet = false;
                }
                else
                {
                    _page = 1;
                    _firstLetter = (char)Random.Shared.Next(97, 122);
                }

                string apiUrl = $"https://yts.mx/api/v2/list_movies.json?query_term={_firstLetter}&page={_page}&limit={_configuration.GetSection("MovieCount").Value}";

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl, stoppingToken);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                try
                {
                    JsonDocument jsonDoc = JsonDocument.Parse(content);
                    var movies = jsonDoc.RootElement.GetProperty("data").GetProperty("movies");

                    List<MovieDto> movieTitles = new List<MovieDto>();

                    foreach (var movie in movies.EnumerateArray())
                    {
                        string title = movie.GetProperty("title").GetString()!;
                        string movieId = movie.GetProperty("imdb_code").GetString()!;

                        if (title.ToLower().StartsWith(_firstLetter))
                        {
                            movieTitles.Add(new MovieDto { Title = title, MoviId = movieId });
                        }
                    }

                    Console.WriteLine("--------------------SSS");
                    Console.WriteLine();

                    bool added = false;

                    foreach (var movie in movieTitles)
                    {
                        Console.WriteLine("- " + movie.Title);
                        Console.WriteLine("- " + movie.MoviId);
                        Console.WriteLine();

                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var movieRepository = scope.ServiceProvider.GetRequiredService<IMovieRepository>();

                            if (!(await movieRepository.FindMovieAsync(movie.MoviId)) && !added)
                            {
                                await movieRepository.AddAsync(movie);
                                added = true;
                                await movieRepository.SaveAsync();


                                Console.WriteLine();
                                var defaultBackgroundColor = Console.BackgroundColor;
                                Console.BackgroundColor = ConsoleColor.Green;
                                Console.WriteLine($" OK Add Proses Perfect {movie.Title}   |||   {movie.MoviId}");
                                Console.BackgroundColor = defaultBackgroundColor;
                                Console.WriteLine();
                            }
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("--------------------EEE");

                    if (!added) { _xusisiVezyet = true; }
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine(jsonEx.Message);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
