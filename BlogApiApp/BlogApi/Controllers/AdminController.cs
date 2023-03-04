using AutoMapper;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels.ResponseDTO;
using Models.Constants;
using Models.Entities;
using Services.Exceptions.Users;
using System.Net.Mime;

namespace BlogApi.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class Adminontroller : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<Adminontroller> _logger;

        public Adminontroller(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Suspend a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        [HttpGet("suspend-user/{username}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SuspendUser([FromRoute] string username)
        {
            try
            {
                var user = await _unitOfWork.AppUsers.SuspendByUsername(username);
                if(user == null)
                {
                    throw new UserNotFoundException(
                        username
                        );
                }
                _logger.LogInformation("User with usrname : {} is now suspended.", username);

                return Ok(
                    _mapper.Map<AppUserAdminResponse>(user)
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{Error} Executing {Action} with parameters {Parameters}.", 
                        ex.Message, nameof(SuspendUser), username
                    );

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Unsuspend a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        [HttpGet("unsuspend-user/{username}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnSuspendUser([FromRoute] string username)
        {
            try
            {
                var user = await _unitOfWork.AppUsers.UnSuspendByUsername(username);
                if (user == null)
                {
                    throw new UserNotFoundException(
                        username
                        );
                }
                _logger.LogInformation("User with usrname : {} is now unsuspended.", username);

                return Ok(
                    _mapper.Map<List<AppUser>>(user)
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "{Error} Executing {Action} with parameters {Parameters}.",
                        ex.Message, nameof(UnSuspendUser), username
                    );

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
