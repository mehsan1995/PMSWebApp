$(document).ready(function () {
    $('#tenantTable').DataTable({

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
            url: '/Tenants/GetPagginatedData',
            type: 'POST'
        },
        columns: [
            { data: 'tenantName' },
            { data: 'tenantCode' },
            { data: 'description' },
            {
                data: null, // Use full row object
                render: function (data) {
                    const id = data.id;
                    const isActive = data.isActive;

                    return `
        <div class="d-flex gap-2">
             <a href="#" onclick="ViewTenant('${id}')" class="btn btn-outline-dark rounded-pill btn-sm " data-id="${id}">
               <i class="bi bi-eye"></i>
            </a>
            <a href="/Tenants/AddEdit/${id}" class="btn btn-outline-dark rounded-pill btn-sm btn-icon edit-btn" data-id="${id}">
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

function Delete(tenantId) {
    var isConfirmed = confirm('Are you sure you want to delete this tenant? This action cannot be undone.');
    if (isConfirmed) {
        $.ajax({
            url: '/Tenants/Delete',
            type: 'POST',
            data: { id: tenantId },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#tenantTable').DataTable().ajax.reload(); 
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
