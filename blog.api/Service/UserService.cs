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

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await _repository.ListAsync();
        }

        public async Task<User> GetByIdAsync(long id)
        {
            var dbUser = await _repository.GetByIdAsync(id);
            return dbUser;
        }

        public async Task<User> SaveAsync(User user)
        {
            user.ID = 0;

            if (user.Role.Count() == 0)
                user.Role.Add(UserRole.Public);

            var dbUser = await _repository.SaveAsync(user);
            return dbUser;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var dbUser = await _repository.GetByUserNameAsync(username);
            return dbUser;
        }

        public async Task<bool> HasRole(long userId, UserRole role)
        {
            var user = await GetByIdAsync(userId);

            return user.Role.Contains(role);
        }

        public User? Authenticate(string username, string password)
        {
            return _repository.Authenticate(username, password);
        }
    }
}
