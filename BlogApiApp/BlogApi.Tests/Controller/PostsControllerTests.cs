using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Models.ApiModels;
using Models.Entities;
using DataAccess.Repositories.Interfaces;
using BlogApi.Controllers;
using NUnit.Framework;
using AutoMapper;
using Moq;

namespace BlogApi.Tests.Controller
{
    [TestFixture]
    public class PostsControllerTests
    {

        private readonly PostsController _postsController;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly string userId = Guid.NewGuid().ToString();
        private Blog _blog;

        public PostsControllerTests()
        {
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _postsController = new PostsController(_unitOfWork.Object, _mapper.Object);
        }


        [SetUp]
        public void Setup()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "username"),
                new Claim(ClaimTypes.Name, "whoever@whatever.com"),
                new Claim("uid", userId),
            }, "TestAuthentication"));

            _postsController.ControllerContext = new ControllerContext();
            _postsController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            _blog = new Blog()
            {
                Id = 1,
                UserId = userId,
                Description = "main",
                Title = "main"
            };

            _unitOfWork.Setup((x) => x.BlogRepository.Get(
                It.IsAny<Expression<Func<Blog, bool>>>(), default!, default!
                )).Returns(_blog);
            _unitOfWork.Setup((x) => x.PostRepository.Add(It.IsAny<Post>()));
            _unitOfWork.Setup((x) => x.PostRepository.Remove(It.IsAny<Post>()));
        }


        [Test]
        public void GetPosts_Returns200OK()
        {
            //Arrange
            var postsDB = new Mock<List<Post>>();
            var postsResponseList = new Mock<List<PostResponse>>();

            _mapper.Setup(x => x.Map<List<PostResponse>>(postsDB.Object))
                .Returns(postsResponseList.Object);

            _unitOfWork.Setup(
                (x) => x.PostRepository.GetAll(
                    It.Is<int>(x => x == _blog.Id), 
                    It.Is<string>(x => x == userId)
                )).Returns(postsDB.Object);

            //Act
            var result = _postsController.GetPage(_blog.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }


        [Test]
        public void Post_Returns201Created_WhenSuccess()
        {
            //Arrange
            PostRequest postModel = new PostRequest
            {
                BlogId = _blog.Id,
                
            };
            var postResult = new Mock< PostResponse>();
            var post = new Mock<Post>();

            _mapper.Setup(x => x.Map<Post>(postModel))
                .Returns(post.Object);

            //Act
            var result = _postsController.Post(postModel);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<CreatedResult>());

        }


        [Test]
        public void Put_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            Post postDB = new Post()
            {
                Id = 1,
                UserId = userId,
                BlogId = _blog.Id,
                Content = "content",
                HeadLine = ""
            };

            PostRequest postModel = new PostRequest
            {
                PostId = postDB.Id,
                BlogId = _blog.Id,
                Content = "test edit",
                HeadLine = ""
            };

            _unitOfWork.Setup((x) => x.PostRepository.Get(It.IsAny<Expression<Func<Post, bool>>>(), default!, default!))
                .Returns(postDB);

            // Act
            var result = _postsController.Put(postDB.Id, postModel);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NoContentResult>());
            Assert.That(postDB.Content, Is.EqualTo(postModel.Content));
        }


        [Test]
        public void Delete_ReturnsNoContent_WhenBlogExistsAndAuthorized()
        {
            //Arrange
            var post = new Post()
            {
                Id = 1,
                UserId = userId,
                BlogId = _blog.Id
            };

            _unitOfWork.Setup((x) => x.PostRepository.Get(It.IsAny<Expression<Func<Post, bool>>>(), default!, default!))
                .Returns(post);

            //Act
            var result = _postsController.Delete(post.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }


        [Test]
        public void LikePost_ReturnsOkResult_WhenPostExists()
        {
            //Arrange
            var post = new Post()
            {
                Id = 1,
                UserId = "some user's id",
            };

            _unitOfWork.Setup((x) => x.PostRepository
                .Get(It.IsAny<Expression<Func<Post, bool>>>(), It.IsAny<string>(), default!))
                .Returns(post);

            //Act
            var result = _postsController.LikePost(post.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkResult>());
        }

        [Test]
        public void UnLikePost_ReturnsOkResult_WhenPostExists()
        {
            //Arrange
            var post = new Post()
            {
                Id = 1,
                UserId = "some user's id",
            };

            _unitOfWork.Setup((x) => x.PostRepository
                .Get(It.IsAny<Expression<Func<Post, bool>>>(), It.IsAny<string>(), default!))
                .Returns(post);

            //Act
            var result = _postsController.UnLikePost(post.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkResult>());
        }

    }
}
