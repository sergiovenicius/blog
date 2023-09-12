using blog.api.Mapper;
using blog.api.Model;
using blog.common.Database;
using blog.common.Model;
using blog.common.Repository;
using blog.common.Service;
using Blog.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var postService = new PostService(new PostRepository(new DBContextBlog(new Microsoft.EntityFrameworkCore.DbContextOptions<DBContextBlog>())),
                new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            Assert.AreEqual("postService", Assert.Throws<ArgumentNullException>(() => new PostController(logger, null, currentUserService)).ParamName);
            Assert.AreEqual("logger", Assert.Throws<ArgumentNullException>(() => new PostController(null, postService, currentUserService)).ParamName);
            Assert.AreEqual("currentUser", Assert.Throws<ArgumentNullException>(() => new PostController(logger, postService, null)).ParamName);
        }

        [Test]
        public async Task ListPostsSuccess()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);

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
        }

        [Test]
        public async Task ListPostsFail()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService);
            var response = await controller.GetAsync();

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task ListPendingPostsSuccess()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);

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
        }

        [Test]
        public async Task ListPendingPostsFail()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService);
            var response = await controller.GetPendingPostsAsync();

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetPostByIdNotFound()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService);
            var response = await controller.GetByIdAsync(1);

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetMinePostsNotFound()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService);
            var response = await controller.GetByOwnerAsync();

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task AddPostSuccess()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();

            var dbPostRepo = new PostRepository(null);

            PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), currentUserService);

            var controller = new PostController(logger, svc, currentUserService);
            var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });

            var badRequestResult = response as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(badRequestResult);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task EditPostSuccess()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
                var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                response = await controller.EditAsync(postCreated.ID, new PostInput() { Title = postCreated.Title, Content = "editted post" });
                result = response as OkObjectResult;

                // assert
                Assert.IsNotNull(result);
                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public async Task EditPostFail()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
                var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                response = await controller.SubmitAsync(postCreated.ID);
                result = response as OkObjectResult;

                // assert
                Assert.IsNotNull(result);
                Assert.That(result.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public async Task ApprovePostSuccess()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
        }

        [Test]
        public async Task RejectPostSuccess()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
        }

        [Test]
        public async Task CommentPostSuccess()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
        }






        [Test]
        public async Task SubmitPostFail()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
                var response = await controller.PostAsync(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                response = await controller.SubmitAsync(10000);
                var result400 = response as BadRequestObjectResult;

                // assert
                Assert.IsNotNull(result400);
                Assert.That(result400.StatusCode, Is.EqualTo(400));
            }
        }

        [Test]
        public async Task ApprovePostFail()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
        }

        [Test]
        public async Task RejectPostFail()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
        }

        [Test]
        public async Task CommentPostFail()
        {
            var logger = new NullLogger<PostController>();
            var currentUserService = new CurrentUser();
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var controller = new PostController(logger, svc, currentUserService);
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
}
