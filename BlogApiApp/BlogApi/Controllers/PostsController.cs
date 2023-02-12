using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels;
using Models.Entities;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Services.Extensions;
using Models.Query;
using BlogApi.Filters;
using Models.ApiModels.ResponseDTO;
using AutoMapper;
using Services.Exceptions.Posts;
using Services.Exceptions.Blogs;

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
        public async Task<IActionResult> Get([FromRoute]int blogId, [FromQuery]PostParameters postParameters )
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                postParameters.UsreId = userId;
                var posts = await _unitOfWork.PostRepository
                    .GetPostsAsync(blogId, postParameters);
                
                Response.AddPaginationHeader(
                   currentPage:  posts.CurrentPage,
                   itemsPerPage: posts.PageSize,
                   totalItems:   posts.TotalCount,
                   totalPages:   posts.TotalPages
                );

               return Ok(
                    _mapper.Map<List<PostResponse>>(posts)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetPagePost", ex.Message);
                return BadRequest(ModelState);
            }
        }


        [AllowAnonymous]
        [HttpGet("/api/posts/{postId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int postId)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                var post = await _unitOfWork.PostRepository.GetOneAsync(postId, userId);
                if(post == null)
                    throw new PostNotFoundException(postId);
                
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
        public async Task<IActionResult> Post([FromRoute]int blogId,[FromBody] PostRequest postModel)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                var blog = await _unitOfWork.BlogRepository
                    .GetOneAsync(b => b.Id == postModel.BlogId, tracked : true);

                if (blog == null)
                {
                    throw new BlogNotFoundException(blogId);
                }
                if (blog.UserId != userId)
                {
                    return Unauthorized();
                }
                
                var post = _mapper.Map<Post>(postModel);
                post.UserId = userId;
                blog.Posts.Add(post);
                await _unitOfWork.SaveAsync();

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
        public async Task<IActionResult> Put(int postId, [FromBody] PostRequest postModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var post = await _unitOfWork.PostRepository
                        .GetOneAsync(p => p.Id == postId, default!, default!);
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

                    await _unitOfWork.PostRepository.UpdateAsync(post);
                    await _unitOfWork.SaveAsync();

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
        public async Task<IActionResult> Delete( int postId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var post = await _unitOfWork.PostRepository
                        .GetOneAsync(p => p.Id == postId, default!, default!);
                    if (post == null)
                    { 
                        return BadRequest(ModelState); 
                    }
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    if (post.UserId != userId) 
                    {
                        return Unauthorized();
                    }
                    await _unitOfWork.PostRepository.RemoveAsync(post);
                    await _unitOfWork.SaveAsync();
                    
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
        public async Task<IActionResult> LikePost(int postId) 
        {
            try
            {
                var Post = await _unitOfWork.PostRepository
                    .GetOneAsync(c => c.Id == postId, default!, default!);
                if (Post == null) return BadRequest();
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                await _unitOfWork.PostRepository.AddLikeAsync(Post.Id, userId);
                await _unitOfWork.SaveAsync();
                
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
        public async Task<IActionResult> UnLikePost(int postId)
        {
            try
            {
                var Post = await _unitOfWork.PostRepository
                    .GetOneAsync(c => c.Id == postId, default!, default!);
                if (Post == null) return BadRequest();
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                await _unitOfWork.PostRepository.RemoveLikeAsync(Post.Id, userId);
                await _unitOfWork.SaveAsync();
                
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
        public async Task<IActionResult> Likes(int postId) 
        {
            try
            {
                var Post = await _unitOfWork.PostRepository.GetOneAsync(c => c.Id == postId);
                if (Post == null) return BadRequest();
                var usersLikes = await _unitOfWork.PostRepository.GetLikesAsync(postId);
                
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


        [HttpGet("/api/posts/{postId}/add-tag")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TagPost([FromRoute]int postId, [FromQuery]string tagName)
        {
            try
            {
                var post = await _unitOfWork.PostRepository
                    .GetOneAsync(c => c.Id == postId, "PostTags", tracked: true);
                if (Post == null) 
                    return BadRequest();
                
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                if (post.UserId != userId) 
                    return Unauthorized();
                
                Tag? tag = await _unitOfWork.PostRepository.GetTagByName(tagName);
                if(tag == null) 
                    return BadRequest(
                        "Not valid tag name."
                        );
                if (post.PostTags.Any(x => x.Tag.Id == tag.Id))
                    return BadRequest(
                        "Tag already on post."
                        );

                post.PostTags.Add(new PostTag { TagId = tag.Id });
                await _unitOfWork.SaveAsync();

                return Created( $"~api/posts/{postId}/add-tag",
                    _mapper.Map<PostResponse>(post)
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetPostlikes", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("/api/posts/{postId}/remove-tag")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveTagPost([FromRoute]int postId, [FromQuery] string tagName)
        {
            try
            {
                if (tagName == null)
                {
                    return BadRequest(
                        "You must Specify tagname in request query."
                        );
                }
                var post = await _unitOfWork.PostRepository
                    .GetOneAsync(c => c.Id == postId, "PostTags", tracked : true);
                if (Post == null)
                    return BadRequest();

                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                if (post.UserId != userId)
                    return Unauthorized(
                        "This post doenst belong to you."
                        );

                Tag? tag = await _unitOfWork.PostRepository.GetTagByName(tagName);
                if (tag == null)
                    return BadRequest(
                        "Not a valid tag name."
                        );
                
                if(post.PostTags.Any(x => x.Tag.Id == tag.Id))
                    return BadRequest(
                        "Tag already on post."
                        );

                post.PostTags.Remove(new PostTag { TagId = tag.Id });
                await _unitOfWork.SaveAsync();

                return Created(
                    $"~api/posts/{postId}/remove-tag",
                    _mapper.Map<PostResponse>(post)
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
