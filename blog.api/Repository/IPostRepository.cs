using blog.common.Model;

namespace blog.common.Repository
{
    public interface IPostRepository
    {
        Task<IEnumerable<PostDB>> ListAsync(PostStatus[] status);
        Task<PostDB> GetByIdAsync(long id);
        Task<IEnumerable<PostDB>> ListByOwnerAsync(long ownerId);

        Task<PostDB> AddAsync(PostDB post);
        Task<PostDB> EditAsync(PostDB post, CommentDB? comment);
    }
}
