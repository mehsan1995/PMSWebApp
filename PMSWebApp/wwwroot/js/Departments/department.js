function CreateUpdateDepartment(id = null, parentId = null) {
  

    let queryParams = [];
    if (id !== null) queryParams.push("id=" + id);
    if (parentId !== null) queryParams.push("parentId=" + parentId);

    let url = '/Departments/AddEdit' + (queryParams.length > 0 ? '?' + queryParams.join('&') : '');
    
    $.ajax({
        url: url,
        type: 'GET',
        success: function (html) {
           
            $('#departmentModalContent').html(html);
            $('#departmentModal').modal('show');

            // Reset and re-parse validation
            var form = $('#createDepartmentForm');
            form.removeData('validator').removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);
        },
        error: function (error) {
            alert('Failed to load form.');
        }
    });
}

function formToNestedJson(form) {
    var data = { Department: {} };

    form.serializeArray().forEach(function (item) {
        if (item.name.startsWith("Department.")) {
            var propName = item.name.split('.')[1]; // e.g. "Name"
            data.Department[propName] = item.value;
        } else {
            data[item.name] = item.value;
        }
    });

    return data;
}

$(document).on('submit', '#createDepartmentForm', function (e) {
    e.preventDefault();
    
    var form = $(this);

    // Clear old errors
    $('#createDepartmentForm .is-invalid').removeClass('is-invalid');
    $('#createDepartmentForm span[data-valmsg-for]').text('');

    // Serialize to object
    var formData = { Department: {} };
    $.each(form.serializeArray(), function (_, field) {
        
        formData.Department[field.name.replace('Department.', '')] = field.value;
    });


    var token = form.find('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: form.attr('action'),
        type: 'POST',
        data: form.serialize(), // not JSON
        headers: {
            'RequestVerificationToken': token
        },
        success: function (response) {
            if (response.success) {
                $('#departmentModal').modal('hide');
                alert(response.message);
                $('#departmentTable').DataTable().ajax.reload(); 
                loadDepartmentTree();
            } else {
                $.each(response.errors, function (index, error) {
                    $('[name="' + error.field + '"]').addClass('is-invalid');
                    $('span[data-valmsg-for="' + error.field + '"]').text(error.errors[0]);
                });
            }
        },
        error: function () {
            alert('Failed to save department.');
        }
    });

});




function searchDepartment() {
    debugger
    var searchValue = document.getElementById('searchDepartment').value;
    // For DataTable
    var table = $('#departmentTable').DataTable(); // Use jQuery to access the DataTable instance
    table.search(searchValue).draw();

}

document.getElementById('searchDepartment').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') {
        searchDepartment();

    }
});
$(document).ready(function () {
    $('#departmentTable').DataTable({

        processing: true,
        serverSide: true,
        paging: true,
        info: false,
        lengthChange: false,
        destroy: true,
        pageLength: 10,
        scrollX: true,
        dom: 'lrtip',
        autoWidth: false,
        ajax: {
            url: '/Departments/GetPagginatedData',
            type: 'POST'
        },
        columns: [
            { data: 'name' },
            { data: 'departmentCode' },
            {
                data: null, // Use full row object
                render: function (data) {
                    const id = data.id;

                    return `
        <div class="d-flex gap-2">
             <a href="/Departments/Details?Id=${id}" class="btn btn-outline-dark rounded-pill btn-sm " data-id="${id}">
               <i class="bi bi-eye"></i>
            </a>
            <a href="#" onclick="CreateUpdateDepartment('${id}')" class="btn btn-outline-dark rounded-pill btn-sm btn-icon edit-btn" data-id="${id}">
                <i class="bi bi-pencil"></i>
            </a>
            <button onclick="Delete('${id}')" class="btn btn-outline-danger rounded-pill btn-sm btn-icon delete-btn" data-id="${id}">
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

function Delete(Id) {
    var isConfirmed = confirm('Are you sure you want to delete this department? ');
    if (isConfirmed) {
        $.ajax({
            url: '/Departments/Delete',
            type: 'POST',
            data: { Id: Id },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#departmentTable').DataTable().ajax.reload();
                } else {
                    alert(`Error: ${response.message}\nDetails: ${response.detail}`);
                }

            },
            error: function (error) {

                alert('Failed to load tenant.');
            }
        });
    }

}
function ViewTenant(tenantId) {

    $.ajax({
        url: '/Tenants/Details', //
        type: 'GET',
        data: { id: tenantId },
        success: function (html) {
            $('#tenantModalContent').html("");
            $('#tenantModalContent').html(html);
            $('#tenantModal').modal('show');
        },
        error: function (error) {

            alert('Failed to load tenant.');
        }
    });
}
