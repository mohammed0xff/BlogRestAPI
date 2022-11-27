using AutoMapper;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels;
using Models.Entities;
using System.Net.Mime;

namespace BlogApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public CommentsController(IUnitOfWork unitOfWork, IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("/api/posts/{postId}/comments")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetComments(int postId)
        {
            var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
            var comments = _unitOfWork.CommentRepository.GetAll(postId, userId); 
            var commentResponse = _mapper.Map<List<CommentResponse>>(comments);

            return Ok(
                commentResponse
                );
        }



        [HttpPost("/api/posts/{postId}/comments")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(int postId, [FromBody] CommentRequest comment)
        {
            try
            {
                if (ModelState.IsValid) {
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    var post = _unitOfWork.PostRepository.Get(p => p.Id == postId, default!, default!);
                    if(post == null)
                    {
                        ModelState.AddModelError("addComment", "Post doens't exist.");
                        return BadRequest();
                    }
                    var newComment = _mapper.Map<Comment>(comment);
                    newComment.UserId = userId;
                    newComment.PostId = postId;
                    _unitOfWork.CommentRepository.Add(newComment);
                    _unitOfWork.save();
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
        public IActionResult Put(int postId, int commentId , [FromBody] CommentRequest ModifiedComment)
        {
            try
            {
                if (ModelState.IsValid) // would null values in req model apply on db ?? 
                {
                    var comment = _unitOfWork.CommentRepository.Get(c => c.Id == commentId, default!, default!);
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    if (comment.UserId != userId)
                    {
                        return Unauthorized();
                    }
                    if(comment == null)
                    {
                        ModelState.AddModelError("EditComment", "Comment doesnt exist");
                        return BadRequest();
                    }
                    comment.Content = ModifiedComment.Content;
                    _unitOfWork.CommentRepository.Update(comment);
                    _unitOfWork.save();

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
        public IActionResult Delete(int postId, int commentId)
        {
            try
            {
                var comment = _unitOfWork.CommentRepository.Get(c => c.Id == commentId, default!, default!);
                if (comment == null)
                {
                    return BadRequest();
                }
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                if (comment.UserId != userId)
                {
                    return Unauthorized();
                }
                _unitOfWork.CommentRepository.Remove(comment);
                _unitOfWork.save();
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
        public IActionResult LikeComment(int postId, int commentId)
        {
            try
            {
                var comment = _unitOfWork.CommentRepository.Get(c => c.Id == commentId, default!, default!);
                if (comment == null) return BadRequest();
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                _unitOfWork.CommentRepository.AddLike(commentId, userId);
                _unitOfWork.save();
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
        public IActionResult RemoveLike(int postId, int commentId)
        {
            try
            {
                var comment = _unitOfWork.CommentRepository.Get(c => c.Id == commentId, default!, default!);
                if (comment == null) return BadRequest();
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                _unitOfWork.CommentRepository.RemoveLike(commentId, userId);
                _unitOfWork.save();
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
