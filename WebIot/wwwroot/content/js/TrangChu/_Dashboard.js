let connection;

async function LayAccessToken() {
    const res = await fetch("/lay-access-token", {
        method: "GET",
        credentials: "include"
    });
    const data = await res.json();
    return data.accessToken;
}

async function Init_Dashboard() {
    let accessToken = await LayAccessToken();
    
    const res = await fetch("/lay-url-api", {
        method: "GET"
    });
    const json = await res.json();

    const url = `${json.data}/deviceHub`;
    LayDanhSachThietBi();
    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(url, {
                accessTokenFactory: () => accessToken
            })
            .configureLogging(signalR.LogLevel.Information)
            .build();
        connection.on("JoinedGroup", () => {
            console.log("✅ Đã tham gia group");
        });

        connection.on("DeviceData", (payload) => {
            const data = JSON.parse(payload);
            switch (data.type) {
                case "AX01":
                    InsertDataAX01(data);
                    break;
            }

        });

        connection.on("DeviceStatus", (payload) => {
            console.log("📦 Status nhận được:", payload);
            const data = JSON.parse(payload);
            InsertStatus(data);
        }); 
        connection.on("ConnectionExpired", msg => {
            console.warn("⏰ " + msg);
            alert(msg);

            connection.stop();
        });

        connection.on("forceDisconnect", () => {
            console.warn("⚠️ Server yêu cầu ngắt kết nối");
            connection.stop();
        });


        try {
            await connection.start();
            console.log("✅ Connected to SignalR");

            await connection.invoke("JoinGroup");
            console.log("📡 Đã join group");

        } catch (err) {
            console.error("❌ Lỗi khi kết nối SignalR:", err);
        }
    }
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

    const payload = JSON.stringify({ [relayName]: newState });

    try {
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
            el.prop("checked", oldState === "1");
            Swal.fire({
                position: "top-end",
                icon: "error",
                title: data.message,
                showConfirmButton: false,
                timer: 1000
            });
            
        }
        console.log(data);
        
    } catch (err) {
        el.prop("checked", oldState === "1");
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
        console.log(device);
        dashboard.append(html);
    }
}




