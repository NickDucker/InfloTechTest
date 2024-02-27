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
}
