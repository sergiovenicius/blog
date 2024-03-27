using blog.api.Events;
using blog.api.Mapper;
using blog.api.Model;
using blog.common.Database;
using blog.common.Model;
using MediatR;

namespace blog.api.Repository
{

    public record CreatePostCommand : IRequest<PostDB>
    {
        public PostInput Post { get; init; }
    }

    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDB>
    {
        private readonly DBContextBlog db;
        private readonly IMapper<PostDB, PostInput> _mapperPostInputToDB;
        private readonly CurrentUser _currentUser;

        public CreatePostCommandHandler(DBContextBlog db, 
            IMapper<PostDB, PostInput> mapperPostInputToDB,
            CurrentUser currentUser)
        {
            this.db = db;
            this._mapperPostInputToDB = mapperPostInputToDB;
            this._currentUser = currentUser;
        }

        public async Task<PostDB> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapperPostInputToDB.Map(request.Post);
            entity.OwnerId = _currentUser.Id;

            entity.AddDomainEvent(new PostCreatedEvent(entity));

            db.Posts.Add(entity);

            await db.SaveChangesAsync(cancellationToken);

            return entity;
        }
    }

}
