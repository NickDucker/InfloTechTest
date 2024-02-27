using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    // Arrange: Initializes objects and sets the value of the data that is passed to the methods under test.
    private readonly UserService _service;
    private readonly IQueryable<User> _users;
    private readonly Mock<IDataContext> _dataContext = new();
    public UserServiceTests()
    {
        // Initialize the UserService with the mocked IDataContext in the constructor.
        _service = new UserService(_dataContext.Object);
        // Set up users for all tests.
        _users = SetupUsers(
            ("Active User 1", "User", "active1@example.com", true),
            ("Active User 2", "User", "active2@example.com", true),
            ("Inactive User 1", "User", "inactive1@example.com", false),
            ("Inactive User 2", "User", "inactive2@example.com", false)
        );
    }
    private IQueryable<User> SetupUsers(params (string Forename, string Surname, string Email, bool IsActive)[] userParams)
    {
        var users = userParams.Select(u => new User
        {
            Forename = u.Forename,
            Surname = u.Surname,
            Email = u.Email,
            IsActive = u.IsActive
        }).AsQueryable();

        // Prepares the mock IDataContext to run with the predefined 'users' list in the testing environment.
        _dataContext.Setup(s => s.GetAll<User>()).Returns(users);
        // No preparation required for the FilterByActive method because it is already part of the UserService we are testing.


        return users;
    }

    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Act: Invokes the method under test with the arranged parameters.
        var result = _service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(_users);
    }

    [Fact]
    public void FilterByActive_WhenIsActiveTrue_MustReturnOnlyActiveUsers()
    {
        // Act
        var result = _service.FilterByActive(true);

        // Assert: Verifies all users in the result are active.
        result.Should().OnlyContain(u => u.IsActive);
    }

    [Fact]
    public void FilterByActive_WhenIsActiveFalse_MustReturnOnlyInactiveUsers()
    {
        // Act
        var result = _service.FilterByActive(false);

        // Assert: Verifies all users in the result are inactive.
        result.Should().OnlyContain(u => !u.IsActive);
    }
}
