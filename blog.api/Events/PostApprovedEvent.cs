using blog.common.Model;

namespace blog.api.Events
{
    public class PostApprovedEvent : BaseEvent
    {
        public PostApprovedEvent(PostDB item)
        {
            Item = item;
        }

        public PostDB Item { get; }
    }
}
