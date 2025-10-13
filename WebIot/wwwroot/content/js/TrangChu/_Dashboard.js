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

    const url = `${json.data}/deviceHub?access_token=${accessToken}`;
    LayDanhSachThietBi();
    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(url)
            .configureLogging(signalR.LogLevel.Information)
            .build();
        connection.on("JoinedGroup", () => {
            console.log("✅ Đã tham gia group");
        });

        connection.on("DeviceData", (payload) => {
            var data = JSON.parse(payload);
            console.log("📦 Data nhận được:", data);

        });

        connection.on("DeviceStatus", (payload) => {
            console.log("📦 Status nhận được:", payload);

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




