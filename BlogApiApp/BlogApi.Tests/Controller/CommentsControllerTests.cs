using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using BlogApi.Controllers;
using DataAccess.Repositories.Interfaces;
using Models.ApiModels;
using Models.Entities;
using NUnit.Framework;
using AutoMapper;
using Moq;


namespace BlogApi.Tests.Controller
{
    [TestFixture]
    public class CommentsControllerTests
    {

        private readonly CommentsController _commentsController;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly string userId = Guid.NewGuid().ToString();
        private Post _post;

        public CommentsControllerTests()
        {
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _commentsController = new CommentsController(_unitOfWork.Object, _mapper.Object);
        }


        [SetUp]
        public void Setup()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "username"),
                new Claim(ClaimTypes.Name, "whoever@whatever.com"),
                new Claim("uid", userId),
            }, "TestAuthentication"));

            _commentsController.ControllerContext = new ControllerContext();
            _commentsController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            _post = new Post()
            {
                Id = 1,
                UserId = userId,
            };

            _unitOfWork.Setup((x) => x.PostRepository.Get(
                It.IsAny<Expression<Func<Post, bool>>>(), default!, default!
                )).Returns(_post);

            _unitOfWork.Setup((x) => x.CommentRepository.Add(It.IsAny<Comment>()));
            _unitOfWork.Setup((x) => x.CommentRepository.Remove(It.IsAny<Comment>()));
            _unitOfWork.Setup((x) => x.CommentRepository.Update(It.IsAny<Comment>()));
            _unitOfWork.Setup((x) => x.CommentRepository.AddLike(It.IsAny<int>(), It.IsAny<string>()));
            _unitOfWork.Setup((x) => x.CommentRepository.RemoveLike(It.IsAny<int>(), It.IsAny<string>()));

        }


        [Test]
        public void GetByPostId_ReturnsListOfComments()
        {
            //Arrange
            var commentsDB = new Mock<List<Comment>>();
            var commentsResponse = new Mock<List<CommentResponse>>();

            _unitOfWork.Setup(
                (x) => x.CommentRepository.GetAll(
                    It.Is<int>(x => x == _post.Id),
                    It.Is<string>(x => x == userId)
                )).Returns(commentsDB.Object);

            _mapper.Setup(x => x.Map<List<CommentResponse>>(commentsDB.Object))
                .Returns(commentsResponse.Object);

            //Act
            var result = _commentsController.GetComments(_post.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }


        [Test]
        public void Post_Returns201Created_WhenSuccess()
        {
            //Arrange
            CommentRequest commentModel = new CommentRequest
            {
                PostId = _post.Id,
                Content = "test content",
            };
            var comment = new Mock<Comment>();

            _mapper.Setup(x => x.Map<Comment>(commentModel))
                .Returns(comment.Object);

            //Act
            var result = _commentsController.Post(_post.Id, commentModel);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<CreatedResult>());

        }


        [Test]
        public void Edit_ReturnsNoContent_WhenExistsANDAuthorized()
        {
            // Arrange
            Comment commentDB = new Comment()
            {
                Id = 1,
                UserId = userId,
                Content = "content",
            };

            CommentRequest commentModel = new CommentRequest
            {
                PostId = _post.Id,
                Content = "test edit",
            };

            _unitOfWork.Setup((x) => x.CommentRepository
                .Get(It.IsAny<Expression<Func<Comment, bool>>>(), default!, default!))
                .Returns(commentDB);

            // Act
            var result = _commentsController.Put(_post.Id, commentModel.Id, commentModel);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NoContentResult>());
            Assert.That(commentModel.Content, Is.EqualTo(commentModel.Content));
        }


        [Test]
        public void Delete_ReturnsNoContent_WhenBlogExistsAndAuthorized()
        {
            //Arrange
            var comment = new Comment()
            {
                Id = 1,
                UserId = "some user's id",
            };

            _unitOfWork.Setup((x) => x.CommentRepository
                .Get(It.IsAny<Expression<Func<Comment, bool>>>(), default!, default!))
                .Returns(comment);

            //Act
            var result = _commentsController.Delete(_post.Id, comment.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }


        [Test]
        public void LikeComment_ReturnsOkResult_WhenCommentExists()
        {
            //Arrange
            var comment = new Comment()
            {
                Id = 1,
                UserId = "some user's id",
            };

            _unitOfWork.Setup((x) => x.CommentRepository
                .Get(It.IsAny<Expression<Func<Comment, bool>>>(), default!, default!))
                .Returns(comment);

            //Act
            var result = _commentsController.LikeComment(_post.Id, comment.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkResult>());
        }

        [Test]
        public void RemoveLike_ReturnsOkResult_WhenCommentExists()
        {
            //Arrange
            var comment = new Comment()
            {
                Id = 1,
                UserId = userId,
            };

            _unitOfWork.Setup((x) => x.CommentRepository
                .Get(It.IsAny<Expression<Func<Comment, bool>>>(), default!, default!))
                .Returns(comment);

            //Act
            var result = _commentsController.RemoveLike(_post.Id, comment.Id);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OkResult>());
        }


    }
}
