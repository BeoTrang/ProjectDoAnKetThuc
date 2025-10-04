async function Init_HoSoTaiKhoan() {
    try {
        const res = await fetch("/lay-ho-so-tai-khoan", { method: "GET" });

        if (!res.ok) {
            throw new Error(`Lỗi HTTP: ${res.status}`);
        }

        const json = await res.json();
        const info = await json.data;

        $('#name').val(info.name || "");
        $('#email').val(info.email || "");
        $('#phone_number').val(info.phone_number || "");
        $('#account_login').val(info.account_login || "");
        $('#tele_chat_id').val(info.tele_chat_id || "");
        $('#tele_bot_id').val(info.tele_bot_id || "");

        $('#DoiMatKhauForm').submit(async function (e) {
            e.preventDefault();
            e.stopPropagation();

            const matKhauCu = $('#password_old');
            const matKhauMoi1 = $('#password_new1');
            const matKhauMoi2 = $('#password_new2');

            const isMatKhauCu = matKhauCu.val().trim().length > 0;
            const isMatKhauMoi1 = matKhauMoi1.val().trim().length > 0;
            const isMatKhauMoi2 = matKhauMoi2.val().trim().length > 0;

            matKhauCu.toggleClass('is-invalid', !isMatKhauCu);
            matKhauMoi1.toggleClass('is-invalid', !isMatKhauMoi1);
            matKhauMoi2.toggleClass('is-invalid', !isMatKhauMoi2);

            if (!isMatKhauCu || !isMatKhauMoi1 || !isMatKhauMoi2) return;

            if (matKhauMoi1.val().trim() !== matKhauMoi2.val().trim()) {
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Mật khẩu mới không trùng khớp"
                });
                return;
            }


            Swal.fire({
                title: "Bạn có chắc chắn muốn đổi mật khẩu không?",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Có, tôi muốn đổi!",
                cancelButtonText: "Hủy bỏ"
            }).then(async (result) => {
                if (result.isConfirmed) {
                    showSpinner();
                    try {
                        const res = await fetch('/kiem-tra-va-doi-mat-khau', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({
                                matKhauCu: matKhauCu.val().trim(),
                                matKhauMoi: matKhauMoi2.val().trim()
                            })
                        });

                        const data = await res.json();

                        if (!data.success) {
                            Swal.fire({
                                icon: "error",
                                title: "Oops...",
                                text: data.message
                            });
                        } else {
                            Swal.fire({
                                title: "Hoàn thành!",
                                text: "Đổi mật khẩu thành công!",
                                icon: "success"
                            });
                            matKhauCu.val('');
                            matKhauMoi1.val('');
                            matKhauMoi2.val('');
                        }
                    } catch (error) {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "Lỗi hệ thống, thử lại sau!"
                        });
                    } finally {
                        hideSpinner();
                    }
                }
            });
        });

        $('#ThongTinForm').submit(async function (e) {
            e.preventDefault();
            e.stopPropagation();

            const HoVaTen = $('#name');
            const Email = $('#email');
            const SoDienThoai = $('#phone_number');
            const TaiKhoanDangNhap = $('#account_login');

            const isHoVaTen = HoVaTen.val().trim().length > 0;
            const isEmail = Email.val().trim().length > 0;
            const isSoDienThoai = SoDienThoai.val().trim().length > 0;
            const isTaiKhoanDangNhap = TaiKhoanDangNhap.val().trim().length > 0;

            HoVaTen.toggleClass('is-invalid', !isHoVaTen);
            Email.toggleClass('is-invalid', !isEmail);
            SoDienThoai.toggleClass('is-invalid', !isSoDienThoai);
            TaiKhoanDangNhap.toggleClass('is-invalid', !isTaiKhoanDangNhap);

            if (!isHoVaTen || !isEmail || !isSoDienThoai || !isTaiKhoanDangNhap) return;

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
                        const res = await fetch('/doi-thong-tin-nguoi-dung', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({
                                hoVaTen: HoVaTen.val().trim(),
                                email: Email.val().trim(),
                                soDienThoai: SoDienThoai.val().trim(),
                                taiKhoanDangNhap: TaiKhoanDangNhap.val().trim()
                            })
                        });

                        const json = await res.json();

                        if (!json.success) {
                            Swal.fire({
                                icon: "error",
                                title: "Oops...",
                                text: json.message
                            });
                        }
                        else {
                            Swal.fire({
                                title: "Thay đổi thông tin thành công!",
                                //text: "You clicked the button!",
                                icon: "success"
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

        $('#TelegramForm').submit(async function (e) {
            e.preventDefault();
            e.stopPropagation();

            const ChatId = $('#tele_chat_id');
            const BotId = $('#tele_bot_id');

            const isChatId = ChatId.val().trim().length > 0;
            const isBotId = BotId.val().trim().length > 0;

            ChatId.toggleClass('is-invalid', !isChatId);
            BotId.toggleClass('is-invalid', !isBotId);

            if (!isChatId || !isBotId) return;

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
                        const res = await fetch('/doi-thong-tin-telegram', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({
                                chatId: ChatId.val().trim(),
                                botId: BotId.val().trim(),
                            })
                        });

                        const json = await res.json();

                        if (!json.success) {
                            Swal.fire({
                                icon: "error",
                                title: "Oops...",
                                text: json.message
                            });
                        }
                        else {
                            Swal.fire({
                                title: "Thay đổi thông tin thành công!",
                                //text: "You clicked the button!",
                                icon: "success"
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

    } catch (err) {
        console.error("Không thể lấy hồ sơ:", err);
        alert("Có lỗi xảy ra khi tải hồ sơ. Vui lòng thử lại.");
    }
};