using blog.common.Model;

namespace blog.common.Repository
{
    public interface IPostRepository
    {
        Task<IEnumerable<PostDB>> List(PostStatus[] status);
        Task<PostDB> GetById(long id);
        Task<IEnumerable<PostDB>> ListByOwner(long ownerId);

        Task<PostDB> Add(PostDB post);
        Task<PostDB> Edit(PostDB post, CommentDB? comment);
    }
}
