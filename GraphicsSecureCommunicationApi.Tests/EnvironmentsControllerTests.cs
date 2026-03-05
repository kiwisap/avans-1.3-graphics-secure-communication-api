using Microsoft.AspNetCore.Mvc;
using Moq;
using GraphicsSecureCommunicationApi.WebApi.Controllers;
using GraphicsSecureCommunicationApi.WebApi.Models;
using GraphicsSecureCommunicationApi.WebApi.Repositories;
using GraphicsSecureCommunicationApi.WebApi.Services;

namespace GraphicsSecureCommunicationApi.Tests;

[TestClass]
public sealed class EnvironmentsControllerTests
{
    private EnvironmentsController _controller;
    private Mock<IEnvironment2DRepository> _repository;
    private Mock<IAuthenticationService> _authService;
    private const string TestUserId = "test-user-123";

    [TestInitialize]
    public void Setup()
    {
        _repository = new Mock<IEnvironment2DRepository>();
        _authService = new Mock<IAuthenticationService>();
        _controller = new EnvironmentsController(_repository.Object, _authService.Object);
        
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(TestUserId);
    }

    [TestMethod]
    public async Task GetAll_WithAuthenticatedUser_ReturnsOkWithEnvironments()
    {
        // Arrange
        var environments = new List<Environment2D>
        {
            new() { Id = 1, Name = "Test Env 1", MaxHeight = 100, MaxLength = 100, UserId = TestUserId },
            new() { Id = 2, Name = "Test Env 2", MaxHeight = 200, MaxLength = 200, UserId = TestUserId }
        };
        
        _repository.Setup(x => x.GetAllByUserIdAsync(TestUserId)).ReturnsAsync(environments);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnedEnvironments = okResult.Value as IEnumerable<Environment2D>;
        Assert.IsNotNull(returnedEnvironments);
        Assert.AreEqual(2, returnedEnvironments.Count());
    }

    [TestMethod]
    public async Task GetAll_WithUnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(null as string);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.IsInstanceOfType<UnauthorizedResult>(result.Result);
    }

    [TestMethod]
    public async Task GetById_WithValidId_ReturnsOkWithEnvironment()
    {
        // Arrange
        var environment = new Environment2D 
        { 
            Id = 1, 
            Name = "Test Env", 
            MaxHeight = 100, 
            MaxLength = 100, 
            UserId = TestUserId 
        };
        
        _repository.Setup(x => x.GetByIdAsync(1, TestUserId)).ReturnsAsync(environment);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnedEnvironment = okResult.Value as Environment2D;
        Assert.IsNotNull(returnedEnvironment);
        Assert.AreEqual(1, returnedEnvironment.Id);
    }

    [TestMethod]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _repository.Setup(x => x.GetByIdAsync(999, TestUserId)).ReturnsAsync(null as Environment2D);

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
    public async Task Create_WithValidEnvironment_ReturnsCreatedAtAction()
    {
        // Arrange
        var newEnvironment = new Environment2D 
        { 
            Name = "New Env", 
            MaxHeight = 150, 
            MaxLength = 150 
        };
        
        var createdEnvironment = new Environment2D 
        { 
            Id = 1, 
            Name = "New Env", 
            MaxHeight = 150, 
            MaxLength = 150, 
            UserId = TestUserId 
        };
        
        _repository.Setup(x => x.CreateAsync(It.IsAny<Environment2D>())).ReturnsAsync(createdEnvironment);

        // Act
        var result = await _controller.Create(newEnvironment);

        // Assert
        Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(_controller.GetById), createdResult.ActionName);
        var returnedEnvironment = createdResult.Value as Environment2D;
        Assert.IsNotNull(returnedEnvironment);
        Assert.AreEqual(TestUserId, returnedEnvironment.UserId);
    }

    [TestMethod]
    public async Task Create_WithUnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(null as string);
        var newEnvironment = new Environment2D { Name = "New Env", MaxHeight = 150, MaxLength = 150 };

        // Act
        var result = await _controller.Create(newEnvironment);

        // Assert
        Assert.IsInstanceOfType<UnauthorizedResult>(result.Result);
    }

    [TestMethod]
    public async Task Update_WithValidEnvironment_ReturnsNoContent()
    {
        // Arrange
        var environment = new Environment2D 
        { 
            Id = 1, 
            Name = "Updated Env", 
            MaxHeight = 200, 
            MaxLength = 200, 
            UserId = TestUserId 
        };
        
        _repository.Setup(x => x.UpdateAsync(environment, TestUserId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Update(1, environment);

        // Assert
        Assert.IsInstanceOfType<NoContentResult>(result);
    }

    [TestMethod]
    public async Task Update_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var environment = new Environment2D 
        { 
            Id = 2, 
            Name = "Updated Env", 
            MaxHeight = 200, 
            MaxLength = 200 
        };

        // Act
        var result = await _controller.Update(1, environment);

        // Assert
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }

    [TestMethod]
    public async Task Update_WithNonExistentEnvironment_ReturnsNotFound()
    {
        // Arrange
        var environment = new Environment2D 
        { 
            Id = 999, 
            Name = "Updated Env", 
            MaxHeight = 200, 
            MaxLength = 200 
        };
        
        _repository.Setup(x => x.UpdateAsync(environment, TestUserId)).ReturnsAsync(false);

        // Act
        var result = await _controller.Update(999, environment);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
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
    public async Task Delete_WithNonExistentEnvironment_ReturnsNotFound()
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
