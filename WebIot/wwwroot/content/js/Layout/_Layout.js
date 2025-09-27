$(document).ready(function () {
    const sidebar = $("#sidebar");
    const overlay = $("#overlay");
    const toggleBtn = $("#toggleBtn");
    const mobileMenu = $("#mobileMenu");

    // Desktop: thu gọn sidebar
    toggleBtn.on("click", function () {
        sidebar.toggleClass("collapsed");
    });

    // Mobile: mở sidebar overlay
    mobileMenu.on("click", function () {
        sidebar.addClass("active");
        overlay.addClass("active");
    });

    overlay.on("click", function () {
        sidebar.removeClass("active");
        overlay.removeClass("active");
    });
});


$('#DangXuat').on("click", async function () {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: "btn btn-success",
            cancelButton: "btn btn-danger"
        },
        buttonsStyling: false
    });
    swalWithBootstrapButtons.fire({
        title: "Bạn có chắc chắn đăng xuất không?",
        //text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Có, đăng xuất!",
        cancelButtonText: "Không.",
        reverseButtons: true
    }).then(async (result) => {
        if (result.isConfirmed) {
            const res = await fetch("/dang-xuat", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            let data;
            try {
                data = await res.json();
            }
            catch (error) {
                data = {
                    success: false
                }
            }
            if (!data.success) {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Đã xảy ra lỗi!"
                });
            }
            else {
                let timerInterval;
                Swal.fire({
                    title: "Đăng xuất thành công!",
                    timer: 2000,
                    timerProgressBar: true,
                    didOpen: () => {
                        Swal.showLoading();
                        timerInterval = setInterval(() => {

                        }, 100);
                    },
                    willClose: () => {
                        clearInterval(timerInterval);
                    }
                }).then((result) => {
                    if (result.dismiss === Swal.DismissReason.timer) {
                        window.location.href = "/";
                    }
                });

            }
        }
        //else if (
        //    result.dismiss === Swal.DismissReason.cancel) {
        //    swalWithBootstrapButtons.fire({
        //        title: "Cancelled",
        //        text: "Your imaginary file is safe :)",
        //        icon: "error"
        //    });
        //}
    });
});