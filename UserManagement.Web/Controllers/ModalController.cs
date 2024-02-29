using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.WebMS.Controllers;

[Route("modals")]
public class ModalController(IUserService userService) : Controller
{
    private readonly IUserService _userService = userService;

    // Gets the add user modal to appear.
    [HttpGet("add-user-modal")]
    public IActionResult AddUserModalContent()
    {
        // Returns the PartialView that represents the modal content.
        return PartialView("_AddUserModal");
    }

    // Gets the add user modal to appear.
    [HttpGet("view-user-modal")]
    public IActionResult ViewUserModalContent(int userId)
    {
        var user = _userService.GetUserById(userId);
        if (user == null)
        {
            // Handle the case where the user isn't found.
            ViewBag.ErrorMessage = "User not found.";
            return PartialView("_ViewUserModal");
        }

        // Returns the PartialView that represents the modal content.
        return PartialView("_ViewUserModal", user);
    }

    [HttpGet("edit-user-modal")]
    public IActionResult EditUserModalContent(int userId)
    {
        var user = _userService.GetUserById(userId);
        // Handle the case where the user is not found.
        if (user == null)
        {
            ViewBag.ErrorMessage = "User not found.";
            return PartialView("_EditUserModal");
        }
        // Returns the PartialView that represents the modal content.
        return PartialView("_EditUserModal", user);
    }

    [HttpGet("delete-user-modal")]
    public IActionResult DeleteUserModalContent(int userId)
    {
        var user = _userService.GetUserById(userId);
        // Handle the case where the user is not found.
        if (user == null)
        {
            ViewBag.ErrorMessage = "User not found.";
            return PartialView("_DeleteUserModal");
        }
        // Returns the PartialView that represents the modal content.
        return PartialView("_DeleteUserModal", user);
    }
}