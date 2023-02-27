using AutoMapper;
using BlogApi.Filters;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels.ResponseDTO;
using System.Net.Mime;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(SuspenededActionFilter))]
    public class TagsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TagsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all available tags
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            return Ok(
                 _mapper.Map<List<TagResponse>>(
                    await _unitOfWork.PostRepository.GetAvailableTags()
                ));
        }

        /// <summary>
        /// Create a new tag
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        [HttpPost("create-tag/{tagname}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTag([FromRoute]string tagname)
        {
            await _unitOfWork.PostRepository.CreateTag(tagname);
            return Created(
                "~/tag",
                 _mapper.Map<TagResponse>(tagname)
                );
        }

    }
}
