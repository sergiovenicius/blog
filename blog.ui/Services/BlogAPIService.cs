using blog.ui.Models;
using Microsoft.Net.Http.Headers;
using System.Buffers.Text;
using System.Net.Http;
using System.Text.Json;

namespace blog.ui.Services
{
    public class BlogAPIService : IBlogAPIService
    {
        private readonly ILogger<BlogAPIService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private User _currentUser;
        private const string urlServiceBase = "http://localhost:5000";

        public BlogAPIService(ILogger<BlogAPIService> logger, IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;
            this._httpClientFactory = httpClientFactory;

            Initialize();
        }

        public void SetUser(string username, string password)
        {
            _currentUser = new User() { Username = username, Password = password };
        }

        private string GetBase64()
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(_currentUser.Username + ":" + _currentUser.Password);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public async Task Initialize()
        {
            using (var client = _httpClientFactory.CreateClient())
            { 

                User userPublic = new User()
                {
                    Name = "public",
                    Username = "puser",
                    Password = "puser",
                    Email = "p@email.com",
                    Role = new List<UserRole>() { UserRole.Public }
                };

                User userWriter = new User()
                {
                    Name = "writer",
                    Username = "wuser",
                    Password = "wuser",
                    Email = "w@email.com",
                    Role = new List<UserRole>() { UserRole.Writer }
                };

                User userEditor = new User()
                {
                    Name = "editor",
                    Username = "euser",
                    Password = "euser",
                    Email = "e@email.com",
                    Role = new List<UserRole>() { UserRole.Editor }
                };

                var pUserContent = JsonContent.Create<User>(userPublic, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
                var wUserContent = JsonContent.Create<User>(userWriter, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
                var eUserContent = JsonContent.Create<User>(userEditor, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

                string urlService = urlServiceBase + "/api/users";

                await client.PostAsync(urlService, pUserContent);
                await client.PostAsync(urlService, wUserContent);
                await client.PostAsync(urlService, eUserContent);
            }
        }

        public async Task<List<Post>> ListAllPublishedPosts()
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                string urlService = urlServiceBase + "/api/posts";

                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get,
                    urlService)
                    {
                        Headers =
                        {
                            { HeaderNames.Accept, "application/vnd.github.v3+json" },
                            { HeaderNames.Authorization, "Basic " +  GetBase64()}
                        }
                    };

                var httpResponseMessage = await client.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    var options = new JsonSerializerOptions();
                    options.PropertyNameCaseInsensitive = true;

                    return await JsonSerializer.DeserializeAsync<List<Post>>(contentStream, options);
                }
                else
                    return new List<Post>();

            }
        }

        public async Task<Post> GetPostById(int postId)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                string urlService = urlServiceBase + $"/api/posts/{postId}";

                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get,
                    urlService)
                {
                    Headers =
                        {
                            { HeaderNames.Accept, "application/vnd.github.v3+json" },
                            { HeaderNames.Authorization, "Basic " +  GetBase64()}
                        }
                };

                var httpResponseMessage = await client.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    var options = new JsonSerializerOptions();
                    options.PropertyNameCaseInsensitive = true;

                    var post = await JsonSerializer.DeserializeAsync<Post>(contentStream, options);

                    urlService = urlServiceBase + $"/api/users/id/{post.OwnerId}";

                    var response = await client.GetAsync(urlService);

                    if (response.IsSuccessStatusCode)
                    {
                        using var contentStreamUser = await response.Content.ReadAsStreamAsync();

                        var user = await JsonSerializer.DeserializeAsync<User>(contentStreamUser, options);

                        post.Owner = user;
                    }

                    return post;
                }
                return new Post();
            }

        }
    }
}
