using blog.api.Mapper;
using blog.api.Model;
using blog.common.Database;
using blog.common.Exceptions;
using blog.common.Model;
using blog.common.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
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
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.GetByIdAsync(0));
            }
        }

        [Test]
        public void TestUserRepositoryShouldGetUserByUsernameNullFail()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.GetByUserNameAsync("test"));
            }
        }

        [Test]
        public async Task TestUserRepositoryShouldGetUserByIdHappyPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                await dbPostRepo.SaveAsync(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = await dbPostRepo.GetByIdAsync(1);

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
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                await dbPostRepo.SaveAsync(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = await dbPostRepo.GetByUserNameAsync("newuser");

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
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                await dbPostRepo.SaveAsync(new User()
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
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                await dbPostRepo.SaveAsync(new User()
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