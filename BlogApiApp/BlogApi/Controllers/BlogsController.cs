using AutoMapper;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels;
using Models.Entities;
using Services.Extensions;
using System.Net.Mime;


namespace BlogApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Get([FromQuery]int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string userid = null)
        {
            try
            {
                var blogs = await _unitOfWork.BlogRepository.GetPageAsync(pageNumber, pageSize, userid);
                Response.AddPaginationHeader(
                    currentPage : pageNumber,
                    itemsPerPage : pageSize,
                    totalItems : blogs.TotalCount,
                    totalPages : blogs.TotalPages
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
        public IActionResult Get(int id)
        {
            try
            {
                var blog = _unitOfWork.BlogRepository.Get(b => b.Id == id);
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
        public IActionResult Post([FromBody] BlogRequest blogModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                    if(userId == null) return BadRequest(ModelState);   
                    var blog = _mapper.Map<Blog>(blogModel);
                    blog.UserId = userId;
                    _unitOfWork.BlogRepository.Add(blog);
                    _unitOfWork.save();
                    Response.StatusCode = 201;
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
        public IActionResult Put(int id, [FromBody] BlogRequest blogModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var blog = _unitOfWork.BlogRepository.Get(b => b.Id == id, default!, default!);
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

                    _unitOfWork.BlogRepository.Update(blog);
                    _unitOfWork.save();

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
        public IActionResult Delete(int id)
        {
            try
            {
                var blog = _unitOfWork.BlogRepository.Get(b => b.Id == id, default!, default!);
                if (blog == null) 
                { 
                    return BadRequest(); 
                }
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;

                if (blog.UserId != userId)
                {
                    return Unauthorized();
                }
                _unitOfWork.BlogRepository.Remove(blog);
                _unitOfWork.save();
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


        [HttpPost("{id}/follow")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Follow(int id)
        {
            try
            {
                var blog = _unitOfWork.BlogRepository.Get(b => b.Id == id, default!, default!);
                if (blog == null)
                {
                    return BadRequest();
                }
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;

                if (blog.UserId == userId)
                {
                    // user cant follow his own blogs
                    return BadRequest();
                }
                _unitOfWork.BlogRepository.AddFollower(blog.Id, userId);
                _unitOfWork.save();
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
        public IActionResult UnFollow(int id)
        {
            try
            {
                var blog = _unitOfWork.BlogRepository.Get(b => b.Id == id, default!, default!);
                if (blog == null)
                {
                    return BadRequest();
                }
                var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
                _unitOfWork.BlogRepository.RemoveFollower(blog.Id, userId);
                _unitOfWork.save();
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UnfollowBlog", ex.Message);
            }
            return BadRequest(ModelState);
        }

    }

}



