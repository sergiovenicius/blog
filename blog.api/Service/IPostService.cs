using blog.common.Model;

namespace blog.common.Service
{
    public interface IPostService
    {
        Task<IEnumerable<PostDB>> List(PostStatus[] status);

        Task<PostDB> GetById(long id);

        Task<IEnumerable<PostDB>> ListByOwner(long userId);

        Task<PostDB> Add(PostInput post);

        Task<PostDB> Edit(long postId, PostInput post);

        Task<PostDB> Submit(long postId);

        Task<PostDB> Approve(long postId);

        Task<PostDB> Reject(long postId, CommentInput comment);

        Task<CommentDB> Comment(long postId, CommentInput comment);
    }
}
