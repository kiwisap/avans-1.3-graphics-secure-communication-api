using Microsoft.AspNetCore.Http;
using Moq;
using MySecureBackend.WebApi.Services;
using System.Security.Claims;

namespace MySecureBackend.Tests;

[TestClass]
public sealed class AspNetIdentityAuthenticationServiceTests
{
    [TestMethod]
    public void GetCurrentAuthenticatedUserId_WithAuthenticatedUser_ReturnsUserId()
    {
        // Arrange
        const string expectedUserId = "test-user-123";
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, expectedUserId),
            new(ClaimTypes.Name, "testuser@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var authService = new AspNetIdentityAuthenticationService(httpContextAccessor.Object);

        // Act
        var userId = authService.GetCurrentAuthenticatedUserId();

        // Assert
        Assert.AreEqual(expectedUserId, userId);
    }

    [TestMethod]
    public void GetCurrentAuthenticatedUserId_WithNoNameIdentifierClaim_ReturnsNull()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "testuser@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var authService = new AspNetIdentityAuthenticationService(httpContextAccessor.Object);

        // Act
        var userId = authService.GetCurrentAuthenticatedUserId();

        // Assert
        Assert.IsNull(userId);
    }

    [TestMethod]
    public void GetCurrentAuthenticatedUserId_WithUnauthenticatedUser_ReturnsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity(); // No authentication type
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var authService = new AspNetIdentityAuthenticationService(httpContextAccessor.Object);

        // Act
        var userId = authService.GetCurrentAuthenticatedUserId();

        // Assert
        Assert.IsNull(userId);
    }
}
