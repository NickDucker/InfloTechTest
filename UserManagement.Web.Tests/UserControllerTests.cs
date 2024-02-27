using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    // Arrange: Initializes objects and sets the value of the data that is passed to the methods under test.
    private readonly UsersController _controller;
    private readonly User[] _users;
    private readonly Mock<IUserService> _userServiceMock = new();
    public UserControllerTests()
    {
        // Initialize the UsersController with the mocked IUserService in the constructor
        _controller = new UsersController(_userServiceMock.Object);
        // Set up users for all tests
        _users = SetupUsers(
            ("Active User 1", "User", "active1@example.com", true),
            ("Active User 2", "User", "active2@example.com", true),
            ("Inactive User 1", "User", "inactive@example.com", false),
            ("Inactive User 2", "User", "inactive@example.com", false)
        );
    }

    private User[] SetupUsers(params (string Forename, string Surname, string Email, bool IsActive)[] userParams)
    {
        var users = userParams.Select(u => new User
        {
            Forename = u.Forename,
            Surname = u.Surname,
            Email = u.Email,
            IsActive = u.IsActive
        }).ToArray();

        // Prepares the mock IUserService to run with the predefined 'users' list in the testing environment
        _userServiceMock.Setup(s => s.GetAll()).Returns(users);
        _userServiceMock.Setup(s => s.FilterByActive(It.IsAny<bool>())).Returns((bool isActive) => users.Where(u => u.IsActive == isActive));

        return users;
    }

    [Fact]
    public void List_WhenCalledWithAll_ShowsAllUsers()
    {
        // Act
        var result = _controller.List("all");
        // Assert
        result.Model.Should().BeOfType<UserListViewModel>("because the model should not be null and of the type UserListViewModel")
            .Which.Items.Should().BeEquivalentTo(_users, "because the data doesn't match the original user data");
    }

    [Fact]
    public void List_WhenCalledWithActive_ShowsOnlyActiveUsers()
    {
        // Act
        var result = _controller.List("active");
        // Assert
        result.Model.Should().BeOfType<UserListViewModel>("because the model should not be null and of the type UserListViewModel")
            .Which.Items.Should().HaveCount(2, "because there should be exactly 2 users in the list")
            .And.Subject.All(u => u.IsActive).Should().BeTrue("because all users should be active");
    }

    [Fact]
    public void List_WhenCalledWithInactive_ShowsOnlyInactiveUsers()
    {
        // Act
        var result = _controller.List("inactive");
        // Assert
        result.Model.Should().BeOfType<UserListViewModel>("because the model should not be null and of the type UserListViewModel")
            .Which.Items.Should().HaveCount(2, "because there should be exactly 2 users in the list")
            .And.Subject.All(u => u.IsActive).Should().BeFalse("because all users should be active");
    }
}