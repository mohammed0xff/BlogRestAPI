using AutoMapper;
using BlogApi.Filters;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
