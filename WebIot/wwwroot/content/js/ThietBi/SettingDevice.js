async function LoadTrangSetting() {
    try {
        const res = await fetch(`/trang-setting`, {
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

async function LoadDeviceSetting(id) {
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
            $('#DeviceSetting').empty().append(html);

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

async function LoadShareDevice(id) {
    try {
        const res = await fetch(`/chia-se-thiet-bi/${id}`, {
            method: "GET"
        });

        const contentType = res.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            const data = await res.json();

            if (!data.success) {
                //Swal.fire({
                //    position: "top-end",
                //    icon: "error",
                //    title: data.message,
                //    showConfirmButton: false,
                //    timer: 1000
                //});
                $('#ShareDevice').remove();
                return;
            }
        } else {
            const html = await res.text();
            $('#ShareDevice').empty().append(html);
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

async function LoadDeviceInfo(id) {
    try {
        const resView = await fetch(`/view-device-info`, {
            method: "GET"
        });

        const contentType = resView.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            const data = await resView.json();

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
            const html = await resView.text();
            $('#DeviceInfo').empty().append(html);

            try {
                const respone = await fetch(`/api/device-info/${id}`, {
                    method: "GET"
                });
                const dataRespone = await respone.json();
                if (dataRespone) {
                    const deviceTypeEl = $('#deviceType');
                    const deviceIdEl = $('#deviceIdInfo');
                    const deviceTimeEl = $('#deviceTime');

                    const times = new Date(dataRespone.data.deviceTime).toLocaleString('vi-VN', {
                        year: 'numeric',
                        month: '2-digit',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit',
                        second: '2-digit'
                    });

                    deviceIdEl.val(dataRespone.data.deviceId);
                    deviceTypeEl.val(dataRespone.data.deviceType);
                    deviceTimeEl.val(times);

                }
                else {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: data.message,
                        showConfirmButton: false,
                        timer: 1000
                    });
                }
            }
            catch {
                Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: "Lỗi hệ thống",
                    showConfirmButton: false,
                    timer: 1000
                });
            }
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

async function LoadNguongThietBi(id) {
    try {
        const res = await fetch(`/api/lay-nguong-cho-hien-thi/${id}`, {
            method: "GET"
        });

        const contentType = res.headers.get("content-type");

        if (contentType && contentType.includes("application/json")) {
            const data = await res.json();

            if (!data.success) {
                $('#NguongSetting').remove();
                return;
            }
        } else {
            const html = await res.text();
            $('#NguongSetting').empty().append(html);
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
    await LoadTrangSetting();
    await LoadDeviceSetting(id);
    await LoadNguongThietBi(id);
    await LoadShareDevice(id);
    await LoadDeviceInfo(id);
    

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

    $('#AX02SettingForm').submit(async function (e) {
        e.preventDefault();
        e.stopPropagation();

        const deviceid = $('#deviceid');
        const master = $('#master');

        const is_master = master.val().trim().length > 0;

        master.toggleClass('is-invalid', !is_master);

        if (!is_master) {
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
                    const res = await fetch('/api/luu-ten-thiet-bi', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            deviceid: deviceid.val(),
                            master: master.val().trim()
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

    $(document).off("click", "#submitShareDevice").on("click", "#submitShareDevice", async function () {
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
                await LoadShareDevice(id);
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

    $(document).off("click", "#submitCancelShareDevice").on("click", "#submitCancelShareDevice", async function () {
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
                await LoadShareDevice(id);
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


    $(document).off("click", "#HuyTheoDoiThietBi").on("click", "#HuyTheoDoiThietBi", async function () {
        const deviceid = $('#deviceIdInfo');
        Swal.fire({
            title: "Bạn có chắc chắn muốn xóa thiết bị?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Có, tôi muốn xóa!",
            cancelButtonText: "Hủy bỏ"
        }).then(async (result) => {
            if (result.isConfirmed) {
                showSpinner();
                try {
                    showSpinner();
                    const res = await fetch('/api/xoa-thiet-bi-boi-nguoi-dung', {
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
                        setTimeout(() => {
                            window.location.href = '/trang-chu';
                        }, 1500);
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
            }
        });
        
        
    });

    $("#AX01ThresholdForm").on("submit", function (e) {
        e.preventDefault();

        const data = {
            deviceId: parseInt($("#deviceid").val()),

            temNguongDuoi: parseFloat($("#temNguongDuoi").val()),
            temNguongTren: parseFloat($("#temNguongTren").val()),
            humNguongDuoi: parseFloat($("#humNguongDuoi").val()),
            humNguongTren: parseFloat($("#humNguongTren").val()),

            temIsAlert: $("#temEnable").is(":checked"),
            humIsAlert: $("#humEnable").is(":checked")
        };

        console.log(data);

        $.ajax({
            url: "/api/luu-nguong-cho-thiet-bi",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (request) {
                if (request.success) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: request.message,
                        showConfirmButton: false,
                        timer: 1000
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: request.message
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Lỗi hệ thống!"
                });
            }
        });
    });

};