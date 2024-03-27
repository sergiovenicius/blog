using blog.common.Model;

namespace blog.common.Service
{
    public interface IPostService
    {
        Task<IEnumerable<PostDB>> ListAsync(PostStatus[] status);

        Task<PostDB> GetByIdAsync(long userId, long id);

        Task<IEnumerable<PostDB>> ListByOwnerAsync(long userId);

        Task<PostDB> EditAsync(long postId, PostInput post);

        Task<PostDB> SubmitAsync(long postId);

        Task<PostDB> ApproveAsync(long postId);

        Task<PostDB> RejectAsync(long postId, CommentInput comment);

        Task<CommentDB> CommentAsync(long postId, CommentInput comment);
    }
}
