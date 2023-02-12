using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels;
using Models.ApiModels.ResponseDTO;
using Models.Entities;
using Models.Query;
using Services.Exceptions.Blogs;
using Services.Extensions;
using System.Net.Mime;
using AutoMapper;
using BlogApi.Filters;

namespace BlogApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(SuspenededActionFilter))]
    public class BlogsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlogsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [HttpGet]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] BlogParameters blogParameters)
        {
            try
            {
                var username = User.Claims.Where(x => x.Type == "username").FirstOrDefault()?.Value;
                var blogs = await _unitOfWork.BlogRepository.GetBlogsAsync(blogParameters);
                Response.AddPaginationHeader(
                    currentPage: blogs.CurrentPage,
                    itemsPerPage: blogs.PageSize,
                    totalItems: blogs.TotalCount,
                    totalPages: blogs.TotalPages
                );
                return Ok(
                    _mapper.Map<List<BlogResponse>>(blogs)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetBlogs", ex.Message);
                return BadRequest(ModelState);
            }
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var blog = await _unitOfWork.BlogRepository.GetOneAsync(b => b.Id == id);
                if (blog == null)
                {
                    throw new BlogNotFoundException(id);
                }
                return Ok(
                    _mapper.Map<BlogResponse>(blog)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetBlog", ex.Message);
                return BadRequest(ModelState);
            }
        }


        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] BlogRequest blogModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    var blog = _mapper.Map<Blog>(blogModel);
                    blog.UserId = userId;
                    await _unitOfWork.BlogRepository.AddAsync(blog);
                    await _unitOfWork.SaveAsync();

                    return Created("~api/blogs", blogModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AddBlog", ex.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(int id, [FromBody] BlogRequest blogModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var blog = await _unitOfWork.BlogRepository
                        .GetOneAsync(b => b.Id == id, default!, default!);
                    if (blog == null)
                    {
                        return BadRequest(ModelState);
                    }
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    if (blog.UserId != userId)
                    {
                        return Unauthorized();
                    }

                    blog.Description = blogModel.Description;
                    blog.Title = blogModel.Title;

                    await _unitOfWork.BlogRepository.UpdateAsync(blog);
                    await _unitOfWork.SaveAsync();

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UpdateBlog", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var blog = await _unitOfWork.BlogRepository
                    .GetOneAsync(b => b.Id == id, default!, default!);
                if (blog == null)
                {
                    return BadRequest(
                        "Blog deosn't exist or already been deleted."
                        );
                }
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                if (blog.UserId != userId)
                {
                    return Unauthorized();
                }
                await _unitOfWork.BlogRepository.RemoveAsync(blog);
                await _unitOfWork.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("DeleteBlog", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("followed-blogs")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFollowedBlogs()
        {
            var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
            var blogs = await _unitOfWork.BlogRepository.GetFollowedBlogsAsync(userId);

            return Ok(
                _mapper.Map<List<BlogResponse>>(blogs)
                );
        }

        [AllowAnonymous]
        [HttpGet("{blogid}/followers")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBlogFollowers([FromRoute] int blogid)
        {
            List<AppUser> followers = await _unitOfWork.BlogRepository.GetFollowers(blogid);

            return Ok(
                _mapper.Map<List<AppUserResponse>>(followers)
                );
        }

        [HttpPost("{id}/follow")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Follow(int id)
        {
            try
            {
                var blog = await _unitOfWork.BlogRepository
                    .GetOneAsync(b => b.Id == id, default!, default!);
                if (blog == null)
                {
                    throw new BlogNotFoundException(id);
                }
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;

                if (blog.UserId == userId)
                {
                    // user cant follow his own blogs
                    return BadRequest(
                        "You can't follow your OWN blogs."
                        );
                }
                await _unitOfWork.BlogRepository.AddFollowerAsync(blog.Id, userId);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("FollowBlog", ex.Message);
            }
            return BadRequest(ModelState);
        }


        [HttpPost("{id}/unfollow")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnFollow(int id)
        {
            try
            {
                var blog = await _unitOfWork.BlogRepository
                    .GetOneAsync(b => b.Id == id,
                        includeProperties: null,
                        tracked: true
                    );
                if (blog == null)
                {
                    throw new BlogNotFoundException(id);
                }
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                await _unitOfWork.BlogRepository.RemoveFollowerAsync(blog.Id, userId);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UnfollowdBlog", ex.Message);
            }
            return BadRequest(ModelState);
        }

    }

}



