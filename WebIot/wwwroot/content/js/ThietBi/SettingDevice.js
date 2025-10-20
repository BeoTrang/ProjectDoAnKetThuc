async function Init_SettingThietBi() {
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
            //text: "You won't be able to revert this!",
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

                    const res = await fetch('/luu-ten-thiet-bi', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                            deviceid: deviceid.val(),
                            master: master.val().trim(),
                            nameConfig: json
                        })
                    });


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
};