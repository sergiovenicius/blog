using blog.api.Mapper;
using blog.api.Model;
using blog.common.Database;
using blog.common.Exceptions;
using blog.common.Model;
using blog.common.Repository;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace blog.api.test
{
    public class UserRepositoryTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestUserRepositoryShouldGetUserByIdNullFail()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.GetById(0));
            }
        }

        [Test]
        public void TestUserRepositoryShouldGetUserByUsernameNullFail()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.GetByUserName("test"));
            }
        }

        [Test]
        public async Task TestUserRepositoryShouldGetUserByIdHappyPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                await dbPostRepo.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = await dbPostRepo.GetById(1);

                Assert.That(user.ID, Is.EqualTo(1));
                Assert.That(user.Username, Is.EqualTo("newuser"));
                Assert.That(user.Password, Is.EqualTo("pwd"));
                Assert.That(user.Name, Is.EqualTo("user"));
                Assert.That(user.Email, Is.EqualTo("any@email.com"));
                Assert.That(user.Role.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public async Task TestUserRepositoryShouldGetUserByUsernameHappyPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                await dbPostRepo.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = await dbPostRepo.GetByUserName("newuser");

                Assert.That(user.ID, Is.EqualTo(1));
                Assert.That(user.Username, Is.EqualTo("newuser"));
                Assert.That(user.Password, Is.EqualTo("pwd"));
                Assert.That(user.Name, Is.EqualTo("user"));
                Assert.That(user.Email, Is.EqualTo("any@email.com"));
                Assert.That(user.Role.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public async Task TestUserRepositoryShouldAuthenticateHappyPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                await dbPostRepo.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = dbPostRepo.Authenticate("newuser", "pwd");

                Assert.That(user.ID, Is.EqualTo(1));
                Assert.That(user.Username, Is.EqualTo("newuser"));
                Assert.That(user.Password, Is.EqualTo("pwd"));
                Assert.That(user.Name, Is.EqualTo("user"));
                Assert.That(user.Email, Is.EqualTo("any@email.com"));
                Assert.That(user.Role.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public async Task TestUserRepositoryShouldAuthenticateFailPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                await dbPostRepo.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = dbPostRepo.Authenticate("newuser", "wrong");

                Assert.IsNull(user);
            }
        }

    }
}