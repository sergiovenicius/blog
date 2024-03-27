using blog.api.Mapper;
using blog.api.Model;
using blog.common.Database;
using blog.common.Model;
using blog.common.Repository;
using blog.common.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using blog.common.Exceptions;
using MediatR;
using Moq;

namespace blog.api.test
{
    public class PostServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }
        /*
        [Test]
        public async Task TestPostServiceShouldSavePost()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object)
            {
                PostRepository repo = new PostRepository(dbcontext);

                PostService svc = new PostService(repo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };

                var savedPost = await svc.AddAsync(input);

                Assert.That(savedPost.ID, Is.EqualTo(1));
                Assert.That(savedPost.Title, Is.EqualTo(input.Title));
                Assert.That(savedPost.Content, Is.EqualTo(input.Content));
                Assert.That(savedPost.Status, Is.EqualTo(PostStatus.None));
                Assert.That(savedPost.Comments.Count, Is.EqualTo(0));
                Assert.IsNull(savedPost.DatePublished);
                Assert.That(savedPost.OwnerId, Is.EqualTo(1));
            }
        }

        [Test]
        public async Task TestPostServiceShouldListPosts()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                PostRepository repo = new PostRepository(dbcontext);

                PostService svc = new PostService(repo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                var saved1 = await svc.AddAsync(input);

                input = new PostInput() { Title = "title test 2", Content = "content test 2" };
                var saved2 = await svc.AddAsync(input);

                var PostList = await svc.ListAsync(new PostStatus[] { PostStatus.None });

                Assert.That(2, Is.EqualTo(PostList.Count()));
                Assert.That(PostList.ElementAt(0).ID, Is.EqualTo(saved1.ID));
                Assert.That(PostList.ElementAt(1).ID, Is.EqualTo(saved2.ID));
            }
        }

        [Test]
        public async Task TestPostServiceShouldGetPostById()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                PostRepository repo = new PostRepository(dbcontext);

                PostService svc = new PostService(repo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);

                var post = await svc.GetByIdAsync(1, 1);

                Assert.That(post.ID, Is.EqualTo(1));
                Assert.That(post.Title, Is.EqualTo(input.Title));
                Assert.That(post.Content, Is.EqualTo(input.Content));
            }
        }
        */
        [Test]
        public void TestPostServiceShouldGetPostByIdNotFound()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                PostRepository repo = new PostRepository(dbcontext);

                PostService svc = new PostService(repo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                Assert.ThrowsAsync<NotFoundException>(async () => await svc.GetByIdAsync(1, 1));
            }
        }
        /*
        [Test]
        [TestCase(null, "a")]
        [TestCase("a", null)]
        [TestCase(null, null)]
        public void TestServiceShouldAddPostInvalidContentOrTitle(string title, string content)
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var repo = new PostRepository(dbcontext);

                PostService svc = new PostService(repo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = title, Content = content };

                Assert.ThrowsAsync<ValidationException>(async () => await svc.AddAsync(input));
            }
        }
        
        [Test]
        public async Task TestServiceShouldListByOwner()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var repo = new PostRepository(dbcontext);

                PostService svc1 = new PostService(repo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test - user 1", Content = "content test" };
                await svc1.AddAsync(input);

                PostService svc2 = new PostService(repo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 2 });

                input = new PostInput() { Title = "title test 2 another user", Content = "content test 2" };
                await svc2.AddAsync(input);

                
                input = new PostInput() { Title = "title test 3 - user 1", Content = "content test 3" };
                await svc1.AddAsync(input);

                //list customer1 from svc2 (must not make difference)
                var found = await svc2.ListByOwnerAsync(1);
                Assert.That(found.Count(), Is.EqualTo(2));
                Assert.That(found.ElementAt(0).ID, Is.EqualTo(1));
                Assert.That(found.ElementAt(1).ID, Is.EqualTo(3));

                //list customer2 from svc1 (must not make difference)
                found = await svc1.ListByOwnerAsync(2);
                Assert.That(found.Count(), Is.EqualTo(1));
                Assert.That(found.ElementAt(0).ID, Is.EqualTo(2));
            }
        }
        */

        [Test]
        public void TestServiceShouldListByOwnerNoPostFound()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                Assert.ThrowsAsync<NotFoundException>(async () => await svc.ListByOwnerAsync(1));
            }
        }
        /*
        [Test]
        public async Task TestServiceShouldEditPost()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);

                var found = await svc.GetByIdAsync(1, 1);

                input.Content = "after edit content";
                input.Title = "after edit title";
                await svc.EditAsync(found.ID, input);

                found = await svc.GetByIdAsync(1, found.ID);

                Assert.That(found.ID, Is.EqualTo(1));
                Assert.That(found.Title, Is.EqualTo("after edit title"));
                Assert.That(found.Content, Is.EqualTo("after edit content"));
                Assert.That(found.Status, Is.EqualTo(PostStatus.None));
                Assert.That(found.Comments.Count, Is.EqualTo(0));
                Assert.IsNull(found.DatePublished);
                Assert.That(found.OwnerId, Is.EqualTo(1));
            }
        }
        */
        [Test]
        public void TestServiceShouldEditPostNotFound()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput();
                input.Content = "content after edit?";
                input.Title = "title after edit?";

                Assert.ThrowsAsync<NotFoundException>(async () => await svc.EditAsync(1, input));
            }
        }
        /*
        [Test]
        public async Task TestServiceShouldEditPostEvenRejected()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);

                var found = await svc.GetByIdAsync(1, 1);

                await svc.SubmitAsync(found.ID);

                await svc.RejectAsync(found.ID, new CommentInput() { Content = "try-again" });

                input.Content = "after edit content";
                input.Title = "after edit title";
                await svc.EditAsync(found.ID, input);

                found = await svc.GetByIdAsync(1, found.ID);

                Assert.That(found.ID, Is.EqualTo(1));
                Assert.That(found.Title, Is.EqualTo("after edit title"));
                Assert.That(found.Content, Is.EqualTo("after edit content"));
                Assert.That(found.Status, Is.EqualTo(PostStatus.Rejected));
                Assert.That(found.Comments.Count, Is.EqualTo(1));
                Assert.IsNull(found.DatePublished);
                Assert.That(found.OwnerId, Is.EqualTo(1));
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task TestServiceShouldEditPostCannotBecauseNotNoneAndNotRejected(bool mustApprove)
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);

                var found = await svc.GetByIdAsync(1, 1);

                await svc.SubmitAsync(found.ID);

                if (mustApprove)
                    await svc.ApproveAsync(found.ID);

                input.Content = "after edit content";
                input.Title = "after edit title";
                
                Assert.ThrowsAsync<Exception>(async () => await svc.EditAsync(found.ID, input));
                Assert.IsTrue(Assert.ThrowsAsync<Exception>(async () => await svc.EditAsync(found.ID, input)).Message.StartsWith("Cannot edit this post as its status is"));
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task TestServiceShouldSubmitPostCannotBecauseNotNoneAndNotRejected(bool mustApprove)
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);

                var found = await svc.GetByIdAsync(1, 1);

                await svc.SubmitAsync(found.ID);

                if (mustApprove)
                    await svc.ApproveAsync(found.ID);

                input.Content = "after edit content";
                input.Title = "after edit title";

                Assert.ThrowsAsync<Exception>(async () => await svc.SubmitAsync(found.ID));
                Assert.IsTrue(Assert.ThrowsAsync<Exception>(async () => await svc.SubmitAsync(found.ID)).Message.StartsWith("Cannot submit this post as its status is"));
            }
        }

        [Test]
        public async Task TestServiceShouldApprovePostCannotBecauseStatusNone()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var foundNone = await svc.GetByIdAsync(1, 1);

                Assert.ThrowsAsync<Exception>(async () => await svc.ApproveAsync(foundNone.ID));
                Assert.IsTrue(Assert.ThrowsAsync<Exception>(async () => await svc.ApproveAsync(foundNone.ID)).Message.StartsWith("Cannot approve this post as its status is"));
            }
        }

        [Test]
        public async Task TestServiceShouldApprovePostCannotBecauseStatusApproved()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var foundAlreadyApproved = await svc.GetByIdAsync(1, 1);
                await svc.SubmitAsync(foundAlreadyApproved.ID);
                await svc.ApproveAsync(foundAlreadyApproved.ID);

                Assert.ThrowsAsync<Exception>(async () => await svc.ApproveAsync(foundAlreadyApproved.ID));
                Assert.IsTrue(Assert.ThrowsAsync<Exception>(async () => await svc.ApproveAsync(foundAlreadyApproved.ID)).Message.StartsWith("Cannot approve this post as its status is"));
            }
        }

        [Test]
        public async Task TestServiceShouldApprovePostCannotBecauseStatusRejected()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var foundRejected = await svc.GetByIdAsync(1, 1);
                await svc.SubmitAsync(foundRejected.ID);
                await svc.RejectAsync(foundRejected.ID, new CommentInput() { Content = "try-again" });

                Assert.ThrowsAsync<Exception>(async () => await svc.ApproveAsync(foundRejected.ID));
                Assert.IsTrue(Assert.ThrowsAsync<Exception>(async () => await svc.ApproveAsync(foundRejected.ID)).Message.StartsWith("Cannot approve this post as its status is"));
            }
        }


        [Test]
        public async Task TestServiceShouldRejectPostCannotBecauseStatusNone()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var foundNone = await svc.GetByIdAsync(1, 1);

                Assert.ThrowsAsync<Exception>(async () => await svc.RejectAsync(foundNone.ID, new CommentInput() { Content = "try-again" }));
                Assert.IsTrue(Assert.ThrowsAsync<Exception>(async () => await svc.RejectAsync(foundNone.ID, new CommentInput() { Content = "try-again" })).Message.StartsWith("Cannot reject this post as its status is"));
            }
        }

        [Test]
        public async Task TestServiceShouldRejectPostCannotBecauseStatusApproved()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var foundAlreadyApproved = await svc.GetByIdAsync(1, 1);
                await svc.SubmitAsync(foundAlreadyApproved.ID);
                await svc.ApproveAsync(foundAlreadyApproved.ID);

                Assert.ThrowsAsync<Exception>(async () => await svc.RejectAsync(foundAlreadyApproved.ID, new CommentInput() { Content = "try-again" }));
                Assert.IsTrue(Assert.ThrowsAsync<Exception>(async () => await svc.RejectAsync(foundAlreadyApproved.ID, new CommentInput() { Content = "try-again" })).Message.StartsWith("Cannot reject this post as its status is"));
            }
        }

        [Test]
        public async Task TestServiceShouldRejectPostWithCommentContentIsEmpty()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var found = await svc.GetByIdAsync(1, 1);
                await svc.SubmitAsync(found.ID);

                Assert.ThrowsAsync<Exception>(async () => await svc.RejectAsync(found.ID, new CommentInput() { Content = "" }));
                Assert.AreEqual("You must provide a comment when rejecting a post.", Assert.ThrowsAsync<Exception>(async () => await svc.RejectAsync(found.ID, new CommentInput() { Content = "" })).Message);
            }
        }

        [Test]
        public async Task TestServiceShouldRejectPostCannotBecauseStatusRejected()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var foundRejected = await svc.GetByIdAsync(1, 1);
                await svc.SubmitAsync(foundRejected.ID);
                await svc.RejectAsync(foundRejected.ID, new CommentInput() { Content = "try-again" });

                Assert.ThrowsAsync<Exception>(async () => await svc.RejectAsync(foundRejected.ID, new CommentInput() { Content = "try-again" }));
                Assert.IsTrue(Assert.ThrowsAsync<Exception>(async () => await svc.RejectAsync(foundRejected.ID, new CommentInput() { Content = "try-again" })).Message.StartsWith("Cannot reject this post as its status is"));
            }
        }

        [Test]
        public async Task TestServiceShouldAddCommentToPublishedPost()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var found = await svc.GetByIdAsync(1, 1);
                await svc.SubmitAsync(found.ID);
                await svc.ApproveAsync(found.ID);

                var comment1 = await svc.CommentAsync(found.ID, new CommentInput() { Content = "nice" });
                var comment2 = await svc.CommentAsync(found.ID, new CommentInput() { Content = "great" });

                found = await svc.GetByIdAsync(1, 1);

                Assert.That(found.Comments.Count(), Is.EqualTo(2));
                Assert.That(found.Comments.ElementAt(0).ID, Is.EqualTo(1));
                Assert.That(found.Comments.ElementAt(1).ID, Is.EqualTo(2));
                Assert.That(found.Comments.ElementAt(0).PostId, Is.EqualTo(1));
                Assert.That(found.Comments.ElementAt(1).PostId, Is.EqualTo(1));
                Assert.That(found.Comments.ElementAt(0).Content, Is.EqualTo("nice"));
                Assert.That(found.Comments.ElementAt(1).Content, Is.EqualTo("great"));
                Assert.That(found.Comments.ElementAt(0).Type, Is.EqualTo(CommentType.Normal));
                Assert.That(found.Comments.ElementAt(1).Type, Is.EqualTo(CommentType.Normal));
                Assert.That(found.Comments.ElementAt(0).DatePublished, Is.EqualTo(comment1.DatePublished));
                Assert.That(found.Comments.ElementAt(1).DatePublished, Is.EqualTo(comment2.DatePublished));

            }
        }

        [Test]
        public async Task TestServiceShouldAddCommentCannotBecauseStatusNone()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var found = await svc.GetByIdAsync(1, 1);

                Assert.ThrowsAsync<Exception>(async () => await svc.CommentAsync(found.ID, new CommentInput() { Content = "nice" }));
                Assert.AreEqual("Cannot comment a post that is not published.", Assert.ThrowsAsync<Exception>(async () => await svc.CommentAsync(found.ID, new CommentInput() { Content = "nice" })).Message);

            }
        }

        [Test]
        public async Task TestServiceShouldAddCommentCannotBecauseStatusPendingApproval()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var found = await svc.GetByIdAsync(1, 1);
                await svc.SubmitAsync(found.ID);

                Assert.ThrowsAsync<Exception>(async () => await svc.CommentAsync(found.ID, new CommentInput() { Content = "nice" }));

            }
        }

        [Test]
        public async Task TestServiceShouldAddCommentCannotBecauseStatusRejected()
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                var dbPostRepo = new PostRepository(dbcontext);

                PostService svc = new PostService(dbPostRepo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                var input = new PostInput() { Title = "title test", Content = "content test" };
                await svc.AddAsync(input);
                var found = await svc.GetByIdAsync(1, 1);
                await svc.SubmitAsync(found.ID);
                await svc.RejectAsync(found.ID, new CommentInput() { Content = "try-again" });

                Assert.ThrowsAsync<Exception>(async () => await svc.CommentAsync(found.ID, new CommentInput() { Content = "nice" }));

            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task TestPostServiceShouldGetPostByIdWithOwnUser(bool ownUser)
        {
            using (var dbcontext = new DBContextBlog(new DbContextOptionsBuilder<DBContextBlog>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options, (new Mock<IMediator>()).Object))
            {
                PostRepository repo = new PostRepository(dbcontext);

                PostService svc = new PostService(repo, new MapperCommentInputToCommentDB(), new MapperPostInputToPostDB(), new CurrentUser() { Id = 1 });

                PostInput input = new PostInput() { Title = "title test", Content = "content test" };
                var post = await svc.AddAsync(input);

                await svc.SubmitAsync(post.ID);
                await svc.RejectAsync(post.ID, new CommentInput() { Content = "rejected" });
                await svc.SubmitAsync(post.ID);
                await svc.ApproveAsync(post.ID);
                await svc.CommentAsync(post.ID, new CommentInput() { Content = "ok" });
                await svc.CommentAsync(post.ID, new CommentInput() { Content = "ok2" });

                post = await svc.GetByIdAsync(ownUser ? 1 : 2, 1);

                Assert.That(post.ID, Is.EqualTo(1));
                Assert.That(post.Title, Is.EqualTo(input.Title));
                Assert.That(post.Content, Is.EqualTo(input.Content));
                Assert.That(post.Comments.Count(), Is.EqualTo(ownUser ? 3 : 2));
                if (ownUser)
                    Assert.IsTrue(post.Comments.Count(r => r.Type == CommentType.Reject) == 1);
                else
                    Assert.IsTrue(post.Comments.Count(r => r.Type != CommentType.Reject) == 2);
            }
        }
        */

    }
}