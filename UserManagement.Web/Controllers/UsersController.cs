using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

/* Attribute to map incoming requests that match 
the 'Users' route pattern to this controller. */ 
[Route("users")]
/* Class definition for the 'UsersController' which is 
derived from, or inherits from, the 'Controller' class.
Primary constructor used to make code concise */
public class UsersController(IUserService userService) : Controller
{
    /* Assigns the injected userService to a 
    private read-only field '_userService' */
    private readonly IUserService _userService = userService;

    /* Specifies that the following action method 
    is only intended to handle HTTP GET requests */
    [HttpGet]
    public ViewResult List(string status = "all")
    {
        IEnumerable<User> users = status.ToLower() switch
        {
            // If status is "active", call the FilterByActive method with true as an argument.
            "active" => _userService.FilterByActive(true),
            // If status is "inactive", call the FilterByActive method with false as an argument.
            "inactive" => _userService.FilterByActive(false),
            // If status is not either of the above, call the GetAll method to retrieve all users.
            _ => _userService.GetAll(),
        };
        // Ensure users are sorted by Id in ascending order.
        var sortedUsers = users.OrderBy(user => user.Id);
        /* Converts the query results into a list of UserListItemViewModel 
        objects which allows the passing of only necessary data to the view 
        (5 attributes listed), enhancing both security and performance by not 
        exposing all data (other sensitive attributes retrieved from the query). */
        var items = sortedUsers.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth,
            IsActive = p.IsActive
        }).ToList();
        /* Prepare the view model (UserListViewModel) for the user list page.
        Aggregates all the user items in the UserListItemViewModel ('items') into a 
        single object ('model'), making it easier to manage and pass to the view. */
        var model = new UserListViewModel
        {
            Items = items
        };
        /* Returns the view for displaying the user list.
        The model containing the list of users is passed to the view,
        enabling the user data to be rendered. */
        return View(model);
    }

    [HttpPost("add")]
    public IActionResult AddUser(User user)
    {
        if (ModelState.IsValid)
        {
            _userService.CreateUser(user);
            ViewBag.Success = "User created successfully!";
            return PartialView("_AddUserModal", new User()); // Return a new empty form with a success message
        }
        return PartialView("_AddUserModal", user); // Return with validation messages if any
    }

    [HttpPost("delete")]
    public IActionResult DeleteUser(int userId)
    {
        // Attempt to delete the user using the service
        var isDeleted = _userService.DeleteUserById(userId);
        if (isDeleted)
        {
            ViewBag.Success = "User deleted successfully!";
        }
        else
        {
            ViewBag.ErrorMessage = "User not found.";
        }
        return PartialView("_DeleteUserModal");
    }

    [HttpPost("update")]
    public IActionResult EditUser(User user, int userId)
    {
        if (ModelState.IsValid)
        {
            var userToUpdate = _userService.GetUserById(userId);
            if (userToUpdate != null)
            {
                // Apply changes to the fetched user.
                userToUpdate.Forename = user.Forename;
                userToUpdate.Surname = user.Surname;
                userToUpdate.Email = user.Email;
                userToUpdate.DateOfBirth = user.DateOfBirth;
                userToUpdate.IsActive = user.IsActive;
                // UpdateUser with the updated entity.
                _userService.UpdateUser(userToUpdate);
                ViewBag.Success = "User updated successfully!";
                // Return the updated user
                return PartialView("_EditUserModal", userToUpdate);
            }
            else
            {
                ViewBag.ErrorMessage = "User not found.";
            }
        }
        return PartialView("_EditUserModal", user);
    }

}