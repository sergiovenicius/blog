using blog.common.Model;

namespace blog.common.Service
{
    public interface IUserService
    {
        Task<IEnumerable<User>> List();

        Task<User> GetById(long id);

        Task<User> GetByUsername(string username);

        Task<User> Save(User User);

        Task<bool> HasRole(long userId, UserRole role);

        User? Authenticate(string username, string password);
    }
}
