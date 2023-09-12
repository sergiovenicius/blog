using blog.common.Middleware;
using blog.common.Exceptions;
using blog.common.Model;
using blog.common.Service;
using Microsoft.AspNetCore.Mvc;
using blog.api.Model;

namespace Blog.Controllers
{
    [ApiController]
    [AuthorizeFilter]
    [Route("api/v1/posts")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _postService;
        private readonly CurrentUser _currentUser;

        public PostController(ILogger<PostController> logger, IPostService postService, CurrentUser currentUser)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        /// <summary>
        /// List all published Posts
        /// </summary>
        [HttpGet("")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                return Ok(await _postService.ListAsync(new PostStatus[] { PostStatus.Approved }));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }
        }

        /// <summary>
        /// List all "pending to approval" Posts
        /// </summary>
        [HttpGet("pending")]
        [HasPermission(UserRole.Editor)]
        public async Task<IActionResult> GetPendingPostsAsync()
        {
            try
            {
                return Ok(await _postService.ListAsync(new PostStatus[] { PostStatus.Pending_Approval }));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }
        }

        /// <summary>
        /// Get a Post by id
        /// </summary>
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long postId)
        {
            try
            {
                return Ok(await _postService.GetByIdAsync(_currentUser.Id, postId));
            }
            catch (NotFoundException e)
            {
                return NotFound(new ErrorMessage(e.Message));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }

        }

        /// <summary>
        /// Get all Posts of the Owner/Writer
        /// </summary>
        [HttpGet("mine")]
        [HasPermission(UserRole.Writer)]
        public async Task<IActionResult> GetByOwnerAsync()
        {
            try
            {
                return Ok(await _postService.ListByOwnerAsync(_currentUser.Id));
            }
            catch (NotFoundException e)
            {
                return NotFound(new ErrorMessage(e.Message));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }

        }

        /// <summary>
        /// Save a new Post
        /// </summary>
        [HttpPost("")]
        [HasPermission(UserRole.Writer)]
        public async Task<IActionResult> PostAsync([FromBody] PostInput post)
        {
            try
            {
                return Ok(await _postService.AddAsync(post));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }
        }

        /// <summary>
        /// Update a Post
        /// </summary>
        [HttpPut("{postId}")]
        [HasPermission(UserRole.Writer)]
        public async Task<IActionResult> EditAsync([FromRoute] long postId, [FromBody] PostInput post)
        {
            try
            {
                return Ok(await _postService.EditAsync(postId, post));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }
        }

        /// <summary>
        /// Submit a Post to be approved or rejected
        /// </summary>
        [HttpPatch("submit/{postId}")]
        [HasPermission(UserRole.Writer)]
        public async Task<IActionResult> SubmitAsync([FromRoute] long postId)
        {
            try
            {
                return Ok(await _postService.SubmitAsync(postId));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }
        }

        /// <summary>
        /// Approve a Post
        /// </summary>
        [HttpPatch("approve/{postId}")]
        [HasPermission(UserRole.Editor)]
        public async Task<IActionResult> ApproveAsync([FromRoute] long postId)
        {
            try
            {
                return Ok(await _postService.ApproveAsync(postId));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }
        }

        /// <summary>
        /// Reject a Post
        /// </summary>
        [HttpPatch("reject/{postId}")]
        [HasPermission(UserRole.Editor)]
        public async Task<IActionResult> RejectAsync([FromRoute] long postId, [FromBody] CommentInput comment)
        {
            try
            {
                return Ok(await _postService.RejectAsync(postId, comment));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }
        }

        /// <summary>
        /// Add a comment to a published Post
        /// </summary>
        [HttpPost("comment/{postId}")]
        public async Task<IActionResult> CommentAsync([FromRoute] long postId, [FromBody] CommentInput comment)
        {
            try
            {
                return Ok(await _postService.CommentAsync(postId, comment));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorMessage(e.Message));
            }
        }
    }
}