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
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BlogsController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        ///  Get Paged list of blogs
        /// </summary>
        /// <param name="blogParameters"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] BlogParameters blogParameters)
        {
            try
            {
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
                _logger.LogError("Getting paged blog result : ", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        ///  Get Blog by id numeric
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var blog = await _unitOfWork.BlogRepository.GetOneAsync(b => b.Id == id);
                if (blog == null)
                {
                    return NotFound();
                }
                return Ok(
                    _mapper.Map<BlogResponse>(blog)
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError("Getting blog by id : ", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        ///  Insert one blog in database
        /// </summary>
        /// <param name="blogModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                _logger.LogError("Adding a blog to database: ", ex.Message);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Update a blog 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="blogModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody] BlogRequest blogModel)
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
                _logger.LogError("Updating blog from database: ", ex.Message);

            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Delete one blog from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] int id)
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
                _logger.LogError("Deleting a blog to database: ", ex.Message);

            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Get all Folowed blog by current user
        /// </summary>
        /// <returns></returns>
        [HttpGet("followed-blogs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFollowedBlogs()
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                var blogs = await _unitOfWork.BlogRepository.GetFollowedBlogsAsync(userId);

                return Ok(
                    _mapper.Map<List<BlogResponse>>(blogs)
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError("{Error} Executing {Action} .", ex.Message, nameof(GetFollowedBlogs));
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Get all followers for a certain blog
        /// </summary>
        /// <param name="blogid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{blogid}/followers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBlogFollowers([FromRoute] int blogid)
        {
            try
            {
                List<AppUser> followers = await _unitOfWork.BlogRepository.GetFollowers(blogid);

                return Ok(
                    _mapper.Map<List<AppUserResponse>>(followers)
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError("{Error} Executing {Action} .", ex.Message, nameof(GetBlogFollowers));
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Follow a blog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="BlogNotFoundException"></exception>
        [HttpPost("{id}/follow")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Follow([FromRoute] int id)
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
                _logger.LogError(
                    "{Error} Executing {Action} with parameters {Parameters}.", 
                        ex.Message, nameof(Follow), id
                    );
            }

            return ModelState.ErrorCount > 0 ? 
                BadRequest(ModelState) : StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Unfollow a blog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="BlogNotFoundException"></exception>
        [HttpPost("{id}/unfollow")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnFollow([FromRoute] int id)
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
                _logger.LogError(
                    "{Error} Executing {Action} with parameters {Parameters}.",
                        ex.Message, nameof(UnFollow), id
                    );
            }
            return ModelState.ErrorCount > 0 ?
                BadRequest(ModelState) : StatusCode(StatusCodes.Status500InternalServerError);
        }

    }

}



