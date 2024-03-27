using MediatR;

namespace blog.api.Events
{
    public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
    {
        private readonly ILogger<PostCreatedEventHandler> _logger;

        public PostCreatedEventHandler(ILogger<PostCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("********************" +
                "\r\n*** {DateTime} Event Received: {DomainEvent}",
                DateTime.UtcNow.ToString(),
                notification.GetType().Name +
                "\r\n********************");

            return Task.CompletedTask;
        }
    }

}
