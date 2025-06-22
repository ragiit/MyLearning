var dataTable;

$(document).ready(function () {
    // Mendapatkan parameter status dari URL
    var url = window.location.search;
    var status = "";
    if (url.includes("inprocess")) {
        status = "inprocess";
    } else if (url.includes("pending")) {
        status = "pending";
    } else if (url.includes("completed")) {
        status = "completed";
    } else if (url.includes("approved")) {
        status = "approved";
    }
    loadDataTable(status);
});

function loadDataTable(status) {
    dataTable = $('#orderTable').DataTable({
        "ajax": { "url": `/Admin/Order/GetAll?status=${status}` },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'orderStatus', "width": "10%" },
            {
                data: 'orderTotal',
                "render": function (data) {
                    return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR' }).format(data);
                },
                "width": "15%"
            },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="btn-group d-flex justify-content-center" role="group">
                            <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-1">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>`;
                },
                "className": "text-center align-middle",
                "width": "15%"
            }

        ]
    });
}