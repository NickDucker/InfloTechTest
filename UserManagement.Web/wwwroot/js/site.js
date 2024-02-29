// Loads the modal when document is ready and one of the four buttons (Add, Edit, View, Delete) is clicked.
$(document).ready(function () {
    // When any button that opens userModal is clicked:
    $('a[data-bs-target="#modal"]').on('click', function() {
        // Get the user Id from the anchor's data-user-id attribute.
        var userId = $(this).data('user-id');
        // Get the content URL from the anchor's data-content-url attribute.
        var contentUrl = $(this).attr('data-content-url') + "?userId=" + userId; // Append the user ID to the URL.
        // Get the title from the anchor's data-title attribute.
        var title = $(this).attr('data-title');
        // Set the modal title.
        $('#modal').find('.modal-title').text(title);
        // Load the dynamic content into the modal's content area.
        $('#modalContent').load(contentUrl);
    });

    // Event listener for when the modal is closed (resets form to original state).
    $('#modal').on('hidden.bs.modal', function (e) {
        // Check if there's a success message.
        if ($('#modalContent').find('.alert-success').length > 0) {
            // Reload the form content only
            $("#modalContent").load("/modals/add-user-modal", function() {
                // Remove the success message after reloading the form.
                $('.alert-success').remove();
                // Reload the current page to update user list.
                window.location.reload();
            });
        }
    });

    // Posts form data and displays validation messages based on inputs.
    $('body').on('submit', '.ajax-form', function (e) {
        // Prevent the form from submitting via the browser's default method.
        e.preventDefault();
        var form = $(this);
        $.ajax({
            // Get the URL from the form's action attribute.
            url: form.attr('action'),
            type: 'POST',
             // Serialize the form data.
            data: form.serialize(),
            success: function (result) {
                // Update the modal's content with the returned HTML.
                $('#modalContent').html(result);   
            },
            error: function () {
                alert('An error occurred while processing your request. Please try again.');
            }
        });
    });
});