using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    void CreateUser(User user);
    IEnumerable<User> FilterByActive(bool isActive);
    IEnumerable<User> GetAll();
    User? GetUserById(int userId);
    bool DeleteUserById(int userId);
    void UpdateUser(User user);
}
