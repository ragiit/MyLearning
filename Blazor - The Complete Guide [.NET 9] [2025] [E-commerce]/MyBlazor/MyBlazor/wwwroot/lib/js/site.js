window.showToastr = (type, message) => {
    // Konfigurasi default Toastr (opsional, bisa disesuaikan)
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right", // atau "toast-top-center", "toast-bottom-left", dll.
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000", // Notifikasi akan hilang setelah 5 detik
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    switch (type) {
        case "success":
            toastr.success(message);
            break;
        case "error":
            toastr.error(message);
            break;
        case "warning":
            toastr.warning(message);
            break;
        case "info":
            toastr.info(message);
            break;
        default:
            toastr.info(message); // Default ke info jika tipe tidak dikenal
            break;
    }
}