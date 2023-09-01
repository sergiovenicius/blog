using blog.api.Mapper;
using blog.common.Model;

namespace blog.api.test
{
    public class MapTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestShouldMapPostInputToPostDB()
        {
            IMapper<PostDB, PostInput> mapper = new MapperPostInputToPostDB();

            PostInput input = new PostInput
            {
                Title = "Test Title",
                Content = "Test Content"
            };

            var postDB = mapper.Map(input);

            Assert.That(postDB.Title, Is.EqualTo(input.Title));
            Assert.That(postDB.Content, Is.EqualTo(input.Content));
            Assert.That(postDB.Status, Is.EqualTo(PostStatus.None));
            Assert.IsNotNull(postDB.Comments);
            Assert.That(postDB.Comments.Count, Is.EqualTo(0));
            Assert.IsNull(postDB.DatePublished);
        }

        [Test]
        public void TestShouldMapCommentInputToCommentDB()
        {
            IMapper<CommentDB, CommentInput> mapper = new MapperCommentInputToCommentDB();

            CommentInput input = new CommentInput
            {
                Content = "Test Comment Content"
            };

            var commentDB = mapper.Map(input);

            Assert.That(commentDB.Content, Is.EqualTo(input.Content));
            Assert.That(commentDB.Type, Is.EqualTo(CommentType.Normal));
        }
    }
}