using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels;
using Models.Entities;
using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace BlogApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostsController(IUnitOfWork unitOfWork, IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("/api/blogs/{blogId}/posts")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PostResponse>))]
        public IActionResult GetPosts(int blogId) // we can even add paging .. 
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                var posts = _unitOfWork.PostRepository.GetAll(blogId, userId);
                return Ok(
                        _mapper.Map<List<PostResponse>>(posts)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetAllPosts", ex.Message);
                return BadRequest(ModelState);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("/api/blogs/blogId/posts/page")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPage(int pageNumber, int pageSize)
        {
            try
            {
                return Ok(
                    _unitOfWork.PostRepository.GetPage(pageNumber, pageSize)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetPagePost", ex.Message);
                return BadRequest(ModelState);
            }
        }



        [AllowAnonymous]
        [HttpGet("/api/blogs/{blogId}/posts/{postId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get( int postId)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                var post =_unitOfWork.PostRepository.Get(postId, userId);
                return Ok(
                        _mapper.Map<PostResponse>(post)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetPostById", ex.Message);
                return BadRequest(ModelState);
            }
        }


        [HttpPost("/api/blogs/{blogId}/posts")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] PostRequest postModel)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                var blog = _unitOfWork.BlogRepository.Get(b => b.Id == postModel.BlogId, default!, default!);

                if (blog == null)
                {
                    ModelState.AddModelError("Post Post", "Blog Doesn't Exist");
                    return BadRequest(ModelState);
                }
                if (blog.UserId != userId)
                {
                    return Unauthorized();
                }
                
                var post = _mapper.Map<Post>(postModel);
                post.UserId = userId;
                _unitOfWork.PostRepository.Add(post);
                _unitOfWork.save();

                return Created(
                    $"~api/blogs/{blog.Id}/posts", 
                    postModel
                    );
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AddPost", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpPut("/api/blogs/{blogId}/posts/{postId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Put(int postId, [FromBody] PostRequest postModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var post = _unitOfWork.PostRepository.Get(p => p.Id == postId, default!, default!);
                    if (post == null)
                    {
                        return BadRequest(ModelState);
                    }
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    if (post.UserId != userId)
                    {
                        return Unauthorized();
                    }
                    post.HeadLine = postModel.HeadLine;
                    post.Content = postModel.Content;

                    _unitOfWork.PostRepository.Update(post);
                    _unitOfWork.save();

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UpdatePost", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpDelete("/api/blogs/{blogId}/posts/{postId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Delete( int postId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var post = _unitOfWork.PostRepository.Get(p => p.Id == postId, default!, default!);
                    if (post == null)
                    { 
                        return BadRequest(ModelState); 
                    }
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    if (post.UserId != userId) 
                    {
                        return Unauthorized();
                    }
                    _unitOfWork.PostRepository.Remove(post);
                    _unitOfWork.save();
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("DeletePost", ex.Message);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("/api/posts/{postId}/like")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult LikePost(int postId) 
        {
            try
            {
                var Post = _unitOfWork.PostRepository.Get(c => c.Id == postId, "Likes",default!);
                if (Post == null) return BadRequest();
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                _unitOfWork.PostRepository.AddLike(Post.Id, userId);
                _unitOfWork.save();
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("LikePost", ex.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("/api/posts/{postId}/unlike")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UnLikePost(int postId)
        {
            try
            {
                var Post = _unitOfWork.PostRepository.Get(c => c.Id == postId, "Likes", default!);
                if (Post == null) return BadRequest();
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                _unitOfWork.PostRepository.RemoveLike(Post.Id, userId);
                _unitOfWork.save();
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UnlikePost", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [AllowAnonymous]
        [HttpGet("/api/posts/{postId}/likes")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Likes(int postId) 
        {
            try
            {
                var Post = _unitOfWork.PostRepository.Get(c => c.Id == postId);
                if (Post == null) return BadRequest();
                var usersLikes = _unitOfWork.PostRepository.GetLikes(postId).ToList();
                    return Ok(
                        _mapper.Map<List<AppUserResponse>>(usersLikes)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetPostlikes", ex.Message);
            }
            return BadRequest(ModelState);
        }

    }


}
