const DangKyForm = $('#DangKyForm');
const CanhBao = $('#CanhBao');
const SubmitBtn = $('#submitBtn');
const BtnSpinner = $('#btnSpinner');
const BtnText = $('#btnText');

function SetLoading(isLoading) {
    SubmitBtn.prop('disabled', isLoading);
    BtnText.text(isLoading ? "Đang xử lý..." : "Đăng ký tài khoản");
    BtnSpinner.toggleClass("d-none", !isLoading);
}

function ShowAlert(message, type) {
    CanhBao.removeClass().addClass(`alert alert-${type}`).text(message).show();
}

DangKyForm.submit(async function (e) {
    e.preventDefault();

    const name = $('#name');
    const email = $('#email');
    const phone_number = $('#phone_number');
    const account_login = $('#account_login');
    const password_login = $('#password_login');
    const xacNhanMatKhau = $('#xacNhanMatKhau');
    const agreeTerms = $('#agreeTerms');

    const is_name = name.val().trim().length > 0;
    const is_email = email.val().trim().length > 0;
    const is_phone_number = phone_number.val().trim().length > 0;
    const is_account_login = account_login.val().trim().length > 0;
    const is_password_login = password_login.val().trim().length > 0;
    const is_xacNhanMatKhau = xacNhanMatKhau.val().trim().length > 0;

    name.toggleClass('is-invalid', !is_name);
    email.toggleClass('is-invalid', !is_email);
    phone_number.toggleClass('is-invalid', !is_phone_number);
    account_login.toggleClass('is-invalid', !is_account_login);
    password_login.toggleClass('is-invalid', !is_password_login);
    xacNhanMatKhau.toggleClass('is-invalid', !is_xacNhanMatKhau);

    if (!is_name || !is_email || !is_phone_number || !is_account_login || !is_password_login || !is_xacNhanMatKhau) {
        ShowAlert("Vui lòng điền đầy đủ thông tin.", "danger");
        return;
    }

    if (password_login.val().trim() !== xacNhanMatKhau.val().trim()) {
        ShowAlert("Mật khẩu không trùng khớp.", "danger");
        return;
    }

    if (!agreeTerms.is(':checked')) {
        ShowAlert("Bạn phải đồng ý với các điều khoản trước khi đăng ký.", "warning");
        return;
    }

    SetLoading(true);

    try {
        const res = await fetch("/dang-ky-tai-khoan", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                name: name.val().trim(),
                email: email.val().trim(),
                phone_number: phone_number.val().trim(),
                account_login: account_login.val().trim(),
                password_login: password_login.val().trim()
            })
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
        if (!res.ok || !data.success) {
            ShowAlert(data.message || "Đã có lỗi xảy ra, vui lòng thử lại sau.", "danger");
            return;
        }
        ShowAlert(data.message || 'Đăng nhập thành công!', 'success');
        setTimeout(() => {
            window.location.href = '/dang-nhap';
        }, 2000);
    } catch (err) {
        ShowAlert("Lỗi kết nối tới server.", "danger");
    } finally {
        SetLoading(false);
    }
});
