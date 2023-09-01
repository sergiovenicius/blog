using blog.api.Mapper;
using blog.api.Model;
using blog.common.Database;
using blog.common.Exceptions;
using blog.common.Model;
using blog.common.Repository;
using blog.common.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace blog.api.test
{
    public class UserServiceTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestUserServiceShouldGetUserByIdNullFail()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbPostRepo);

                Assert.ThrowsAsync<NotFoundException>(async () => await svc.GetById(0));
                Assert.IsTrue(Assert.ThrowsAsync<NotFoundException>(async () => await svc.GetById(0)).Message.StartsWith("User not found"));
            }
        }

        [Test]
        public void TestUserServiceShouldGetUserByUsernameNullFail()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbPostRepo);

                Assert.ThrowsAsync<NotFoundException>(async () => await svc.GetByUsername("test"));
                Assert.IsTrue(Assert.ThrowsAsync<NotFoundException>(async () => await svc.GetByUsername("test")).Message.StartsWith("User not found"));
            }
        }

        [Test]
        public async Task TestUserServiceShouldAddUserWithoutRoleHappyPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbPostRepo);

                await svc.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd"
                });

                var user = await svc.GetById(1);

                Assert.That(user.ID, Is.EqualTo(1));
                Assert.That(user.Username, Is.EqualTo("newuser"));
                Assert.That(user.Password, Is.EqualTo("pwd"));
                Assert.That(user.Name, Is.EqualTo("user"));
                Assert.That(user.Email, Is.EqualTo("any@email.com"));
                Assert.That(user.Role.Count(), Is.EqualTo(1));
                Assert.That(user.Role.ElementAt(0), Is.EqualTo(UserRole.Public));
            }
        }

        [Test]
        public async Task TestUserServiceShouldGetUserByIdHappyPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbPostRepo);

                await svc.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = await svc.GetById(1);

                Assert.That(user.ID, Is.EqualTo(1));
                Assert.That(user.Username, Is.EqualTo("newuser"));
                Assert.That(user.Password, Is.EqualTo("pwd"));
                Assert.That(user.Name, Is.EqualTo("user"));
                Assert.That(user.Email, Is.EqualTo("any@email.com"));
                Assert.That(user.Role.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public async Task TestUserServiceShouldGetUserByUsernameHappyPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbPostRepo);

                await svc.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = await svc.GetByUsername("newuser");

                Assert.That(user.ID, Is.EqualTo(1));
                Assert.That(user.Username, Is.EqualTo("newuser"));
                Assert.That(user.Password, Is.EqualTo("pwd"));
                Assert.That(user.Name, Is.EqualTo("user"));
                Assert.That(user.Email, Is.EqualTo("any@email.com"));
                Assert.That(user.Role.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public async Task TestUserServiceShouldAuthenticateHappyPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbPostRepo);

                await svc.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = svc.Authenticate("newuser", "pwd");

                Assert.That(user.ID, Is.EqualTo(1));
                Assert.That(user.Username, Is.EqualTo("newuser"));
                Assert.That(user.Password, Is.EqualTo("pwd"));
                Assert.That(user.Name, Is.EqualTo("user"));
                Assert.That(user.Email, Is.EqualTo("any@email.com"));
                Assert.That(user.Role.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public async Task TestUserServiceShouldAuthenticateFailPath()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbPostRepo);

                await svc.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var user = svc.Authenticate("newuser", "wrong");

                Assert.IsNull(user);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task TestUserServiceShouldCheckHasRole(bool hasRole)
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options))
            {
                var dbPostRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbPostRepo);

                await svc.Save(new User()
                {
                    Name = "user",
                    Email = "any@email.com",
                    Username = "newuser",
                    Password = "pwd",
                    Role = new List<UserRole>() { UserRole.Writer }
                });

                var user = await svc.GetByUsername("newuser");

                var whichRole = UserRole.Editor;
                if (hasRole)
                    whichRole = UserRole.Writer;

                Assert.That(await svc.HasRole(user.ID, whichRole), Is.EqualTo(hasRole));

            }
        }

    }
}