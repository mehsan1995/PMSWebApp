// document.getElementById('toggleSidebar').addEventListener('click', function () {
//     document.getElementById('desktopSidebar').classList.toggle('collapsed');
// });


// function toggleSidebar() {
//   document.getElementById("sidebar").classList.toggle("collapsed");
//   document.querySelector(".content").classList.toggle("collapsed");
// }



$('#departmentUsersModal').on('shown.bs.modal', function () {
    let departmentId = $(this).data("department-id");

    if ($.fn.select2 && $('#departmentUsers').hasClass("select2-hidden-accessible")) {
        $('#departmentUsers').select2('destroy');
    }


    // Initialize Select2
    $('#departmentUsers').select2({
        dropdownParent: $('#departmentUsersModal'), // Fixes z-index in modal
        placeholder: "Search for a user",
        minimumInputLength: 2,
        ajax: {
            url: '/Users/GetAllUsers', // Your API endpoint
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term || ''
                };
            },
            processResults: function (data) {
                
                return {
                    results: data // Must match API output
                };
            },
            cache: true
        }
    });

    // Initialize Select2
    $('#departmentPermission').select2({
        dropdownParent: $('#departmentUsersModal'), // Fixes z-index in modal
        placeholder: "Search for a user",
        minimumInputLength: 2,
        ajax: {
            url: '/Roles/GetAllRoles', // Your API endpoint
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    searchTerm: params.term || ''
                };
            },
            processResults: function (data) {
                
                return {
                    results: data // Must match API output
                };
            },
            cache: true
        }
    });



    // $('#departmentUsers').on('select2:select', function (e) {
    //     debugger
    //      let  selectedUser = e.params.data;
    //     console.log("Selected user:", selectedUser);


    //   $.ajax({
    //     url: '/DepartmentUsers/Add',
    //     dataType: 'json',
    //     type: 'POST',
    //     contentType: 'application/json',
    //     data:JSON.stringify({
    //     departmentId: departmentId,
    //     userId: selectedUser.id

    //     }),
    //     success: function (data) {
    //         alert(`User ${selectedUser.text} added to department!`);
    //         $('#departmentUsers').val(null).trigger('change');
    //     },
    //     error: function (xhr) {
    //         console.error(xhr.responseText);
    //         alert("Failed to add user.");
    //     }
    //    });

    //     });
});



function logout() {

    $('#logoutForm').submit();
}
$(document).ready(function () {



    $('.toggle-password').on('click', function () {
        const passwordInput = $('.password-input');
        const icon = $(this).find('i');

        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
            icon.removeClass('bi-eye').addClass('bi-eye-slash');
        } else {
            passwordInput.attr('type', 'password');
            icon.removeClass('bi-eye-slash').addClass('bi-eye');
        }
    });
});