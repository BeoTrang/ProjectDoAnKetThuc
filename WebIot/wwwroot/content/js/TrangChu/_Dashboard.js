

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

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("JoinedGroup", group => {
        console.log("✅ Đã tham gia group:", group);
    });

    connection.on("DeviceData", (payload) => {
        console.log("📦 Dữ liệu real-time nhận được:", payload);

    });

    try {
        await connection.start();
        console.log("✅ Connected to SignalR");

        await connection.invoke("JoinGroup", "DeviceId_1");
        console.log("📡 Đã join group DeviceId_1");

    } catch (err) {
        console.error("❌ Lỗi khi kết nối SignalR:", err);
    }
}



