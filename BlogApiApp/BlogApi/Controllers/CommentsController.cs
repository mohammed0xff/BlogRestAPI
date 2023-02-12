using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels;
using Models.Entities;
using System.Net.Mime;
using AutoMapper;
using BlogApi.Filters;

namespace BlogApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(SuspenededActionFilter))]
    public class CommentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public CommentsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [AllowAnonymous]
        [HttpGet("/api/posts/{postId}/comments")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComments(int postId)
        {
            var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
            var comments = await _unitOfWork.CommentRepository
                .GetAllCommentstAsync(postId, userId);
            var commentResponse = _mapper.Map<List<CommentResponse>>(comments);

            return Ok(
                commentResponse
                );
        }


        [HttpPost("/api/posts/{postId}/comments")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(int postId, [FromBody] CommentRequest comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    var post = await _unitOfWork.PostRepository
                        .GetOneAsync(p => p.Id == postId, default!, default!);
                    if (post == null)
                    {
                        ModelState.AddModelError(
                            "addComment",
                            "Post doens't exist."
                            );
                        return BadRequest();
                    }
                    if (post.CommentsDisabled)
                    {
                        ModelState.AddModelError(
                            "addComment",
                            "Comments are desabled for this post."
                            );
                        return BadRequest();
                    }
                    var newComment = _mapper.Map<Comment>(comment);
                    newComment.UserId = userId;
                    //newComment.PostId = postId;
                    post.Comments.Add(newComment);
                    // await _unitOfWork.CommentRepository.AddAsync(newComment);
                    // await _unitOfWork.PostRepository.Update(post);
                    await _unitOfWork.SaveAsync();
                    return Created(
                        $"~api/posts/{postId}/comments",
                        comment
                        );
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("addComment", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpPut("/api/posts/{postId}/comments/{commentId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Put(int postId, int commentId, [FromBody] CommentRequest ModifiedComment)
        {
            try
            {
                if (ModelState.IsValid) // would null values in req model apply on db ?? 
                {
                    var comment = await _unitOfWork.CommentRepository.GetOneAsync(c => c.Id == commentId, default!, default!);
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    if (comment.UserId != userId)
                    {
                        return Unauthorized();
                    }
                    if (comment == null)
                    {
                        ModelState.AddModelError("EditComment", "Comment doesn't exist");
                        return BadRequest();
                    }
                    comment.Content = ModifiedComment.Content;
                    await _unitOfWork.CommentRepository.UpdateAsync(comment);
                    await _unitOfWork.SaveAsync();

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("EditComment", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpDelete("/api/posts/{postId}/comments/{commentId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Delete(int postId, int commentId)
        {
            try
            {
                var comment = await _unitOfWork.CommentRepository.GetOneAsync(c => c.Id == commentId, default!, default!);
                if (comment == null)
                {
                    return BadRequest();
                }
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                if (comment.UserId != userId)
                {
                    return Unauthorized();
                }
                await _unitOfWork.CommentRepository.RemoveAsync(comment);
                await _unitOfWork.SaveAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("DeleteComment", ex.Message);
            }
            return BadRequest(ModelState);

        }


        [HttpPost("/api/posts/{postId}/comments/{commentId}/like")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> LikeComment(int postId, int commentId)
        {
            try
            {
                var comment = await _unitOfWork.CommentRepository.GetOneAsync(c => c.Id == commentId, default!, default!);
                if (comment == null) return BadRequest();
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                await _unitOfWork.CommentRepository.AddLikeAsync(commentId, userId);
                await _unitOfWork.SaveAsync();
                return Ok();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("likeComment", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpPost("/api/posts/{postId}/comments/{commentId}/unlike")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> RemoveLike(int postId, int commentId)
        {
            try
            {
                var comment = await _unitOfWork.CommentRepository.GetOneAsync(c => c.Id == commentId, default!, default!);
                if (comment == null) return BadRequest();
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                await _unitOfWork.CommentRepository.RemoveLikeAsync(commentId, userId);
                await _unitOfWork.SaveAsync();
                return Ok();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UnlikeComment", ex.Message);
            }
            return BadRequest(ModelState);
        }

    }
}
