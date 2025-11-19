let BieuDo = null;
async function Init_HistoryDevice() {
    const historyChartCtx = $('#historyChart')[0].getContext('2d');
    
    const btnTaoBieuDo = $('#TaoBieuDo');
    const pickTimeInput = $('#pickTime');
    const InputDeviceId = $('#deviceId');
    const InputDeviceType = $('#deviceType');

    btnTaoBieuDo.on('click', async function () {
        showSpinner();
        try {
            const deviceId = InputDeviceId.val();
            const deviceType = InputDeviceType.val();
            const selectedTime = $('#pickTime option:selected').val();
            const pickType = $('input[name="pickType"]:checked').val();

            const res = await fetch("/api/lay-lich-su-du-lieu-thiet-bi", {
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    deviceId: deviceId,
                    typePick: pickType,
                    pickTime: selectedTime
                })
            });

            switch (pickType) {
                case "TemHum":
                    console.log("Device ID:", deviceId);
                    console.log("Device Type:", deviceType);
                    console.log("Selected Time:", selectedTime);
                    console.log("Pick Type:", pickType);

                    const dataRes = await res.json();
                    console.log("Response:", dataRes);

                    if (dataRes.success) {
                        if (BieuDo) BieuDo.destroy();
                        const data = dataRes.data;
                        TaoBieuDo(data, historyChartCtx);
                    } else {
                        Swal.fire({
                            position: "top-end",
                            icon: "error",
                            title: "Lỗi hệ thống",
                            showConfirmButton: false,
                            timer: 1000
                        });
                    }
                    break;

                case "Relay":
                    
                    break;

                default:
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "Không thấy kiểu biểu đồ!",
                        showConfirmButton: false,
                        timer: 1000
                    });
                    break;
            }
        } catch (error) {
            console.error(error);
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
}

function TaoBieuDo(data, ctx) {
    const times = data.map(d => new Date(d.time).toLocaleString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    }));

    const hums = data.map(d => d.hum);
    const temps = data.map(d => d.tem);

    BieuDo = new Chart(ctx, {
        type: 'line',
        data: {
            labels: times,
            datasets: [
                { label: 'Độ ẩm (%)', data: hums, borderColor: 'blue', fill: false },
                { label: 'Nhiệt độ (°C)', data: temps, borderColor: 'red', fill: false }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                x: {
                    ticks: {
                        maxRotation: 90,
                        minRotation: 90,
                        color: 'black',
                        font: { size: 14 },
                        autoSkip: true
                    }
                },
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}
