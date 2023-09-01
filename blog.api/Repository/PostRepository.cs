using blog.common.Database;
using blog.common.Exceptions;
using blog.common.Model;
using Microsoft.EntityFrameworkCore;

namespace blog.common.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly DBContextBlog db;
        public PostRepository(DBContextBlog db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<PostDB>> List(PostStatus[] status)
        {
            return await db.Posts.AsNoTracking().Where(r => status.Contains(r.Status)).Include(o => o.Comments.Where(c => c.Type != CommentType.Reject)).ToListAsync();
        }

        public async Task<PostDB> GetById(long id)
        {
            var post = await db.Posts.AsNoTracking().Where(r => r.ID == id).Include(o => o.Comments).FirstOrDefaultAsync();
            if (post == null)
                throw new NotFoundException("Post not found");
            return post;
        }

        public async Task<PostDB> Add(PostDB post)
        {
            var newPost = db.Posts.Add(post);

            await db.SaveChangesAsync(CancellationToken.None);

            return newPost.Entity;
        }
        public async Task<PostDB> Edit(PostDB post, CommentDB? comment)
        {
            var existingPost = db.Posts.Where(r => r.ID == post.ID).Include(c => c.Comments).FirstOrDefault();

            if (existingPost == null)
                throw new NotFoundException("Post not found");

            existingPost.Content = post.Content;
            existingPost.Title = post.Title;
            existingPost.Status = post.Status;
            existingPost.DatePublished = post.DatePublished;
            if (comment != null)
            {
                existingPost.Comments.Add(comment);
            }

            await db.SaveChangesAsync(CancellationToken.None);

            return existingPost;
        }


        public async Task<IEnumerable<PostDB>> ListByOwner(long ownerId)
        {
            var posts = await db.Posts.AsNoTracking().Where(r => r.OwnerId == ownerId).Include(p => p.Comments).ToListAsync();
            if (posts.Count == 0)
                throw new NotFoundException("No posts found");

            return posts;
        }
    }
}
