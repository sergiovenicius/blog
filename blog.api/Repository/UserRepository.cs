using blog.common.Database;
using blog.common.Exceptions;
using blog.common.Model;
using Microsoft.EntityFrameworkCore;

namespace blog.common.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContextBlog db;
        public UserRepository(DBContextBlog db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<User>> List()
        {
            return await db.Users.ToListAsync();
        }

        public async Task<User> GetById(long id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                throw new NotFoundException("User not found");
            return user;
        }


        public async Task<User> Save(User user)
        {
            var newUser = await db.Users.AddAsync(user);

            db.SaveChanges();

            return newUser.Entity;
        }

        public async Task<User> GetByUserName(string username)
        {
            var user = await db.Users.FirstOrDefaultAsync(r => r.Username == username);
            if (user == null)
                throw new NotFoundException("User not found");
            return user;
        }

        public User? Authenticate(string username, string password)
        {
            return db.Users.AsNoTracking().FirstOrDefault(r => r.Username == username && r.Password == password);
        }
    }
}
