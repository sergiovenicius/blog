using blog.common.Model;

namespace blog.api.Events
{
    public class PostCreatedEvent : BaseEvent
    {
        public PostCreatedEvent(PostDB item)
        {
            Item = item;
        }

        public PostDB Item { get; }
    }
}
