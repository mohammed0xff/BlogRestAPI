using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ApiModels.ResponseDTO;
using Models.Constants;
using Models.Entities;
using Services.Exceptions.Users;
using System.Net.Mime;
using System.Text.RegularExpressions;
using AutoMapper;
using Services.Storage;
using BlogApi.Filters;
using ISession = Services.Authentication.Session.ISession;

namespace BlogApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(SuspenededActionFilter))]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly ISession _session;

        public UserController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IStorageService storageService
,
            ISession session)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storageService = storageService;
            _session = session;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="suspended"></param>
        /// <returns></returns>
        [HttpGet("users-list")]
        [Authorize(Roles = Roles.Admin)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool suspended = false)
        {
            IEnumerable<AppUser> users;
            if(suspended)
            {
                users = await _unitOfWork.AppUsers
                    .GetAllAsync(x => x.IsSuspended.Equals(true));
            }
            else
            {
                users = await _unitOfWork.AppUsers.GetAllAsync();
            }

            return Ok(
                _mapper.Map<IEnumerable<AppUserAdminResponse>>(users)
                );
        }
        
        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUser([FromQuery] string? username)
        {
            try
            {
                if(username == null)
                {
                    username = User.Claims.Where(x => x.Type == "username").FirstOrDefault()?.Value;
                }
                AppUser user = await _unitOfWork.AppUsers
                    .GetOneAsync(u => u.UserName == username);
                if(user == null)
                {
                    throw new UserNotFoundException(username);
                }

                return Ok(
                    _mapper.Map<AppUserResponse>(user)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GetUser", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Get user's profile image
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        [HttpPost("change-profile-photo")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePicture(IFormFile imageFile)
        {
            try
            {
                var userId = _session.UserId;
                AppUser user = await _unitOfWork.AppUsers
                    .GetOneAsync(u => 
                        u.Id.Equals(userId), 
                        tracked: true
                        );
                
                // delete old image 
                _storageService.DeleteProfileImage(user.UserName);
                
                // upload new one 
                var path = _storageService.UploadProfileImage(imageFile, user.UserName);
                user.ProfileImagePath = path;
                
                // save changes
                await _unitOfWork.SaveAsync();
                
                return Ok(
                    _mapper.Map<AppUserResponse>(user)
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("change-profile-photo", ex.Message);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Remove user's profile image
        /// </summary>
        /// <returns></returns>
        [HttpDelete("remove-profile-photo")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemovePicture()
        {
            try
            {
                var userId = _session.UserId;
                AppUser user = await _unitOfWork.AppUsers
                    .GetOneAsync( u => 
                        u.Id.Equals(userId), 
                        tracked: true
                        );

                _storageService.DeleteProfileImage(user.UserName);
                
                // set path as null
                user.ProfileImagePath = null;
                
                // save changes
                await _unitOfWork.SaveAsync();
                
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("remove-profile-photo", ex.Message);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Change user's username
        /// </summary>
        /// <param name="newUsername"></param>
        /// <returns></returns>
        /// <exception cref="UsernameAlreadyExistsException"></exception>
        /// <exception cref="NotValidUsernameException"></exception>
        [HttpPost("change-username")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeUsername([FromBody] string newUsername)
        {
            try
            {
                if (await _unitOfWork.AppUsers.UsernameAlreadyExists(newUsername))
                {
                    throw new UsernameAlreadyExistsException(newUsername);
                }

                if (!IsValidUsername(newUsername))
                {
                    throw new NotValidUsernameException(newUsername);
                }
                
                var username = _session.Username;
                await _unitOfWork.AppUsers.ChangeUsername(username, newUsername);
                
                return Ok(
                    "Username changed successfully."
                    );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("change-username", ex.Message);
            }
            return BadRequest(ModelState);
        }


        private static Regex sUserNameAllowedRegEx = new Regex(
                @"^[a-zA-Z]{1}[a-zA-Z0-9\._\-]{0,23}[^.-]$", 
                RegexOptions.Compiled
            );
        private static Regex sUserNameIllegalEndingRegEx = new Regex(
                @"(\.|\-|\._|\-_)$", 
                RegexOptions.Compiled
            );
        private static bool IsValidUsername(string userName)
        {
            if (string.IsNullOrEmpty(userName)
                || !sUserNameAllowedRegEx.IsMatch(userName)
                || sUserNameIllegalEndingRegEx.IsMatch(userName))
            {
                return false;
            }
            return true;
        }
    }
}
