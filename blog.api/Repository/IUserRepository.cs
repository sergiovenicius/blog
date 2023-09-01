using blog.common.Model;

namespace blog.common.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> List();
        Task<User> GetById(long id);
        Task<User> GetByUserName(string username);

        Task<User> Save(User user);

        User? Authenticate(string username, string password);
    }
}
