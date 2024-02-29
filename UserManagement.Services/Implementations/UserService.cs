using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService(IDataContext dataAccess) : IUserService
{
    private readonly IDataContext _dataAccess = dataAccess;

    // Method to fetch and list all users based on status.
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>().Where(u => u.IsActive == isActive);
    }

    // Method to fetch and list all users.
    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();

    public void CreateUser(User user)
    {
        _dataAccess.Create(user);
    }
    // Nullable because might not return a user if the id does not match.
    public User? GetUserById(int userId)
    {
        return _dataAccess.GetAll<User>().FirstOrDefault(u => u.Id == userId);
    }
    public bool DeleteUserById(int userId)
    {
        var user = _dataAccess.GetAll<User>().FirstOrDefault(u => u.Id == userId);
        if (user == null) return false;
        _dataAccess.Delete(user);
        return true;
    }
    public void UpdateUser(User user)
    {
        var existingUser = _dataAccess.GetAll<User>().FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            // Update the properties of the existing user entity.
            existingUser.Forename = user.Forename;
            existingUser.Surname = user.Surname;
            existingUser.Email = user.Email;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.IsActive = user.IsActive;
            // Apply changes by calling Update function and passing in new user entity with updated properties.
            _dataAccess.Update(existingUser);
        }
        else
        {
            throw new KeyNotFoundException("User not found.");
        }
    }
}
