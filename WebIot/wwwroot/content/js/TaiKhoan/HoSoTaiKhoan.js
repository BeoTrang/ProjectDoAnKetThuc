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

    } catch (err) {
        console.error("Không thể lấy hồ sơ:", err);
        alert("Có lỗi xảy ra khi tải hồ sơ. Vui lòng thử lại.");
    }
};