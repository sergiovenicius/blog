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
    public class PostRepositoryTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task TestRepositoryShouldAddPost()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

                CurrentUser currentUser = new CurrentUser() { Id = 1 };

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };

                PostDB postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;

                var savedPost = await dbPostRepo.AddAsync(postDB);

                Assert.That(savedPost.ID, Is.EqualTo(1));
                Assert.That(savedPost.Title, Is.EqualTo(input.Title));
                Assert.That(savedPost.Content, Is.EqualTo(input.Content));
                Assert.That(savedPost.Status, Is.EqualTo(postDB.Status));
                Assert.That(savedPost.Comments.Count, Is.EqualTo(0));
                Assert.IsNull(savedPost.DatePublished);
                Assert.That(savedPost.OwnerId, Is.EqualTo(currentUser.Id));
            }
        }

        [Test]
        [TestCase(null, "a")]
        [TestCase("a", null)]
        [TestCase(null, null)]
        public void TestRepositoryShouldAddPostInvalidContentOrTitle(string title, string content)
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

                CurrentUser currentUser = new CurrentUser() { Id = 1 };

                PostInput input = new PostInput() { Title = title, Content = content };

                PostDB postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;

                Assert.ThrowsAsync<ValidationException>(async () => await dbPostRepo.AddAsync(postDB));
            }
        }

        [Test]
        public async Task TestRepositoryShouldListPosts()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

                CurrentUser currentUser = new CurrentUser() { Id = 1 };

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                PostDB postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                input = new PostInput() { Title = "title test 2", Content = "content test 2" };
                postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                input = new PostInput() { Title = "title test 3", Content = "content test 3" };
                postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                IEnumerable<PostDB> found = await dbPostRepo.ListAsync(new PostStatus[] { PostStatus.None } );

                Assert.That(found.Count(), Is.EqualTo(3));
                Assert.That(found.ElementAt(0).ID, Is.EqualTo(1));
                Assert.That(found.ElementAt(1).ID, Is.EqualTo(2));
                Assert.That(found.ElementAt(2).ID, Is.EqualTo(3));
            }
        }

        [Test]
        public async Task TestRepositoryShouldGetPostById()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

                CurrentUser currentUser = new CurrentUser() { Id = 1 };

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                PostDB postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                input = new PostInput() { Title = "title test 2", Content = "content test 2" };
                postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                input = new PostInput() { Title = "title test 3", Content = "content test 3" };
                postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                var found = await dbPostRepo.GetByIdAsync(2);

                Assert.That(found.ID, Is.EqualTo(2));
                Assert.That(found.Title, Is.EqualTo("title test 2"));
                Assert.That(found.Content, Is.EqualTo("content test 2"));
                Assert.That(found.Status, Is.EqualTo(PostStatus.None));
                Assert.That(found.Comments.Count, Is.EqualTo(0));
                Assert.IsNull(found.DatePublished);
                Assert.That(found.OwnerId, Is.EqualTo(currentUser.Id));
            }
        }

        [Test]
        public void TestRepositoryShouldGetPostByIdNotFound()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.GetByIdAsync(0));
                Assert.AreEqual(Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.GetByIdAsync(0)).Message, "Post not found");
            }
        }

        [Test]
        public async Task TestRepositoryShouldListByOwner()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

                CurrentUser currentUser = new CurrentUser() { Id = 1 };

                PostInput input = new PostInput() { Title = "title test - user 1", Content = "content test" };
                PostDB postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                currentUser.Id = 2;

                input = new PostInput() { Title = "title test 2 another user", Content = "content test 2" };
                postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                currentUser.Id = 1;

                input = new PostInput() { Title = "title test 3 - user 1", Content = "content test 3" };
                postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                var found = await dbPostRepo.ListByOwnerAsync(currentUser.Id);

                Assert.That(found.Count(), Is.EqualTo(2));
                Assert.That(found.ElementAt(0).ID, Is.EqualTo(1));
                Assert.That(found.ElementAt(1).ID, Is.EqualTo(3));
            }
        }

        [Test]
        public void TestRepositoryShouldListByOwnerNoPostFound()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

                Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.ListByOwnerAsync(1));
                Assert.AreEqual(Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.ListByOwnerAsync(1)).Message, "No posts found");
            }
        }

        [Test]
        public async Task TestRepositoryShouldEditPost()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

                CurrentUser currentUser = new CurrentUser() { Id = 1 };

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                PostDB postDB = mapper.Map(input);
                postDB.OwnerId = currentUser.Id;
                await dbPostRepo.AddAsync(postDB);

                var found = await dbPostRepo.GetByIdAsync(1);

                found.Content = "after edit content";
                found.Title = "after edit title";
                await dbPostRepo.EditAsync(found, null);

                found = await dbPostRepo.GetByIdAsync(1);

                Assert.That(found.ID, Is.EqualTo(1));
                Assert.That(found.Title, Is.EqualTo("after edit title"));
                Assert.That(found.Content, Is.EqualTo("after edit content"));
                Assert.That(found.Status, Is.EqualTo(PostStatus.None));
                Assert.That(found.Comments.Count, Is.EqualTo(0));
                Assert.IsNull(found.DatePublished);
                Assert.That(found.OwnerId, Is.EqualTo(currentUser.Id));
            }
        }

        [Test]
        public void TestRepositoryShouldEditPostNotFound()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, new Mock<IMediator>().Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

                CurrentUser currentUser = new CurrentUser() { Id = 1 };

                PostDB postDB = new PostDB();
                postDB.OwnerId = currentUser.Id;
                postDB.Content = "content";
                postDB.Title = "title";
                postDB.Status = PostStatus.None;

                Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.EditAsync(postDB, null));
                Assert.AreEqual(Assert.ThrowsAsync<NotFoundException>(async () => await dbPostRepo.EditAsync(postDB, null)).Message, "Post not found");
            }
        }
    }
}