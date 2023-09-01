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

                await controller.Post(new PostInput() { Title = "title", Content = "content" });
                await controller.Submit(1);
                await controller.Approve(1);

                var response = await controller.Get();

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
            var response = await controller.Get();

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

                await controller.Post(new PostInput() { Title = "title", Content = "content" });
                await controller.Submit(1);

                var response = await controller.GetPendingPosts();

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
            var response = await controller.GetPendingPosts();

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
                var response = await controller.GetById(1);

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
            var response = await controller.GetById(1);

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
                var response = await controller.GetByOwner();

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
            var response = await controller.GetByOwner();

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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });

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
            var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });

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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                response = await controller.Edit(postCreated.ID, new PostInput() { Title = postCreated.Title, Content = "editted post" });
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                response = await controller.Edit(1000, new PostInput() { Title = postCreated.Title, Content = "editted post" });
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                response = await controller.Submit(postCreated.ID);
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                await controller.Submit(postCreated.ID);

                response = await controller.Approve(postCreated.ID);
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                await controller.Submit(postCreated.ID);

                response = await controller.Reject(postCreated.ID, new CommentInput() { Content = "try-again" });
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                await controller.Submit(postCreated.ID);

                await controller.Approve(postCreated.ID);

                response = await controller.Comment(postCreated.ID, new CommentInput() { Content = "nice post!" });
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                response = await controller.Submit(10000);
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                await controller.Submit(postCreated.ID);

                response = await controller.Approve(10000);
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                await controller.Submit(postCreated.ID);

                response = await controller.Reject(1000000, new CommentInput() { Content = "try-again" });
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
                var response = await controller.Post(new PostInput() { Title = "title", Content = "content" });
                var result = response as OkObjectResult;

                var postCreated = result.Value as PostDB;

                await controller.Submit(postCreated.ID);

                await controller.Approve(postCreated.ID);

                response = await controller.Comment(1000000, new CommentInput() { Content = "nice post!" });
                var result400 = response as BadRequestObjectResult;

                // assert
                Assert.IsNotNull(result400);
                Assert.That(result400.StatusCode, Is.EqualTo(400));
            }
        }


    }
}
