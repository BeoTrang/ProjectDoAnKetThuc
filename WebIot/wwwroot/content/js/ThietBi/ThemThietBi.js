async function TrangThemThietBi() {
    try {
        const res = await fetch(`/view-trang-them-thiet-bi`, {
            method: "GET"
        });

        const contentType = res.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            const data = await res.json();

            if (!data.success) {
                Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: data.message,
                    showConfirmButton: false,
                    timer: 1000
                });
                return;
            }
        } else {
            const html = await res.text();
            $('#main-content').empty().append(html);

        }
    } catch (err) {
        console.error(err);
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Lỗi hệ thống",
            showConfirmButton: false,
            timer: 1000
        });
    } finally {
        hideSpinner();
    }
};

async function ViewThemThietBiMoi() {
    try {
        const res = await fetch(`/view-them-thiet-bi-theo-kieu-thiet-bi-moi`, {
            method: "GET"
        });

        const contentType = res.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            const data = await res.json();

            if (!data.success) {
                Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: data.message,
                    showConfirmButton: false,
                    timer: 1000
                });
                return;
            }
        } else {
            const html = await res.text();
            $('#ViewThemThietBiMoi').empty().append(html);

        }
    } catch (err) {
        console.error(err);
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Lỗi hệ thống",
            showConfirmButton: false,
            timer: 1000
        });
    } finally {
        hideSpinner();
    }
};

async function ViewThemThietBiShare() {
    try {
        const res = await fetch(`/view-them-thiet-bi-theo-kieu-chia-se`, {
            method: "GET"
        });

        const contentType = res.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            const data = await res.json();

            if (!data.success) {
                Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: data.message,
                    showConfirmButton: false,
                    timer: 1000
                });
                return;
            }
        } else {
            const html = await res.text();
            $('#ViewShareThietBi').empty().append(html);

        }
    } catch (err) {
        console.error(err);
        Swal.fire({
            position: "top-end",
            icon: "error",
            title: "Lỗi hệ thống",
            showConfirmButton: false,
            timer: 1000
        });
    } finally {
        hideSpinner();
    }
};

async function Init_AddDevice() {
    showSpinner();
    await TrangThemThietBi();
    await ViewThemThietBiMoi();
    await ViewThemThietBiShare();

    $(document).off("click", "#TaoMaThemThietBi").on("click", "#TaoMaThemThietBi", async function () {
        const deviceType = $('#MaThietBi');
        console.log(deviceType.val());
        try {
            showSpinner();
            const res = await fetch('/api/tao-ma-them-thiet-bi', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    deviceType: deviceType.val()
                })
            });
            const request = await res.json();
            if (request.success) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: request.message,
                    showConfirmButton: false,
                    timer: 1000
                });
                await ViewThemThietBiMoi();
            }
            else {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: request.message
                });
            }
        }
        catch {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Lỗi hệ thống, thử lại sau!"
            });
        }
        finally {
            hideSpinner();
        }
    });

    $(document).off("click", "#HuyThemThietBi").on("click", "#HuyThemThietBi", async function () {
        try {
            showSpinner();
            const res = await fetch('/api/huy-ma-them-thiet-bi', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
            const request = await res.json();
            if (request.success) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: request.message,
                    showConfirmButton: false,
                    timer: 1000
                });
                await ViewThemThietBiMoi();
            }
            else {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: request.message
                });
            }
        }
        catch {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Lỗi hệ thống, thử lại sau!"
            });
        }
        finally {
            hideSpinner();
        }
    });


    $(document).off("click", "#ThemThietBiChiaSe").on("click", "#ThemThietBiChiaSe", async function () {
        try {
            showSpinner();
            const ma = $('#MaChiaSeThietBi');
            console.log(ma.val());
            if (!ma.val()) {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Bạn chưa điền mã chia sẻ thiết bị! :))"
                });
                return;
            }
            const res = await fetch('/api/them-thiet-bi-chia-se', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    maThietBi: ma.val()
                })
            });
            const request = await res.json();
            if (request.success) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: request.message,
                    showConfirmButton: false,
                    timer: 1000
                });
                await ViewThemThietBiMoi();
            }
            else {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: request.message
                });
            }
        }
        catch {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Lỗi hệ thống, thử lại sau!"
            });
        }
        finally {
            hideSpinner();
        }
    });
};
