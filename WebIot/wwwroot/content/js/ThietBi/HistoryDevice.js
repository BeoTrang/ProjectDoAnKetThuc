async function Init_HistoryDevice() {
    const historyChart = $('#historyChart')[0].getContext('2d');
    let BieuDo;
    const TaoBieuDo = $('#TaoBieuDo');
    const pickTime = $('#pickTime');
    const InputDeviceId = $('#deviceId');
    const InputDeviceType = $('#deviceType');

    TaoBieuDo.on('click', async function () {
        showSpinner();
        try {
            const deviceId = InputDeviceId.val();
            const deviceType = InputDeviceType.val();

            if (deviceType == "AX01") {
                const pickTime = $('#pickTime option:selected').val();
                const pickType = $('input[name="pickType"]:checked').val();

                console.log(deviceId);
                console.log(deviceType);
                console.log(pickTime);
                console.log(pickType);

                const res = await fetch("/lay-lich-su-du-lieu-thiet-bi", {
                    method: "POST",
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        deviceId: deviceId,
                        typePick: pickType,
                        pickTime: pickTime
                    })
                });
                const dataRes = await res.json();
                console.log(dataRes);
                if (dataRes.success) {
                    if (BieuDo) BieuDo.destroy();
                    const data = await dataRes.data;
                    var times = data.map(d => new Date(d.time).toLocaleString('vi-VN', {
                        year: 'numeric',
                        month: '2-digit',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit',
                        second: '2-digit'
                    }));
                    var hums = data.map(function (d) { return d.hum; });
                    var temps = data.map(function (d) { return d.tem; });
                    BieuDo = new Chart(historyChart, {
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
                                        font: {
                                            size: 14  
                                        },
                                        autoSkip: true
                                        //maxTicksLimit: 13 
                                    }
                                },
                                y: {
                                    beginAtZero: true
                                }
                            }
                        }
                    });
                }
                else {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "Lỗi hệ thống",
                        showConfirmButton: false,
                        timer: 1000
                    });
                }

            }
        }
        catch {
            Swal.fire({
                position: "top-end",
                icon: "error",
                title: "Lỗi hệ thống",
                showConfirmButton: false,
                timer: 1000
            });
        }
        finally {
            hideSpinner();
        }
    });
}