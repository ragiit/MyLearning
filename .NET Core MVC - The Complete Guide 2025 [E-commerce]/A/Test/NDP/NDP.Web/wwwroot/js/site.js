// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// wwwroot/js/site.js

$(document).ready(function () {
    // 1. Inisialisasi AOS (Animate on Scroll)
    AOS.init({
        duration: 800,
        once: true,
        offset: 100,
        easing: 'ease-in-out-quad',
    });

    // --- LOGIKA HEADER ---

    // 2. Toggle Menu Mobile
    $('#mobile-menu-toggle, #mobile-menu-overlay').on('click', function () {
        $('#mobile-menu').toggleClass('translate-x-full');
        $('#mobile-menu-overlay').toggleClass('hidden');
    });

    // 3. Toggle Search Overlay
    $('#search-toggle').on('click', function () {
        $('#search-overlay').removeClass('hidden');
    });
    // Menutup search overlay saat mengklik area gelap atau tombol close
    $('#search-overlay, #search-overlay .fa-times').on('click', function (e) {
        // Mencegah penutupan saat mengklik konten di dalamnya
        if (e.target !== this && $(e.target).closest('.bg-white').length > 0) {
            return;
        }
        $('#search-overlay').addClass('hidden');
    });

    // 4. Toggle Mini Cart
    $('#mini-cart-toggle').on('click', function () {
        $('#mini-cart-dropdown').toggleClass('hidden');
        // TODO: Panggil fungsi untuk memuat konten keranjang via AJAX jika belum dimuat
        // loadMiniCart();
    });

    // --- LOGIKA HALAMAN ---

    // 5. Tombol Scroll to Top
    const $scrollTopBtn = $('#scroll-to-top-button');

    $(window).on('scroll', function () {
        if ($(window).scrollTop() > 300) {
            $scrollTopBtn.removeClass('hidden');
        } else {
            $scrollTopBtn.addClass('hidden');
        }
    });

    $scrollTopBtn.on('click', function () {
        $('html, body').animate({ scrollTop: 0 }, 'smooth');
    });
});

// --- FUNGSI AJAX (Contoh) ---

// Fungsi untuk memuat konten mini cart secara dinamis
function loadMiniCart() {
    // Cek jika konten sudah ada untuk mencegah load berulang
    if ($('#mini-cart-dropdown').data('loaded')) return;

    $.ajax({
        url: '/Cart/MiniCart', // Buat Controller dan Action ini
        type: 'GET',
        success: function (response) {
            $('#mini-cart-dropdown').html(response);
            $('#mini-cart-dropdown').data('loaded', true);
        },
        error: function () {
            $('#mini-cart-dropdown').html('<div class="p-4 text-center text-red-500">Gagal memuat keranjang.</div>');
        }
    });
}