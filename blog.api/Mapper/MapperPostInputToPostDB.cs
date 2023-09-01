using blog.common.Model;

namespace blog.api.Mapper
{
    public class MapperPostInputToPostDB : IMapper<PostDB, PostInput>
    {
        public PostDB Map(PostInput post)
        {
            return new PostDB()
            {
                Content = post.Content,
                Title = post.Title
            };
        }
    }
}
