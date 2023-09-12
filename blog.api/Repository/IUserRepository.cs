using blog.common.Model;

namespace blog.common.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> ListAsync();
        Task<User> GetByIdAsync(long id);
        Task<User> GetByUserNameAsync(string username);

        Task<User> SaveAsync(User user);

        User? Authenticate(string username, string password);
    }
}
