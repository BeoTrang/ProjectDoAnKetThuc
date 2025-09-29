let accessToken = null;

async function LayAccessToken() {
    const res = await fetch("/lay-access-token", {
        method: "GET",
        credentials: "include"
    });
    const data = await res.json();
    return data.accessToken;
}


async function Init_Dashboard() {
    accessToken = await LayAccessToken();
};