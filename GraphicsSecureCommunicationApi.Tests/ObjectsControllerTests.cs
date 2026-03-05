using Microsoft.AspNetCore.Mvc;
using Moq;
using GraphicsSecureCommunicationApi.WebApi.Controllers;
using GraphicsSecureCommunicationApi.WebApi.Models;
using GraphicsSecureCommunicationApi.WebApi.Repositories;
using GraphicsSecureCommunicationApi.WebApi.Services;

namespace GraphicsSecureCommunicationApi.Tests;

[TestClass]
public sealed class ObjectsControllerTests
{
    private ObjectsController _controller;
    private Mock<IObject2DRepository> _repository;
    private Mock<IAuthenticationService> _authService;
    private const string TestUserId = "test-user-123";

    [TestInitialize]
    public void Setup()
    {
        _repository = new Mock<IObject2DRepository>();
        _authService = new Mock<IAuthenticationService>();
        _controller = new ObjectsController(_repository.Object, _authService.Object);
        
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(TestUserId);
    }

    [TestMethod]
    public async Task GetByEnvironmentId_WithValidId_ReturnsOkWithObjects()
    {
        // Arrange
        var objects = new List<Object2D>
        {
            new() { Id = 1, EnvironmentId = 1, PositionX = 10, PositionY = 20 },
            new() { Id = 2, EnvironmentId = 1, PositionX = 30, PositionY = 40 }
        };
        
        _repository.Setup(x => x.GetAllByEnvironmentIdAsync(1, TestUserId)).ReturnsAsync(objects);

        // Act
        var result = await _controller.GetByEnvironmentId(1);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnedObjects = okResult.Value as IEnumerable<Object2D>;
        Assert.IsNotNull(returnedObjects);
        Assert.AreEqual(2, returnedObjects.Count());
    }

    [TestMethod]
    public async Task GetByEnvironmentId_WithUnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(null as string);

        // Act
        var result = await _controller.GetByEnvironmentId(1);

        // Assert
        Assert.IsInstanceOfType<UnauthorizedResult>(result.Result);
    }

    [TestMethod]
    public async Task GetById_WithValidId_ReturnsOkWithObject()
    {
        // Arrange
        var obj = new Object2D 
        { 
            Id = 1, 
            EnvironmentId = 1, 
            PositionX = 10, 
            PositionY = 20 
        };
        
        _repository.Setup(x => x.GetByIdAsync(1, TestUserId)).ReturnsAsync(obj);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnedObject = okResult.Value as Object2D;
        Assert.IsNotNull(returnedObject);
        Assert.AreEqual(1, returnedObject.Id);
    }

    [TestMethod]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _repository.Setup(x => x.GetByIdAsync(999, TestUserId)).ReturnsAsync(null as Object2D);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    [TestMethod]
    public async Task GetById_WithUnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(null as string);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.IsInstanceOfType<UnauthorizedResult>(result.Result);
    }

    [TestMethod]
    public async Task Create_WithValidObject_ReturnsCreatedAtAction()
    {
        // Arrange
        var newObject = new Object2D 
        { 
            EnvironmentId = 1, 
            PositionX = 10, 
            PositionY = 20,
            ScaleX = 1,
            ScaleY = 1
        };
        
        var createdObject = new Object2D 
        { 
            Id = 1, 
            EnvironmentId = 1, 
            PositionX = 10, 
            PositionY = 20,
            ScaleX = 1,
            ScaleY = 1
        };
        
        _repository.Setup(x => x.CreateAsync(It.IsAny<Object2D>(), TestUserId)).ReturnsAsync(createdObject);

        // Act
        var result = await _controller.Create(newObject);

        // Assert
        Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(_controller.GetById), createdResult.ActionName);
    }

    [TestMethod]
    public async Task Create_WithUnauthorizedEnvironment_ReturnsUnauthorized()
    {
        // Arrange
        var newObject = new Object2D { EnvironmentId = 1, PositionX = 10, PositionY = 20 };
        
        _repository.Setup(x => x.CreateAsync(It.IsAny<Object2D>(), TestUserId))
            .ThrowsAsync(new UnauthorizedAccessException("You don't have access to this environment."));

        // Act
        var result = await _controller.Create(newObject);

        // Assert
        Assert.IsInstanceOfType<UnauthorizedObjectResult>(result.Result);
    }

    [TestMethod]
    public async Task Create_WithUnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(null as string);
        var newObject = new Object2D { EnvironmentId = 1, PositionX = 10, PositionY = 20 };

        // Act
        var result = await _controller.Create(newObject);

        // Assert
        Assert.IsInstanceOfType<UnauthorizedResult>(result.Result);
    }

    [TestMethod]
    public async Task Update_WithValidObject_ReturnsNoContent()
    {
        // Arrange
        var obj = new Object2D 
        { 
            Id = 1, 
            EnvironmentId = 1, 
            PositionX = 15, 
            PositionY = 25 
        };
        
        _repository.Setup(x => x.UpdateAsync(obj, TestUserId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Update(1, obj);

        // Assert
        Assert.IsInstanceOfType<NoContentResult>(result);
    }

    [TestMethod]
    public async Task Update_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var obj = new Object2D { Id = 2, EnvironmentId = 1, PositionX = 15, PositionY = 25 };

        // Act
        var result = await _controller.Update(1, obj);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }

    [TestMethod]
    public async Task Update_WithNonExistentObject_ReturnsNotFound()
    {
        // Arrange
        var obj = new Object2D { Id = 999, EnvironmentId = 1, PositionX = 15, PositionY = 25 };
        
        _repository.Setup(x => x.UpdateAsync(obj, TestUserId)).ReturnsAsync(false);

        // Act
        var result = await _controller.Update(999, obj);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Update_WithUnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(null as string);
        var obj = new Object2D { Id = 1, EnvironmentId = 1, PositionX = 15, PositionY = 25 };

        // Act
        var result = await _controller.Update(1, obj);

        // Assert
        Assert.IsInstanceOfType<UnauthorizedResult>(result);
    }

    [TestMethod]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _repository.Setup(x => x.DeleteAsync(1, TestUserId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsInstanceOfType<NoContentResult>(result);
    }

    [TestMethod]
    public async Task Delete_WithNonExistentObject_ReturnsNotFound()
    {
        // Arrange
        _repository.Setup(x => x.DeleteAsync(999, TestUserId)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Delete_WithUnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(null as string);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsInstanceOfType<UnauthorizedResult>(result);
    }
}
