let connection;
let accessToken = '';
let url = '';

async function Init_Dashboard() {
    $('#main-content').empty();
    await LayUrl();
    await LayAccessToken();

    if (!accessToken) {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Đã hết hạn đăng nhập, yêu cầu đăng nhập lại!"
        });
        return;
    }

    await LayDanhSachThietBi();
    await ConnectSignalR();
    BindReconnectOnFocus();
}

async function LayUrl() {
    const res = await fetch("/lay-url-api");
    const json = await res.json();
    url = `${json.data}/deviceHub`;
}

async function LayAccessToken() {
    const res = await fetch("/lay-access-token", {
        method: "GET",
        credentials: "include"
    });
    const data = await res.json();
    accessToken = data.success ? data.accessToken : '';
}

async function ConnectSignalR() {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        console.log("⚙️ SignalR đã kết nối");
        return;
    }

    if (connection) {
        await connection.stop();
    }

    connection = new signalR.HubConnectionBuilder()
        .withUrl(url, { accessTokenFactory: () => accessToken })
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .build();

    connection.on("JoinedGroup", () => console.log("✅ Đã tham gia group"));
    connection.on("DeviceData", payload => {
        const data = JSON.parse(payload);
        if (data.type === "AX01") InsertDataAX01(data);
    });
    connection.on("DeviceStatus", payload => {
        const data = JSON.parse(payload);
        InsertStatus(data);
    });

    connection.onreconnecting(() => console.warn("⚠️ Mất kết nối, đang thử reconnect..."));
    connection.onreconnected(() => console.log("🔁 Đã reconnect thành công!"));
    connection.onclose(err => console.warn("🔌 Kết nối bị đóng:", err));

    try {
        await connection.start();
        console.log("✅ Connected to SignalR");
        await connection.invoke("JoinGroup");
        console.log("📡 Đã join group");
    } catch (err) {
        console.error("❌ Lỗi kết nối SignalR:", err);
    }
}

function BindReconnectOnFocus() {
    $(document).on("visibilitychange", async () => {
        if (document.visibilityState === "visible") {
            console.log("📱 App quay lại foreground");
            await ReconnectSignalR();
        }
    });

    $(window).on("focus", async () => {
        console.log("🪟 Cửa sổ được focus lại");
        await ReconnectSignalR();
    });
}

async function ReconnectSignalR() {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        console.log("✅ SignalR vẫn đang hoạt động");
        return;
    }

    await LayAccessToken();

    if (!accessToken) {
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Đã hết hạn đăng nhập, yêu cầu đăng nhập lại!"
        });
        return;
    }

    await ConnectSignalR();

    Swal.fire({
        position: "top-end",
        icon: "success",
        title: "Đã kết nối lại!",
        showConfirmButton: false,
        timer: 1000
    });
}



async function InsertDataAX01(data) {
    console.log("Data: ", data);

    const ELtem = $(`#tem_${data.id}`);
    const ELhum = $(`#hum_${data.id}`);
    const ELtime = $(`#time_${data.id}`);

    ELtem.text(data.data.tem);
    ELhum.text(data.data.hum);
    ELtime.text(data.timestamp);

    const relays = data.relays;

    const ELrelay1 = $(`#relay1_${data.id}`);
    const ELrelay2 = $(`#relay2_${data.id}`);
    const ELrelay3 = $(`#relay3_${data.id}`);
    const ELrelay4 = $(`#relay4_${data.id}`);


    ELrelay1.prop("checked", relays.relay1 === 1);
    ELrelay2.prop("checked", relays.relay2 === 1);
    ELrelay3.prop("checked", relays.relay3 === 1);
    ELrelay4.prop("checked", relays.relay4 === 1);
}

$(document).on('change', '.relaySwitch', async function () {
    const el = $(this);
    const id = el.attr('id');
    const [relayName, deviceId] = id.split('_');
    const newState = el.prop('checked') ? "1" : "0"; 
    const oldState = newState === "1" ? "0" : "1"; 

    console.log("relayName:", relayName);
    console.log("deviceId:", deviceId);
    console.log("Trạng thái mới:", newState);

    

    try {
        const payload = JSON.stringify({ [relayName]: newState });
        const res = await fetch("/dieu-khien-thiet-bi", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                deviceId: deviceId,
                payload: payload
            })
        });
        data = await res.json();
        if (data.success) {
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Điều khiển thành công!",
                showConfirmButton: false,
                timer: 1000
            });
        }
        else {
            Swal.fire({
                position: "top-end",
                icon: "error",
                title: data.message,
                showConfirmButton: false,
                timer: 1000
            });
            setTimeout(() => el.prop("checked", oldState === "1"), 50);
        }
        console.log(data);
        
    } catch (err) {
        setTimeout(() => el.prop("checked", oldState === "1"), 50);
    }
});

$(document).on('click', '.device-setting', async function () {
    showSpinner();
    const el = $(this);
    const id = el.attr('id');

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
            $('#main-content').empty().append(html);
            Init_SettingThietBi();
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
});



async function InsertStatus(data) {
    console.log("Status: ", data);
    const status = $(`#status_${data.id}`);
    if (!status) {
        console.log("Không tìm thấy status");
    }
    console.log("Status: ",data.status);
    if (data.status == "1") {
        console.log("Thiết bị online");
        status.text("Online")
            .removeClass("bg-secondary")
            .addClass("bg-success");
    }
    else if (data.status == "0") {
        console.log("Thiết bị offline");
        status.text("Offline")
            .removeClass("bg-success")
            .addClass("bg-secondary");
    }
}

async function LayDanhSachThietBi() {
    const res = await fetch("/lay-danh-sach-thiet-bi", {
        method: "GET",
        headers: { 'Content-Type': 'application/json' }
    });

    const json = await res.json();
    const data = json.data;
    console.log(data);
    var dashboard = $('#main-content');

    for (const device of data) {
        const viewRes = await fetch("/view-cho-thiet-bi", {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                deviceId: device.deviceId,
                deviceType: device.deviceType
            })
        });

        const html = await viewRes.text();
        dashboard.append(html);
    }
}




