let currentPage = 1;
let searcFiels = "";
loadGridCards(currentPage);

function searchUser() {
    debugger
    var searchValue = document.getElementById('searchUser').value;
    // For DataTable
    var table = $('#usersListTable').DataTable(); // Use jQuery to access the DataTable instance
    table.search(searchValue).draw();

    loadGridCards(currentPage);
}

document.getElementById('searchUser').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
        searchUser();

    }
});

$(document).on('click', '.toggle-status-btn', function () {
    const $button = $(this);
    const roleId = $button.data('id');
    const currentStatus = $button.data('status'); // true or false

    const action = currentStatus ? 'disable' : 'enable';
    if (!confirm(`Are you sure you want to ${action} this role?`)) return;

    $.ajax({
        url: `/Users/ToggleStatus`,
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
                $('#usersListTable').DataTable().ajax.reload();
            } else {
                alert(`Error: ${response.message}`);
            }
        },
        error: function () {
            alert('An unexpected error occurred.');
        }
    });
});


$(document).on('change', '#select-all-users', function () {
    
    const isChecked = $(this).prop('checked');
    $('.row-checkbox').prop('checked', isChecked);
    toggleActionButtons();
});

$(document).on('change', '.row-checkbox', function () {
    const total = $('.row-checkbox').length;
    const checked = $('.row-checkbox:checked').length;

    // Check/uncheck header checkbox
    $('#select-all-users').prop('checked', total === checked);

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
        url: '/Users/DeleteMultiple',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(selectedIds),
        success: function (response) {
            alert(response.message);
            // Optionally reload your DataTable
            $('#usersListTable').DataTable().ajax.reload();
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
        url: '/Users/DisableMultiple',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(selectedIds),
        success: function (response) {
            alert(response.message);
            // Optionally reload your DataTable
            $('#usersListTable').DataTable().ajax.reload();
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
        url: '/Users/EnableMultiple',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(selectedIds),
        success: function (response) {
            alert(response.message);
            // Optionally reload your DataTable
            $('#usersListTable').DataTable().ajax.reload();
        },
        error: function (xhr) {
            alert('Error occurred: ' + xhr.responseText);
        }
    });
});


$(document).ready(function () {

    

    $('#usersListTable').DataTable({
        
       processing: true,
    serverSide: true,
    paging: true,
    info: false,
    lengthChange: false,
    destroy: true,
    pageLength: 10,
    dom: 'lrtip',
    autoWidth: false,
        ajax: {
            url: '/Users/GetPagginatedData',
            type: 'POST'
        },
        columns: [
            {
                
                data: 'id',
                orderable: false,
                searchable: false,
                render: function (id) {
                    return `<input class="form-check-input row-checkbox" data-id="${id}" type="checkbox" />`;
                }
            },
            { data: 'name' },
            { data: 'email' },
            { data: 'departmentName' },
            { data: 'jobTitle' },
            {
                data: 'isActive',
                render: function (data) {
                    if (data) {
                        return `<span class="badge rounded-pill text-success bg-success bg-opacity-10 px-3 py-2">Active</span>`;
                    } else {
                        return `<span class="badge rounded-pill text-danger bg-danger bg-opacity-10 px-3 py-2">Inactive</span>`;
                    }
                },
                orderable: false,
                searchable: false
            },
            {
                data: null, // Use full row object
                render: function (data) {
                    const id = data.id;
                    const isActive = data.isActive;

                    return `
        <div class="d-flex gap-2">
             <a href="/Users/Details/${id}" class="btn btn-outline-dark rounded-pill btn-sm " data-id="${id}" data-status="${isActive}">
               <i class="bi bi-eye"></i>
            </a>
            <button class="btn btn-outline-dark rounded-pill btn-sm toggle-status-btn" data-id="${id}" data-status="${isActive}">
                ${isActive ? 'Disable' : 'Enable'}
            </button>
            <a href="/Users/AddEdit/${id}" class="btn btn-outline-dark rounded-pill btn-sm btn-icon edit-btn" data-id="${id}">
                <i class="bi bi-pencil"></i>
            </a>
            <button onclick="DeleteUser('${id}')" class="btn btn-outline-danger rounded-pill btn-sm btn-icon delete-btn" data-id="${id}">
                <i class="bi bi-trash"></i>
            </button>
        </div>`;
                },
                orderable: false,
                searchable: false
            }

        ],
               
    });
});


