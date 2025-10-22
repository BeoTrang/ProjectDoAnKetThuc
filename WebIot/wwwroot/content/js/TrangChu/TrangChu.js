const sidebar = $("#sidebar");
const overlay = $("#overlay");
const toggleBtn = $("#toggleBtn");
const mobileMenu = $("#mobileMenu");

toggleBtn.on("click", function () {
    sidebar.toggleClass("collapsed");
    const isCollapsed = sidebar.hasClass("collapsed");
    localStorage.setItem("sidebarCollapsed", isCollapsed);
});
mobileMenu.on("click", function () {
    sidebar.addClass("active");
    overlay.addClass("active");
});

overlay.on("click", function () {
    sidebar.removeClass("active");
    overlay.removeClass("active");
    localStorage.setItem("sidebarActive", false);
});





$(document).ready(async function () {
    const isCollapsed = localStorage.getItem("sidebarCollapsed") === "true";

    if (isCollapsed) {
        sidebar.addClass("collapsed");
    }
    try {
        showSpinner();
        $('#main-content').load('/dashboard', function () {
            console.log("Đã load dashoard");
        });
        console.log("Đã load dashboard");
        await Init_Dashboard();
    } catch (err) {
        console.error("Lỗi khi load:", err);
    } finally {
        hideSpinner();
    }
    
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
    });
});

$('#HoSo').on('click', function () {
    showSpinner();
    $('#main-content').load('/ho-so-tai-khoan', async function () {
        console.log("Đã load hồ sơ");
        await Init_HoSoTaiKhoan(); 
        hideSpinner();
    });
});


$('#TrangChu').on('click', async function () {
    try {
        showSpinner();
        $('#main-content').load('/dashboard', function () {
            console.log("Đã load dashoard");
        });
        console.log("Đã load dashboard");
        await Init_Dashboard();
    } catch (err) {
        console.error("Lỗi khi load:", err);
    } finally {
        hideSpinner();
    }
});



