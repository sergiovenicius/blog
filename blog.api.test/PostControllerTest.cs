using blog.api.Events;
using blog.api.Mapper;
using blog.api.Model;
using blog.api.Repository;
using blog.common.Database;
using blog.common.Model;
using blog.common.Repository;
using blog.common.Service;
using Blog.Controllers;
using MediatR;
using MediatR.Registration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace blog.api.test
{
    public class PostControllerTest
    {
        [Test]
        public void Initialize()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            var postService = new PostService(new PostRepository(new DBContextBlog(new Microsoft.EntityFrameworkCore.DbContextOptions<DBContextBlog>(), new Mock<IMediator>().Object)),
                new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            Assert.AreEqual("postService", Assert.Throws<ArgumentNullException>(() => new PostController(logger, null, currentUserService)).ParamName);
            Assert.AreEqual("logger", Assert.Throws<ArgumentNullException>(() => new PostController(null, postService, currentUserService)).ParamName);
            Assert.AreEqual("currentUser", Assert.Throws<ArgumentNullException>(() => new PostController(logger, postService, null)).ParamName);
        }

        [Test]
        public async Task ListPostsSuccess()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);

            await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            await controller.SubmitAsync(1);
            await controller.ApproveAsync(1);

            var response = await controller.GetAsync();

            var okResult = response as OkObjectResult;

            var objectResponse = okResult.Value as List<PostDB>;

            //Assert
            Assert.That(objectResponse.Count, Is.EqualTo(1));
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            
        }

        [Test]
        public async Task ListPostsFail()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(null, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.GetAsync();

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task ListPendingPostsSuccess()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);

            await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            await controller.SubmitAsync(1);

            var response = await controller.GetPendingPostsAsync();

            var okResult = response as OkObjectResult;

            var objectResponse = okResult.Value as List<PostDB>;

            // assert
            Assert.That(objectResponse.Count, Is.EqualTo(1));
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

        }

        private static ServiceProvider IncludeRequiredServices()
        {
            var services = new ServiceCollection();
            var serviceProvider = services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(PostCreatedEventHandler).Assembly))
                .AddDbContext<DBContextBlog>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IPostRepository, PostRepository>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IPostService, PostService>()
                .AddScoped<IMapper<PostDB, PostInput>, MapperPostInputToPostDB>()
                .AddScoped<IMapper<CommentDB, CommentInput>, MapperCommentInputToCommentDB>()
                .AddScoped<CurrentUser>()
                .AddLogging(loggerBuilder =>
                {
                    loggerBuilder.ClearProviders();
                    loggerBuilder.AddConsole();
                })
                .BuildServiceProvider();
            return serviceProvider;
        }

        [Test]
        public async Task ListPendingPostsFail()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PostDB());

            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator.Object);
            var response = await controller.GetPendingPostsAsync();

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetPostByIdNotFound()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PostDB());
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options, mediator.Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService, mediator.Object);
                var response = await controller.GetByIdAsync(1);

                var result = response as NotFoundObjectResult;

                // assert
                Assert.IsNotNull(result);
                Assert.That(result.StatusCode, Is.EqualTo(404));
            }
        }

        [Test]
        public async Task GetPostByIdFail()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PostDB());

            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator.Object);
            var response = await controller.GetByIdAsync(1);

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetMinePostsNotFound()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PostDB());
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options, mediator.Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService, mediator.Object);
                var response = await controller.GetByOwnerAsync();

                var result = response as NotFoundObjectResult;

                // assert
                Assert.IsNotNull(result);
                Assert.That(result.StatusCode, Is.EqualTo(404));
            }
        }

        [Test]
        public async Task GetMinePostsFail()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PostDB());

            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator.Object);
            var response = await controller.GetByOwnerAsync();

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task AddPostSuccess()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PostDB());

            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options, mediator.Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService, mediator.Object);
                var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });

                var result = response as OkObjectResult;

                // assert
                Assert.IsNotNull(result);
                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public async Task AddPostFail()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("error"));

            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator.Object);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task EditPostSuccess()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            var postCreated = result.Value as PostDB;

            response = await controller.EditAsync(postCreated.ID, new PostInput() { Title = postCreated.Title, Content = "editted post" });
            result = response as OkObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task EditPostFail()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PostDB());
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options, mediator.Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService, mediator.Object);
                var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                response = await controller.EditAsync(1000, new PostInput() { Title = postCreated.Title, Content = "editted post" });
                var result400 = response as BadRequestObjectResult;

                // assert
                Assert.IsNotNull(result400);
                Assert.That(result400.StatusCode, Is.EqualTo(400));
            }
        }

        [Test]
        public async Task SubmitPostSuccess()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            var postCreated = result.Value as PostDB;

            response = await controller.SubmitAsync(postCreated.ID);
            result = response as OkObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        
        }

        [Test]
        public async Task ApprovePostSuccess()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            var postCreated = result.Value as PostDB;

            await controller.SubmitAsync(postCreated.ID);

            response = await controller.ApproveAsync(postCreated.ID);
            result = response as OkObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        
        }

        [Test]
        public async Task RejectPostSuccess()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            var postCreated = result.Value as PostDB;

            await controller.SubmitAsync(postCreated.ID);

            response = await controller.RejectAsync(postCreated.ID, new CommentInput() { Content = "try-again" });
            result = response as OkObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        
        }

        [Test]
        public async Task CommentPostSuccess()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            var postCreated = result.Value as PostDB;

            await controller.SubmitAsync(postCreated.ID);

            await controller.ApproveAsync(postCreated.ID);

            response = await controller.CommentAsync(postCreated.ID, new CommentInput() { Content = "nice post!" });
            result = response as OkObjectResult;

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        
        }

        [Test]
        public async Task SubmitPostFail()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            response = await controller.SubmitAsync(10000);
            var result400 = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(result400);
            Assert.That(result400.StatusCode, Is.EqualTo(400));
        
        }

        [Test]
        public async Task ApprovePostFail()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            var postCreated = result.Value as PostDB;

            await controller.SubmitAsync(postCreated.ID);

            response = await controller.ApproveAsync(10000);
            var result400 = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(result400);
            Assert.That(result400.StatusCode, Is.EqualTo(400));
        
        }

        [Test]
        public async Task RejectPostFail()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            var postCreated = result.Value as PostDB;

            await controller.SubmitAsync(postCreated.ID);

            response = await controller.RejectAsync(1000000, new CommentInput() { Content = "try-again" });
            var result400 = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(result400);
            Assert.That(result400.StatusCode, Is.EqualTo(400));
        
        }

        [Test]
        public async Task CommentPostFail()
        {
            ServiceProvider serviceProvider = IncludeRequiredServices();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbcontext = serviceProvider.GetRequiredService<DBContextBlog>();
            var logger = new NullLogger<PostController>();
            var currentUserService = serviceProvider.GetRequiredService<CurrentUser>();
            var dbPostRepo = serviceProvider.GetRequiredService<IPostRepository>();
            var mapperComments = serviceProvider.GetRequiredService<IMapper<CommentDB, CommentInput>>();
            var mapperPosts = serviceProvider.GetRequiredService<IMapper<PostDB, PostInput>>();

            currentUserService.Id = 1;

            PostService svc = new PostService(dbPostRepo, mapperComments, mapperPosts, currentUserService);

            var controller = new PostController(logger, svc, currentUserService, mediator);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
            var result = response as OkObjectResult;

            var postCreated = result.Value as PostDB;

            await controller.SubmitAsync(postCreated.ID);

            await controller.ApproveAsync(postCreated.ID);

            response = await controller.CommentAsync(1000000, new CommentInput() { Content = "nice post!" });
            var result400 = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(result400);
            Assert.That(result400.StatusCode, Is.EqualTo(400));
        
        }


    }
}
