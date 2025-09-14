const DangNhapForm = $('#DangNhapForm');
const CanhBao = $('#CanhBao');
const SubmitBtn = $('#submitBtn');
const BtnSpinner = $('#btnSpinner');
const BtnText = $('#btnText');

function SetLoading(isLoading) {
    SubmitBtn.prop('disabled', isLoading);
    BtnText.text(isLoading ? "Đang xử lý..." : "Đăng nhập");
    BtnSpinner.toggleClass("d-none", !isLoading);
}

function ShowAlert(message, type) {
    CanhBao.text(message);
    CanhBao.attr('class', `alert alert-${type}`);
}

DangNhapForm.submit(async function (e) {
    e.preventDefault();

    const taiKhoan = $('#taiKhoan');
    const matKhau = $('#matKhau');

    const isTaiKhoan = taiKhoan.val().trim().length > 0;
    const isMatKhau = matKhau.val().trim().length > 0;

    taiKhoan.toggleClass('is-invalid', !isTaiKhoan);
    matKhau.toggleClass('is-invalid', !isMatKhau);

    if (!isTaiKhoan || !isMatKhau) return;

    SetLoading(true);

    try {
        const res = await fetch("/kiem-tra-dang-nhap", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                taiKhoan: taiKhoan.val().trim(),
                matKhau: matKhau.val().trim()
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
            window.location.href = '/trang-chu';
        }, 2000);
    }
    catch (err) {
        ShowAlert('Có lỗi xảy ra, vui lòng thử lại.', 'danger');
    }
    finally {
        SetLoading(false);
    }
});