using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using DataAccess.Repositories.Interfaces;
using BlogApi.Controllers;
using Models.ApiModels;
using Models.Entities;
using NUnit.Framework;
using AutoMapper;
using Moq;
using Models.Query;

namespace BlogApi.Tests.Controller
{
    [TestFixture]
    public class BlogsControllerTests
    {

        private readonly BlogsController _blogsController;
        private readonly Mock<IUnitOfWork> _unitOfWork ;
        private readonly Mock<IMapper> _mapper;
        private readonly string userId = Guid.NewGuid().ToString();

        public BlogsControllerTests()
        {
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _blogsController = new BlogsController(_unitOfWork.Object, _mapper.Object);

        }

        [SetUp]
        public void Setup()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "username"),
                new Claim(ClaimTypes.Name, "whoever@whatever.com"),
                new Claim("uid", userId),
            }, "TestAuthentication"));

            _blogsController.ControllerContext = new ControllerContext();
            _blogsController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var blog = new Blog()
            {
                Id = 1,
                UserId = userId,
                Description = "main",
                Title = "main"
            };

            _unitOfWork.Setup((x) => x.BlogRepository.AddAsync(It.IsAny<Blog>()));
            _unitOfWork.Setup((x) => x.BlogRepository.RemoveAsync(It.IsAny<Blog>()));
            _unitOfWork.Setup((x) => x.BlogRepository.AddFollowerAsync(It.IsAny<int>(), It.IsAny<string>()));
            _unitOfWork.Setup((x) => x.BlogRepository.RemoveFollowerAsync(It.IsAny<int>(), It.IsAny<string>()));
        }


        [Test]
        public void GetAllBlogs_ReturnListOfBlogs_200OK()
        {
            //Arrange
            var blogsResult = new Mock<IEnumerable<BlogResponse>>();
            var blogsList = new Mock<IEnumerable<Blog>>();
            var blogRequest = new BlogParameters {
                
            };
            var BlogDB = new Mock<List<Blog>>();
            _mapper.Setup(x => x.Map<IEnumerable<BlogResponse>>(blogsList))
                .Returns(blogsResult.Object);

            _unitOfWork.Setup((x) => x.BlogRepository.GetAllAsync(It.IsAny<Expression<Func<Blog, bool>>>(), default!))
                .ReturnsAsync(blogsList.Object);

            //Act
            var result = _blogsController.Get(blogRequest);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }  


        [Test]
        public void Post_Returns201Created_WhenSuccess()
        {
            //Arrange
            var blogreq = new Mock<BlogRequest>();
            var blog = new Mock<Blog>();
            _mapper.Setup(x => x.Map<Blog>(blogreq.Object))
            .Returns(blog.Object);

            //Act
            var result = _blogsController.Post(blogreq.Object);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<CreatedResult>());

        }

        [Test]
        public void Put_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var blog = new Blog() {
                Id = 1,
                Description = "...",
                Title = "...",
                UserId = userId
            };

            var blogReq = new BlogRequest()
            {
                Id = 1,
                Description = "test edit",
                Title = "..."
            };

            /*            
            // doesnt work, dk why!
            Expression<Func<Blog, bool>> testExpression = b => (b.Id == blog.Id);
            _unitOfWork.Setup((x) => x.BlogRepository
                .Get(It.Is<Expression<Func<Blog, bool>>>((criteria) => criteria.Equals(testExpression)), default!, default!))
                .Returns(blog);
            */
            _unitOfWork.Setup((x) => x.BlogRepository.GetOneAsync(It.IsAny<Expression<Func<Blog, bool>>>(), default!, default!))
                .ReturnsAsync(blog);


            // Act
            var result = _blogsController.Put(blog.Id, blogReq);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NoContentResult>());
            Assert.That(blog.Description, Is.EqualTo("test edit"));
        }


        [Test]
        public void Put_Returns401Unauthorized_WhenUnAuthorized()
        {
            // Arrange
            var blogDB = new Blog()
            {
                Id = 1,
                Description = "...",
                Title = "...",
                UserId = "not current user id"
            };

            var blogReq = new BlogRequest()
            {
                Id = 1,
                Description = "test edit",
                Title = "..."
            };

            _unitOfWork.Setup((x) => x.BlogRepository.GetOneAsync(It.IsAny<Expression<Func<Blog, bool>>>(), default!, default!))
                .ReturnsAsync(blogDB);

            // Act
            var result = _blogsController.Put(blogDB.Id, blogReq);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
            Assert.AreNotEqual(blogDB.Description, blogReq.Description);
        }


        [Test]
        public void Delete_ReturnsNoContent_WhenBlogExistsAndAuthorized()
        {
            //Arrange
            var blog = new Blog()
            {
                Id = 1,
                Description = "main",
                Title = "main",
                UserId = userId
            };

            _unitOfWork.Setup((x) => x.BlogRepository.GetOneAsync(It.IsAny<Expression<Func<Blog, bool>>>(), default!, default!))
                .ReturnsAsync(blog);

            //Act
            var result = _blogsController.Delete(blog.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }


        [Test]
        public void Follow_ReturnsOk_WhenBlogBelongsToAnotherUser()
        {
            // Arrange
            var blog = new Blog()
            {
                Id = 1,
                Description = "main",
                Title = "main",
                UserId = Guid.NewGuid().ToString()
            };

            _unitOfWork.Setup((x) => x.BlogRepository.GetOneAsync(It.IsAny<Expression<Func<Blog, bool>>>(), default!, default!))
            .ReturnsAsync(blog);
            
            // Act
            var result = _blogsController.Follow(blog.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkResult>());

        }

        [Test]
        public void Follow_ReturnsBadRequest_WhenBlogBelongsToSameUser()
        {
            // Arrange
            var blog = new Blog()
            {
                Id = 1,
                Description = "main",
                Title = "main",
                UserId = userId
            };

            _unitOfWork.Setup((x) => x.BlogRepository.GetOneAsync(It.IsAny<Expression<Func<Blog, bool>>>(), default!, default!))
            .ReturnsAsync(blog);

            // Act
            var result = _blogsController.Follow(blog.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<BadRequestResult>());

        }


        [Test]
        public void Unfollow_ReturnsOk_WhenBlogBelongsToAnotherUser()
        {
            // Arrange
            var blog = new Blog()
            {
                Id = 1,
                Description = "main",
                Title = "main",
                UserId = Guid.NewGuid().ToString()
            };

            _unitOfWork.Setup((x) => x.BlogRepository.GetOneAsync(It.IsAny<Expression<Func<Blog, bool>>>(), default!, default!))
            .ReturnsAsync(blog);

            // Act
            var result = _blogsController.UnFollow(blog.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkResult>());

        }
    }
}
