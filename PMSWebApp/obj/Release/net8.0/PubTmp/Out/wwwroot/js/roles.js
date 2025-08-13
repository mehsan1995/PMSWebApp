document.addEventListener("DOMContentLoaded", function () {
    // Event delegation for dynamically loaded content
    document.addEventListener("change", function (event) {
        // Handle "Select All"
        if (event.target.id === "selectAllModules") {
            const checkboxes = document.querySelectorAll('input[name="SelectedPermissionIds"]');
            checkboxes.forEach(cb => cb.checked = event.target.checked);
        }

        // Handle individual checkbox
        if (event.target.name === "SelectedPermissionIds" && !event.target.checked) {
            const selectAll = document.getElementById("selectAllModules");
            if (selectAll) {
                selectAll.checked = false;
            }
        }
    });
});


function viewRole(roleId) {
    $.ajax({
        url: '/RolePermissions/GetPermissions', // Update with actual controller
        type: 'GET',
        data: { roleId: roleId,isView:true },
        success: function (html) {
            $('#rolepermissionsModalContent').html("");
            $('#rolepermissionsModalContent').html(html);
            $('#rolePermissionsModal').modal('show');
        },
        error: function () {
            alert('Failed to load permissions.');
        }
    });
}
    // Load the permissions partial view
    function loadPermissions(roleId) {
        $.ajax({
            url: '/RolePermissions/GetPermissions', // Update with actual controller
            type: 'GET',
            data: { roleId: roleId },
            success: function (html) {
                $('#rolepermissionsModalContent').html("");
                $('#rolepermissionsModalContent').html(html);
                $('#rolePermissionsModal').modal('show');
            },
            error: function () {
                alert('Failed to load permissions.');
            }
        });
    }

    // Submit the form as JSON
    $(document).on('submit', '#permissionForm', function (e) {
        e.preventDefault();

    var roleId = $('#permissionForm input[name="RoleId"]').val();
    var selectedPermissionIds = [];

    $('#permissionForm input[name="SelectedPermissionIds"]:checked').each(function () {
        selectedPermissionIds.push(parseInt($(this).val()));
        });

    var token = $('#permissionForm input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/RolePermissions/GetPermissions', // This is your POST method
    type: 'POST',
    contentType: 'application/json',
    headers: {
        'RequestVerificationToken': token
            },
    data: JSON.stringify({
        RoleId: roleId,
    SelectedPermissionIds: selectedPermissionIds
            }),
    success: function (response) {
                if (response.success) {
        alert(response.message);
                } else {
        alert('Error: ' + response.message);
    console.log(response.errors);
                }
            },
    error: function () {
        alert('Failed to save permissions.');
            }
        });
    });



function CreateNewRole(id = null) {
    
    $.ajax({
        url: '/Roles/AddEdit' + (id ? '?id=' + id : ''),
        type: 'GET',
        success: function (html) {
            $('#roleModalContent').html(html);
            $('#roleModal').modal('show');

            // Rebind validation if form contains client-side validation
            $.validator.unobtrusive.parse('#createRoleForm');
        },
        error: function () {
            alert('Failed to load form.');
        }
    });
}

function DeleteRole(id) {
    if (!confirm('Are you sure you want to delete this role?')) return;

    $.ajax({
        url: `/Roles/Delete/${id}`,
        type: 'POST',
        success: function (response) {
            if (response.success) {
                alert(response.message);
                $('#rolesTable').DataTable().ajax.reload(); 
            } else {
                alert(`Error: ${response.message}\nDetails: ${response.detail}`);
            }
        },
        error: function () {
            alert('Request failed while trying to delete the role.');
        }
    });
}


function closeRoleModal() {
    // Hide the modal
    $('#roleModal').modal('hide');

    // Reset the form fields
    $('#createRoleForm')[0]?.reset(); // Safely reset if form exists

    // Remove any validation styles/messages
    $('#createRoleForm')
        .removeClass('was-validated')
        .find('.text-danger').empty(); // optional: clear server-side validation spans
}


$(document).on('submit', '#createRoleForm', function (e) {
    e.preventDefault();
    
    var $form = $(this);

    if ($form[0].checkValidity() === false) {
        $form.addClass('was-validated');
        return;
    }

    $.ajax({
        url: "Roles/AddEdit",
        method: 'POST',
        data: $form.serialize(),
        success: function (response) {
            if (response.success) {
                alert(response.message); 
                $('#roleModal').modal('hide');
                $form[0].reset();
                $form.removeClass('was-validated');
                $('#rolesTable').DataTable().ajax.reload(); 
            } else {
                alert(response.message); 
            }
        },
        error: function () {
            alert('Unexpected error occurred.');
        }
    });
});

$(document).on('click', '.toggle-status-btn', function () {
    const $button = $(this);
    const roleId = $button.data('id');
    const currentStatus = $button.data('status'); // true or false

    const action = currentStatus ? 'disable' : 'enable';
    if (!confirm(`Are you sure you want to ${action} this role?`)) return;

    $.ajax({
        url: `/Roles/ToggleStatus`,
        type: 'POST',
        data: {
            id: roleId,
            isActive: !currentStatus
        },
        success: function (response) {
            if (response.success) {
                alert(response.message);
    
                $button.text(!currentStatus ? 'Disable' : 'Enable');
                $button.data('status', !currentStatus);
                $('#rolesTable').DataTable().ajax.reload(); 
            } else {
                alert(`Error: ${response.message}`);
            }
        },
        error: function () {
            alert('An unexpected error occurred.');
        }
    });
});



