async function LoadDeviceSetting(id) {
    $('#main-content').empty();

    try {
        const res = await fetch(`/thong-tin-thiet-bi/${id}`, {
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
            $('#main-content').append(html);

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

    try {
        const res = await fetch(`/chia-se-thiet-bi/${id}`, {
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
            $('#main-content').append(html);
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
}
async function Init_SettingThietBi(id) {
    showSpinner();
    await LoadDeviceSetting(id);
    

    $('#AX01SettingForm').submit(async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const deviceid = $('#deviceid');
        const master = $('#master');
        const relay1 = $('#relay1');
        const relay2 = $('#relay2');
        const relay3 = $('#relay3');
        const relay4 = $('#relay4');

        const is_master = master.val().trim().length > 0;
        const is_relay1 = relay1.val().trim().length > 0;
        const is_relay2 = relay2.val().trim().length > 0;
        const is_relay3 = relay3.val().trim().length > 0;
        const is_relay4 = relay4.val().trim().length > 0;

        master.toggleClass('is-invalid', !is_master);
        relay1.toggleClass('is-invalid', !is_relay1);
        relay2.toggleClass('is-invalid', !is_relay2);
        relay3.toggleClass('is-invalid', !is_relay3);
        relay4.toggleClass('is-invalid', !is_relay4);

        if (!is_master || !is_relay1 || !is_relay2 || !is_relay3 || !is_relay4) {
            Swal.fire({
                icon: "error",
                title: "Oops...",
                text: "Nhập đầy đủ thông tin!"
            });
            return;
        }
        Swal.fire({
            title: "Bạn có chắc muốn thay đổi thông tin không?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Có",
            cancelButtonText: "Không"
        }).then(async (result) => {
            if (result.isConfirmed) {
                showSpinner();
                try {
                    const json = {
                        relay1: relay1.val().trim(),
                        relay2: relay2.val().trim(),
                        relay3: relay3.val().trim(),
                        relay4: relay4.val().trim()
                    };
                    const res = await fetch('/api/luu-ten-thiet-bi', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            deviceid: deviceid.val(),
                            master: master.val().trim(),
                            nameConfig: JSON.stringify(json)
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
                        return;
                    }
                    else {
                        Swal.fire({
                            position: "top-end",
                            icon: "error",
                            title: request.message,
                            showConfirmButton: false,
                            timer: 1000
                        });
                        return;
                    }

                    console.log(json);
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
            }
        });
    });

    $(document).on("click", "#submitShareDevice", async function () {
        const deviceid = $('#ShareDeviceId');
        const quyen = $('#ShareQuyen');
        console.log(quyen.val());
        console.log(deviceid.val());
        try {
            showSpinner();
            const res = await fetch('/api/tao-ma-chia-se-thiet-bi', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    deviceid: deviceid.val(),
                    quyen: quyen.val()
                })
            });
            const request = await res.json();
            if (request.success) {
                Swal.fire({
                    title: request.message,
                    text: "Mã này sẽ có hiệu lực trong vòng 7 ngày",
                    icon: "success"
                });
                await LoadDeviceSetting(id);
                return;
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

    $(document).on("click", "#submitCancelShareDevice", async function () {
        const deviceid = $('#ShareDeviceId');
        try {
            showSpinner();
            const res = await fetch('/api/xoa-ma-chia-se-thiet-bi', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    deviceid: deviceid.val()
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
                await LoadDeviceSetting(id);
                return;
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