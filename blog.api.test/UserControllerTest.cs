using blog.api.Mapper;
using blog.api.Model;
using blog.common.Database;
using blog.common.Model;
using blog.common.Repository;
using blog.common.Service;
using Blog.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace blog.api.test
{
    public class UserControllerTest
    {
        [Test]
        public void Initialize()
        {
            var logger = new NullLogger<UserController>();
            var UserService = new UserService(new UserRepository(new DBContextBlog(new Microsoft.EntityFrameworkCore.DbContextOptions<DBContextBlog>(), new Mock<IMediator>().Object)));

            Assert.AreEqual("userService", Assert.Throws<ArgumentNullException>(() => new UserController(logger, null)).ParamName);
            Assert.AreEqual("logger", Assert.Throws<ArgumentNullException>(() => new UserController(null, UserService)).ParamName);
        }

        [Test]
        public async Task ListUsersSuccess()
        {
            var logger = new NullLogger<UserController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options, new Mock<IMediator>().Object))
            {
                var dbuserRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbuserRepo);

                var controller = new UserController(logger, svc);
                var response = await controller.GetAsync();

                var okResult = response as OkObjectResult;

                // assert
                Assert.IsNotNull(okResult);
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public async Task ListUsersFail()
        {
            var logger = new NullLogger<UserController>();
            var currentUserService = new CurrentUser();

            var dbuserRepo = new UserRepository(null);

            UserService svc = new UserService(dbuserRepo);

            var controller = new UserController(logger, svc);
            var response = await controller.GetAsync();

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        
        [Test]
        public async Task GetUserByIdNotFound()
        {
            var logger = new NullLogger<UserController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options, new Mock<IMediator>().Object))
            {
                var dbuserRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbuserRepo);

                var controller = new UserController(logger, svc);
                var response = await controller.GetByIdAsync(1);

                var result = response as NotFoundObjectResult;

                // assert
                Assert.IsNotNull(result);
                Assert.That(result.StatusCode, Is.EqualTo(404));
            }
        }

        [Test]
        public async Task GetUserByIdFail()
        {
            var logger = new NullLogger<UserController>();
            var currentUserService = new CurrentUser();

            var dbuserRepo = new UserRepository(null);

            UserService svc = new UserService(dbuserRepo);

            var controller = new UserController(logger, svc);
            var response = await controller.GetByIdAsync(1);

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }


        [Test]
        public async Task GetUserByUsernameNotFound()
        {
            var logger = new NullLogger<UserController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options, new Mock<IMediator>().Object))
            {
                var dbuserRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbuserRepo);

                var controller = new UserController(logger, svc);
                var response = await controller.GetByUserNameAsync("user_test");

                var result = response as NotFoundObjectResult;

                // assert
                Assert.IsNotNull(result);
                Assert.That(result.StatusCode, Is.EqualTo(404));
            }
        }

        [Test]
        public async Task GetUserByUsernameFail()
        {
            var logger = new NullLogger<UserController>();
            var currentUserService = new CurrentUser();

            var dbuserRepo = new UserRepository(null);

            UserService svc = new UserService(dbuserRepo);

            var controller = new UserController(logger, svc);
            var response = await controller.GetByUserNameAsync("user_test");

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task AddUserSuccess()
        {
            var logger = new NullLogger<UserController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options, new Mock<IMediator>().Object))
            {
                var dbuserRepo = new UserRepository(dbcontext);

                UserService svc = new UserService(dbuserRepo);

                var controller = new UserController(logger, svc);
                var response = await controller.PostAsync(new User() 
                { 
                    Name = "user", Email = "any@email.com", Username = "newuser", Password = "pwd", 
                    Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
                });

                var result = response as OkObjectResult;

                // assert
                Assert.IsNotNull(result);
                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public async Task AddUserFail()
        {
            var logger = new NullLogger<UserController>();
            var currentUserService = new CurrentUser();

            var dbuserRepo = new UserRepository(null);

            UserService svc = new UserService(dbuserRepo);

            var controller = new UserController(logger, svc);
            var response = await controller.PostAsync(new User()
            {
                Name = "user",
                Email = "any@email.com",
                Username = "newuser",
                Password = "pwd",
                Role = new List<UserRole>() { UserRole.Public, UserRole.Writer, UserRole.Editor }
            });

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        

    }
}
