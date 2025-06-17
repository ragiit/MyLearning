var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#productTable').DataTable({
        "ajax": {
            // Rute ke API endpoint yang kita buat di controller
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            // 'data' harus cocok dengan nama properti di JSON (case-sensitive, biasanya camelCase)
            {
                // 'data' adalah properti 'imageUrl' dari JSON
                data: 'imageUrl',
                "render": function (data) {
                    // Jika ada URL gambar, tampilkan gambarnya.
                    // Jika tidak, tampilkan gambar placeholder.
                    if (data) {
                        return `<div class="text-center"><img src="${data}" style="width:70px; height:auto; border-radius:5px; border:1px solid #bbb" /></div>`;
                    }
                    else {
                        return `<div class="text-center"><img src="https://placehold.co/70x70?text=NotFound" style="width:70px; height:auto; border-radius:5px; border:1px solid #bbb" /></div>`;
                    }
                },
                "width": "10%"
            },
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "15%" }, // Mengakses properti nested
            {
                // Kolom untuk tombol Aksi (Edit dan Delete)
                data: 'id',
                "render": function (data) {
                    // 'data' di sini adalah ID produk
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                                <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                            </div>`
                },
                "width": "20%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Apakah Anda yakin?',
        text: "Data yang dihapus tidak dapat dikembalikan!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Ya, hapus!',
        cancelButtonText: 'Batal'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    // Muat ulang tabel dan tampilkan notifikasi
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}