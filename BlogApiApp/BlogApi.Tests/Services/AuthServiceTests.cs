using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using DataAccess.Repositories.Interfaces;
using Services.Helpers;
using Models.ApiModels;
using Models.Entities;
using NUnit.Framework;
using Services;
using Moq;
using Services.Options;
using Models.Constants;

namespace BlogApi.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private IAuthService _authService;
        private readonly Mock<TokenValidationParameters> _tokenValidationParams;
        private readonly Mock<UserManager<AppUser>> _userManager;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly JWTOptions jwt;
        private readonly AppUser _user;
        private readonly string password;

        public AuthServiceTests()
        {
            _user = new AppUser
            {
                Id = "userid",
                FirstName = "test",
                LastName = "test",
                UserName = "test",
                Email = "test123@test.com",
            };
            password = "test123@PWD";
            List<AppUser> _users = new List<AppUser>
            {
                _user
            };

            jwt = new JWTOptions
            {
                Key = "sz8eI7OdHBrjrIo8j9nTW/rQyO1OvY0pAQ2wDKQZw/0=",
                Issuer = "issuer",
                Audience = "audience",
                DurationInDays = 3,
            };

            _tokenValidationParams = new Mock<TokenValidationParameters>();
            _userManager = MockUserManager(_users);
            _unitOfWork = new Mock<IUnitOfWork>();

            _authService = new AuthService(
                    _userManager.Object,
                    _tokenValidationParams.Object,
                    Options.Create<JWTOptions>(jwt),
                    _unitOfWork.Object
                    );
            }

        [Test]
        public async Task RegisterAsync_Returns_WhenSuccess()
        {
            // Arrange
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);
            _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);
            _userManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()));
            List<string> userRoles = new()
            {
                Roles.Admin,
                Roles.User,
            };
            _userManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(userRoles);
            _unitOfWork.Setup(x => x.TokenRepository.AddAsync(It.IsAny<RefreshToken>()));
            _userManager.Setup(x => x.GetClaimsAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<Claim>());

            RegistrationModelRequest registrationModelRequest = new RegistrationModelRequest
            {
                Username = "test123",
                FirstName = "test",
                LastName = "test",
                Email = "test123@test.com",
                ConfirmPassword = "test123@PWD",
                Password = "test123@PWD",
            };

            // Act
            var ErrorMessages = await _authService.RegisterAsync(registrationModelRequest);

            // Assert
            Assert.NotNull(ErrorMessages);
            Assert.IsEmpty(ErrorMessages);

        }

        [Test]
        public async Task LoginAsync_Returns_WhenSuccess()
        {
            List<Claim> claims = new List<Claim>();
            List<string> roles = new();

            _userManager.Setup(x => x.GetClaimsAsync(It.IsAny<AppUser>())).ReturnsAsync(claims);
            _userManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(roles);
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(_user);
            _userManager.Setup(x => x.CheckPasswordAsync(_user, password))
                .ReturnsAsync(true);
            
            _unitOfWork.Setup(x => x.TokenRepository.AddAsync(It.IsAny<RefreshToken>()));

            LoginModelRequest registrationModelRequest = new LoginModelRequest
            {
                Email = _user.Email,
                Password = password,
            };

            // Act
            var LoginResponse = await _authService.LoginAsync(registrationModelRequest);

            // Assert
            Assert.NotNull(LoginResponse);
            Assert.IsTrue(LoginResponse.IsAuthenticated);
            Assert.IsEmpty(LoginResponse.ErrorMessage);
            Assert.IsNotNull(LoginResponse.RefreshToken);
            Assert.IsNotNull(LoginResponse.Token);
            DateTime expiratoinDate = DateTime.Now.AddDays(jwt.DurationInDays);
            Assert.AreEqual(LoginResponse.ExpiresOn.Date, expiratoinDate.Date);

        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

    }
}