function loadGridCards(page = 1) {
    
    var requestData = {
        draw: 1,
        start: (page - 1) * 10,
        length: 10,
        sortColumn: 'EmployeeName',
        sortDirection: 'asc',
        search: {
            value: document.getElementById('searchUser').value

        }
    };

    $.ajax({
        url: '/Users/GetPagginatedData', // Update to your actual API endpoint
        type: 'POST',
        data: requestData,
        success: function (response) {
            const users = response.data;
            const recordsTotal = response.recordsTotal;
            $('#totalUserCount').html("");
            $('#totalUserCount').html(recordsTotal);
            const $grid = $('#gridContainer');
            $grid.empty();

            users.forEach(user => {
                const fullName = `${user.firstName ?? ''} ${user.lastName ?? ''}`;
                const email = user.email ?? '';
                const phone = user.phoneNumber ?? '';
                const role = user.roleName ?? 'Project Manager';
                const id = user.id;

                const cardHtml = `
                <div class="col-sm-6 col-md-4 col-lg-3 mb-3">
                    <div class="card h-100">
                        <div class="card-body d-flex flex-column">
                            <div class="d-flex justify-content-between">
                                <div class="d-flex align-items-center">
                                    <div class="me-2">
                                        <img src="" class="rounded-circle d-none" alt="User" width="40" height="40">
                                        <i class="bi bi-person-circle user-icon-xxl user-icon"></i>
                                    </div>
                                    <div>
                                        <h6 class="mb-0">${fullName}</h6>
                                        <small class="text-muted">${role}</small>
                                    </div>
                                </div>
                                <div class="dropdown">
                                    <button class="btn btn-sm" data-bs-toggle="dropdown">
                                        <i class="bi bi-three-dots-vertical"></i>
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <li><a class="dropdown-item" href="/Users/Details/${id}">View</a></li>
                                        <li><a class="dropdown-item" href="/Users/AddEdit/${id}">Edit</a></li>
                                        <li><hr class="dropdown-divider"></li>
                                        <li><a class="dropdown-item text-danger" href="/Users/Delete/${id}">Delete</a></li>
                                    </ul>
                                </div>
                            </div>
                            <hr />
                            <p class="mb-1"><i class="bi bi-person-circle"></i> Role: ${role}</p>
                            <p class="mb-1"><i class="bi bi-envelope me-1"></i> ${email}</p>
                            <p class="mb-0"><i class="bi bi-telephone me-1"></i> ${phone}</p>
                        </div>
                    </div>
                </div>`;

                $grid.append(cardHtml);
            });

            renderGridPagination(response.recordsTotal, page);
        }
    });
}

function renderGridPagination(totalRecords, currentPage) {
    const totalPages = Math.ceil(totalRecords / 10);
    let paginationHtml = `<nav><ul class="pagination">`;

    for (let i = 1; i <= totalPages; i++) {
        paginationHtml += `
        <li class="page-item ${i === currentPage ? 'active' : ''}">
  <a class="page-link" href="#" data-page="${i}" 
     style="background-color: ${i === currentPage ? '#198754' : 'white'}; color: ${i === currentPage ? 'white' : '#198754'}; border-color: #198754;">
    ${i}
  </a>
</li>
`;
    }

    paginationHtml += `</ul></nav>`;
    $('#gridPagination').html(paginationHtml);

    $('#gridPagination .page-link').on('click', function (e) {
        e.preventDefault();
        const page = parseInt($(this).data('page'));
        loadGridCards(page);
    });
}

// Trigger on tab switch (optional)
$('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
    const target = $(e.target).attr("href");
    if (target === "#grid-tab") {
        loadGridCards();
    }
});

