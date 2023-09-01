using blog.common.Model;

namespace blog.api.Mapper
{
    public class MapperCommentInputToCommentDB : IMapper<CommentDB, CommentInput>
    {
        public CommentDB Map(CommentInput comment)
        {
            return new CommentDB()
            {
                Content = comment.Content
            };
        }
    }
}
