using blog.ui.Models;

namespace blog.ui.Services
{
    public interface IBlogAPIService
    {
        Task<Post> GetPostById(int postId);
        Task<List<Post>> ListAllPublishedPosts();
        void SetUser(string username, string password);

        Task Initialize();
    }
}