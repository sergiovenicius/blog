using blog.common.Model;

namespace blog.common.Service
{
    public interface IUserService
    {
        Task<IEnumerable<User>> ListAsync();

        Task<User> GetByIdAsync(long id);

        Task<User> GetByUsernameAsync(string username);

        Task<User> SaveAsync(User User);

        Task<bool> HasRole(long userId, UserRole role);

        User? Authenticate(string username, string password);
    }
}
