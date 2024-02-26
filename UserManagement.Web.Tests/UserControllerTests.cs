using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
    private readonly UsersController _controller;
    private readonly User[] _users;
    public UserControllerTests()
    {
        _controller = CreateController();
        _users = SetupUsers(
            ("Active User 1", "User", "active1@example.com", true),
            ("Active User 2", "User", "active2@example.com", true),
            ("Inactive User 1", "User", "inactive@example.com", false),
            ("Inactive User 2", "User", "inactive@example.com", false)
        );
    }
    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
    // Initializes multiple users with different properties by passing tuples that represent different users
    private User[] SetupUsers(params (string Forename, string Surname, string Email, bool IsActive)[] userParams)
    {
        /* u represents each tuple in userParams. For each u, a new User 
        object is instantiated and initialized with the values from u */
        var users = userParams.Select(u => new User
        {
            Forename = u.Forename,
            Surname = u.Surname,
            Email = u.Email,
            IsActive = u.IsActive
        }).ToArray();

        _userService.Setup(s => s.GetAll()).Returns(users.AsQueryable());

        return users;
    }

    // Test Methods
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Act: Invokes the method under test with the arranged parameters.
        var result = _controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            // Checks if result.Model is of the type UserListViewModel.
            .Should().BeOfType<UserListViewModel>("the model should not be null and of type UserListViewModel")
            /* UserListViewModel should be equivalent to the users array
            (the data inside them should be equivalent as all users should have been listed). */
            .Which.Items.Should().BeEquivalentTo(_users, "the data doesn't match the original user data");
    }

    [Fact]
    public void List_WhenCalledWithAll_ShowsAllUsers()
    {
        // Act
        var result = _controller.List("all"); // ViewModel type.

        // Assert
        /* Checks that the result.Model is of type UserListViewModel, and 
        if this passes then the result and result.model cannot be null. */
        result.Model.Should().BeOfType<UserListViewModel>("the model should not be null and of type UserListViewModel")
            /* Checks that the Items property of the 
            UserListViewModel should have exactly 4 items */
            .Which.Items.Should().HaveCount(4, "there should be exactly 4 items in the list");
    }

    [Fact]
    public void List_WhenCalledWithActive_ShowsOnlyActiveUsers()
    {
        // Act
        var result = _controller.List("active");

        // Assert
        /* Checks that the result.Model is of type UserListViewModel, and 
        if this passes then the result and result.model cannot be null. */
        result.Model.Should().BeOfType<UserListViewModel>("the model should not be null and of type UserListViewModel")
            /* Checks that the Items property of the 
            UserListViewModel should have exactly 4 items */
            .Which.Items.Should().HaveCount(2, "there should be exactly 2 items in the list")
            /* Checks that all User objects within the Items collection satisfy the condition 
            IsActive == true. The message provides a clear reason for if the assertion fails. */
            .And.Subject.All(u => u.IsActive).Should().BeTrue("all users should be active");
    }

    [Fact]
    public void List_WhenCalledWithInactive_ShowsOnlyInactiveUsers()
    {
        // Act
        var result = _controller.List("inactive");

        // Assert
        /* Checks that the result.Model is of type UserListViewModel, and 
        if this passes then the result and result.model cannot be null. */
        result.Model.Should().BeOfType<UserListViewModel>("the model should not be null and of type UserListViewModel")
            /* Checks that the Items property of the 
            UserListViewModel should have exactly 4 items */
            .Which.Items.Should().HaveCount(2, "there should be exactly 2 items in the list")
            /* Checks that all User objects within the Items collection satisfy the condition 
            IsActive == true. The message provides a clear reason for if the assertion fails. */
            .And.Subject.All(u => u.IsActive).Should().BeFalse("all users should be active");
    }
}
