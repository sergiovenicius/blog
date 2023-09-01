using blog.api.Mapper;
using blog.api.Model;
using blog.common.Model;
using blog.common.Repository;

namespace blog.common.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper<CommentDB, CommentInput> _mapperCommentInputToDB;
        private readonly IMapper<PostDB, PostInput> _mapperPostInputToDB;
        private readonly CurrentUser _currentUser;

        public PostService(IPostRepository postRepository,
            IMapper<CommentDB, CommentInput> mapperCommentInputToDB,
            IMapper<PostDB, PostInput> mapperPostInputToDB,
            CurrentUser currentUser)
        {
            _postRepository = postRepository;
            _mapperCommentInputToDB = mapperCommentInputToDB;
            _mapperPostInputToDB = mapperPostInputToDB;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<PostDB>> List(PostStatus[] status)
        {
            return await _postRepository.List(status);
        }

        public async Task<PostDB> GetById(long userId, long id)
        {
            var dbPost = await _postRepository.GetById(id);

            if (dbPost.Comments.Any())
            {
                if (dbPost.OwnerId != userId)
                    dbPost.Comments.RemoveAll(r => r.Type == CommentType.Reject);
            }

            return dbPost;
        }

        public async Task<PostDB> Add(PostInput post)
        {
            var postDB = _mapperPostInputToDB.Map(post);
            postDB.OwnerId = _currentUser.Id;

            var dbPost = await _postRepository.Add(postDB);
            return dbPost;
        }

        public async Task<IEnumerable<PostDB>> ListByOwner(long userId)
        {
            var dbPost = await _postRepository.ListByOwner(userId);

            return dbPost;
        }

        public async Task<PostDB> Edit(long postId, PostInput post)
        {
            var dbPost = await _postRepository.GetById(postId);

            if (dbPost.Status != PostStatus.Rejected
            && dbPost.Status != PostStatus.None)
                throw new Exception($"Cannot edit this post as its status is {Enum.GetName(typeof(PostStatus), dbPost.Status)}.");

            dbPost.Title = post.Title;
            dbPost.Content = post.Content;
            dbPost = await _postRepository.Edit(dbPost, null);

            return dbPost;
        }

        public async Task<PostDB> Submit(long postId)
        {
            var dbPost = await _postRepository.GetById(postId);

            if (dbPost.Status != PostStatus.None
            && dbPost.Status != PostStatus.Rejected)
                throw new Exception($"Cannot submit this post as its status is {Enum.GetName(typeof(PostStatus), dbPost.Status)}.");

            dbPost.Status = PostStatus.Pending_Approval;
            dbPost = await _postRepository.Edit(dbPost, null);

            return dbPost;
        }

        public async Task<CommentDB> Comment(long postId, CommentInput comment)
        {
            var dbPost = await _postRepository.GetById(postId);

            if (dbPost.Status != PostStatus.Approved)
                throw new Exception("Cannot comment a post that is not published.");

            var commentDB = _mapperCommentInputToDB.Map(comment);
            commentDB.PostId = postId;

            await _postRepository.Edit(dbPost, commentDB);

            return commentDB;
        }

        public async Task<PostDB> Approve(long postId)
        {
            var dbPost = await _postRepository.GetById(postId);

            if (dbPost.Status != PostStatus.Pending_Approval)
                throw new Exception($"Cannot approve this post as its status is {Enum.GetName(typeof(PostStatus), dbPost.Status)}.");

            dbPost.Status = PostStatus.Approved;
            dbPost.DatePublished = DateTime.UtcNow;
            dbPost = await _postRepository.Edit(dbPost, null);

            return dbPost;
        }

        public async Task<PostDB> Reject(long postId, CommentInput comment)
        {
            var dbPost = await _postRepository.GetById(postId);

            if (dbPost.Status != PostStatus.Pending_Approval)
                throw new Exception($"Cannot reject this post as its status is {Enum.GetName(typeof(PostStatus), dbPost.Status)}.");

            if (string.IsNullOrEmpty(comment.Content))
                throw new Exception("You must provide a comment when rejecting a post.");

            dbPost.Status = PostStatus.Rejected;

            var commentDB = _mapperCommentInputToDB.Map(comment);

            commentDB.PostId = postId;
            commentDB.Type = CommentType.Reject;

            dbPost = await _postRepository.Edit(dbPost, commentDB);

            return dbPost;
        }
    }
}
