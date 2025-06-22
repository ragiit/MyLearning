var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#userTable').DataTable({
        "ajax": { "url": "/Admin/User/GetAll" },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'company.name', "width": "15%" },
            { data: 'role', "width": "10%" },
            {
                data: { id: 'id', lockoutEnd: 'lockoutEnd' },
                "render": function (data) {
                    if (data.id === currentUserId) {
                        return `<div class="text-center text-muted">Tidak tersedia</div>`;
                    }
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    var lockUnlockButton;
                    if (lockout > today) {
                        // Pengguna sedang terkunci
                        lockUnlockButton = `
                            <div class="text-center">
                                <a onClick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:120px;">
                                    <i class="bi bi-unlock-fill"></i> Unlock
                                </a>
                            </div>`
                    }
                    else {
                        // Pengguna aktif
                        lockUnlockButton = `
                            <div class="text-center">
                                <a onClick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:120px;">
                                    <i class="bi bi-lock-fill"></i> Lock
                                </a>
                            </div>`
                    }
                    return `<div class="text-center btn-group">
                            <a href="/admin/user/rolemanagement?userId=${data.id}" class="btn btn-primary text-white mx-1">
                                <i class="bi bi-pencil-square"></i> Role
                            </a>
                            ${lockUnlockButton}
                        </div>`
                },
                "width": "20%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}