using blog.common.Model;

namespace blog.api.Events
{
    public class PostRejectedEvent : BaseEvent
    {
        public PostRejectedEvent(PostDB item)
        {
            Item = item;
        }

        public PostDB Item { get; }
    }
}
