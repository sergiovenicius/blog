using blog.common.Model;
using blog.common.Repository;

namespace blog.common.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> List()
        {
            return await _repository.List();
        }

        public async Task<User> GetById(long id)
        {
            var dbUser = await _repository.GetById(id);
            return dbUser;
        }

        public async Task<User> Save(User user)
        {
            user.ID = 0;

            if (user.Role.Count() == 0)
                user.Role.Add(UserRole.Public);

            var dbUser = await _repository.Save(user);
            return dbUser;
        }

        public async Task<User> GetByUsername(string username)
        {
            var dbUser = await _repository.GetByUserName(username);
            return dbUser;
        }

        public async Task<bool> HasRole(long userId, UserRole role)
        {
            var user = await GetById(userId);

            return user.Role.Contains(role);
        }

        public User? Authenticate(string username, string password)
        {
            return _repository.Authenticate(username, password);
        }
    }
}
