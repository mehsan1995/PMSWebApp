loadDepartmentTree();

function loadDepartmentTree() {
    $.ajax({
        url: '/Departments/GetDepartmentTree', // update controller name
        type: 'GET',
        data: { Id: $("#departmentId").val() },
        success: function (response) {

            if (response.success) {
                $('#departmentTree').html("");
                let html = buildTree(response.data);
                $('#departmentTree').html(html);
            } else {
                alert(response.message)
            }

        },
        error: function () {
            $('#departmentTree').html('<p class="text-danger">Failed to load data.</p>');
        }
    });
}

function buildTree(node) {
    
    if (!node) return '';

    let hasChildren = node.childDeparts && node.childDeparts.length > 0;
    let toggleBtn = hasChildren
        ? `<span class="toggle-btn text-success" style="cursor:pointer;">[-]</span>`
        : '';
    let addBtn = `<button onclick="CreateUpdateDepartment(null, ${node.id})" class="btn btn-sm btn-outline-success ms-2">+</button>`;

    // Start the parent container
    let html = `
        <div class="tree-node">
            <div class="node-header">
                ${toggleBtn}
                <span class="node-name"><a href="javascript:void(0)" onclick="ShowDepartmentUsers(${node.id})">${node.name}</a></span>
                ${addBtn}
            </div>
    `;

    // If children exist, build them recursively inside this node
    if (hasChildren) {
        html += `<div class="children" style="margin-left: 20px;">`;
        node.childDeparts.forEach(child => {
            html += buildTree(child);
        });
        html += `</div>`;
    }

    html += `</div>`; // Close this tree-node

    return html;
}

function ShowDepartmentUsers(departmentId) {
    $.ajax({
        url: '/DepartmentUsers/GetUsersList',
        type: 'GET',
        data: { id: departmentId },
        success: function (html) {
            
            $("#departmentUsersContainer").html("");
            $("#departmentUsersContainer").html(html);
        },
        error: function () {
            alert("Failed to load users.");
        }
    });
}

function CreateUserDepartment() {
    let departId = $('#departmentUsersModal').data('department-id');

    const userId = $('#departmentUsers').val();
    const permissionId = $('#departmentPermission').val();

    if (!userId) {
        alert("Please select a user.");
        return;
    }
    if (!permissionId) {
        alert("Please select a permission.");
        return;
    }

    $.ajax({
        url: '/DepartmentUsers/Add',
        dataType: 'json',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            departmentId: departId,
            userId: userId,
            roleId: permissionId


        }),
        success: function (data) {
            if (data.success) {
                alert(`User added to department!`);
                ShowDepartmentUsers(departId)
            } else {

            }
            
        },
        error: function (xhr) {
            console.error(xhr.responseText);
            alert("Failed to add user.");
        }
    });
}
function deleteUser(id, departmentId) {
    
    if (confirm("Are you sure you want to delete this user?")) {
        $.ajax({
            url: '/DepartmentUsers/DeleteUser',
            type: 'POST',
            data: { id: id },
            success: function (response) {
                if (response.success) {
                    ShowDepartmentUsers(departmentId)
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("An error occurred while deleting the user.");
            }
        });
    }
}
function assignManager(userId, departmentId) {
    
    $.ajax({
        url: '/Departments/AssignManager',
        contentType: 'application/json', 
        type: 'POST',
        data: JSON.stringify({
            departmentId: departmentId,
            userId: userId
        }),
        success: function (response) {
            if (response.success) {
                alert("Manager assign successfully");
                ShowDepartmentUsers(departmentId)
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert("An error occurred while updating record.");
        }
    });
}
function AddUserDepartment(departmentId) {

    $("#departmentUsersModal").data("department-id", departmentId);

    // Show modal
    $("#departmentUsersModal").modal("show");
}




$(document).on('click', '.toggle-btn', function () {
    let $this = $(this);
    let $childrenContainer = $this.closest('.tree-node').children('.children');

    // Toggle visibility
    $childrenContainer.toggle();

    // Update button text
    if ($childrenContainer.is(':visible')) {
        $this.text('[-]');
    } else {
        $this.text('[+]');
    }
});