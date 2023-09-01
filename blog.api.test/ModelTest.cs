using blog.common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blog.api.test
{
    public class ModelTest
    {
        [Test]
        public void PostDBModel()
        {
            var post = new PostDB();

            Assert.That(post.ID, Is.EqualTo(0));
            Assert.That(post.Title, Is.EqualTo(string.Empty));
            Assert.That(post.Content, Is.EqualTo(string.Empty));
            Assert.That(post.DatePublished, Is.EqualTo(null));
            Assert.That(post.OwnerId, Is.EqualTo(0));
            Assert.That(post.Status, Is.EqualTo(PostStatus.None));
            Assert.That(post.Comments.Count(), Is.EqualTo(0));
        }

        [Test]
        public void PostInputModel()
        {
            var post = new PostInput();

            Assert.That(post.Title, Is.EqualTo(string.Empty));
            Assert.That(post.Content, Is.EqualTo(string.Empty));
        }

        [Test]
        public void UserModel()
        {
            var user = new User();

            Assert.That(user.ID, Is.EqualTo(0));
            Assert.That(user.Username, Is.EqualTo(string.Empty));
            Assert.That(user.Email, Is.EqualTo(string.Empty));
            Assert.That(user.Password, Is.EqualTo(string.Empty));
            Assert.That(user.Name, Is.EqualTo(string.Empty));
            Assert.That(user.Role.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ErrorMessageModel()
        {
            var message = new ErrorMessage("msg");

            Assert.That(message.Error, Is.EqualTo("msg"));
        }
    }
}
