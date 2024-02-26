using System.Linq;
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
    // Method to fetch and list all users based on status.
    public ViewResult List(string status = "all")
    {
        /* Base query to fetch all users, 
        adjusted based on status parameter. */
        var query = _userService.GetAll();

        switch (status.ToLower())
        {
            case "active":
                // Base query modified to fetch only active users.
                query = query.Where(u => u.IsActive);
                break;
            case "inactive":
                // Base qery modified to fetch only inactive users.
                query = query.Where(u => !u.IsActive);
                break;
            /* Modifications are not required to fetch
            all users, as this is the base query. */
        }

        /* Converts the query results into a list of UserListItemViewModel 
        objects which allows the passing of only necessary data to the view 
        (5 attributes listed), enhancing both security and performance by not 
        exposing all data (other sensitive attributes retrieved from the query). */
        var items = query.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
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
}