document.getElementById('roleSearch').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
        var searchValue = document.getElementById('roleSearch').value;
        // For DataTable
        var table = $('#rolesTable').DataTable(); // Use jQuery to access the DataTable instance
        table.search(searchValue).draw();

    }
});

$(document).ready(function () {


    // Toggle all checkboxes when header checkbox is clicked
    $('#select-all-roles').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('.row-checkbox').prop('checked', isChecked);
        toggleActionButtons();
    });

    // When any row checkbox is changed
    // Use `on` with delegation to handle dynamically rendered checkboxes
    $(document).on('change', '.row-checkbox', function () {
        const total = $('.row-checkbox').length;
        const checked = $('.row-checkbox:checked').length;

        // Check/uncheck header checkbox
        $('#select-all-roles').prop('checked', total === checked);

        toggleActionButtons();
    });


    // Toggle visibility of action buttons
    function toggleActionButtons() {
        
        const selectedCount = $('.row-checkbox:checked').length;

        const shouldShow = selectedCount > 0;

        $('#delete-selected').prop('hidden', !shouldShow);
        $('#disable-selected').prop('hidden', !shouldShow);
        $('#enable-selected').prop('hidden', !shouldShow);
    }

    $('#delete-selected').on('click', function () {
        const selectedIds = $('.row-checkbox:checked').map(function () {
            return $(this).data('id');
        }).get();

        if (selectedIds.length === 0) {
            alert('No rows selected!');
            return;
        }

        if (!confirm('Are you sure you want to delete the selected items?')) {
            return;
        }

        $.ajax({
            url: '/Roles/DeleteMultiple',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(selectedIds),
            success: function (response) {
                alert(response.message);
                // Optionally reload your DataTable
                $('#rolesTable').DataTable().ajax.reload();
            },
            error: function (xhr) {
                alert('Error occurred: ' + xhr.responseText);
            }
        });
    });

    $('#disable-selected').on('click', function () {
        
        const selectedIds = $('.row-checkbox:checked').map(function () {
            return $(this).data('id');
        }).get();

        if (selectedIds.length === 0) {
            alert('No rows selected!');
            return;
        }

        if (!confirm('Are you sure you want to disbale the selected items?')) {
            return;
        }

        $.ajax({
            url: '/Roles/DisableMultiple',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(selectedIds),
            success: function (response) {
                alert(response.message);
                // Optionally reload your DataTable
                $('#rolesTable').DataTable().ajax.reload();
            },
            error: function (xhr) {
                alert('Error occurred: ' + xhr.responseText);
            }
        });
    });

    $('#enable-selected').on('click', function () {
        const selectedIds = $('.row-checkbox:checked').map(function () {
            return $(this).data('id');
        }).get();

        if (selectedIds.length === 0) {
            alert('No rows selected!');
            return;
        }

        if (!confirm('Are you sure you want to enable the selected items?')) {
            return;
        }

        $.ajax({
            url: '/Roles/EnableMultiple',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(selectedIds),
            success: function (response) {
                alert(response.message);
                // Optionally reload your DataTable
                $('#rolesTable').DataTable().ajax.reload();
            },
            error: function (xhr) {
                alert('Error occurred: ' + xhr.responseText);
            }
        });
    });

    
    $('#rolesTable').DataTable({
        processing: true,
        serverSide: true,
        paging: true,
        scrollX: true,
        dom: 'lrtip', // Hides the default search input
        ajax: {
            url: '/Roles/GetPagginatedData',
            type: 'POST'
        },
        columns: [
            {
                data: 'id',
                orderable: false, searchable: false,
                render: function (id) {
                    return `<input class="form-check-input row-checkbox" data-id="${id}" type="checkbox" />`;
                }
            },
            { data: 'name' },
            {
                data: 'isActive',
                render: function (data) {
                    if (data) {
                        return `<span class="badge rounded-pill text-success bg-success bg-opacity-10 px-3 py-2">Active</span>`;
                    } else {
                        return `<span class="badge rounded-pill text-danger bg-danger bg-opacity-10 px-3 py-2">Inactive</span>`;
                    }
                },
                orderable: false, searchable: false
            },
            {
                data: null, // Use full row object
                render: function (data) {
                    const id = data.id;
                    const isActive = data.isActive;

                    return `
        <div class="d-flex gap-2">
            <button onclick="viewRole('${id}')" class="btn btn-outline-dark rounded-pill btn-sm"><i class="bi bi-eye"></i></button>
            <button onclick="loadPermissions('${id}')" class="btn btn-outline-dark rounded-pill btn-sm">Permissions</button>
            <button class="btn btn-outline-dark rounded-pill btn-sm toggle-status-btn" data-id="${id}" data-status="${isActive}">
                ${isActive ? 'Disable' : 'Enable'}
            </button>
            <button onclick="CreateNewRole('${id}')" class="btn btn-outline-dark rounded-pill btn-sm btn-icon edit-btn" data-id="${id}">
                <i class="bi bi-pencil"></i>
            </button>
            <button onclick="DeleteRole('${id}')" class="btn btn-outline-danger rounded-pill btn-sm btn-icon delete-btn" data-id="${id}">
                <i class="bi bi-trash"></i>
            </button>
        </div>`;
                },
                orderable: false, searchable: false
            }

        ],
        info: false,             // This shows the "Showing x to y of z entries"
        paging: true,           // Enables pagination
        lengthChange: false,     // Optional: dropdown to change page size
        pageLength: 10          // Optional: default page size
    });

    
});


