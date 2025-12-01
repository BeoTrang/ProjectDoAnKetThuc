let BieuDo = null;

async function Init_HistoryDevice() {
    const historyChartCtx = $('#historyChart')[0].getContext('2d');

    const btnTaoBieuDo = $('#TaoBieuDo');
    const InputDeviceId = $('#deviceId');

    btnTaoBieuDo.on('click', async function () {
        showSpinner();
        try {
            const deviceId = InputDeviceId.val();
            const pickType = $('input[name="pickType"]:checked').val();
            const selectedTime = $('#pickTime option:selected').val();

            const res = await fetch("/api/lay-lich-su-du-lieu-thiet-bi", {
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    deviceId: deviceId,
                    typePick: pickType,
                    pickTime: selectedTime
                })
            });

            const dataRes = await res.json();

            if (!dataRes.success) {
                Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: "Lỗi hệ thống",
                    showConfirmButton: false,
                    timer: 1000
                });
                return;
            }

            switch (pickType) {
                case "TemHum":
                    TaoBieuDoTemHum(dataRes.data, historyChartCtx);
                    $('#relayTableContainer').html(''); // Xóa bảng Relay nếu có
                    break;

                case "Relay":
                    if (BieuDo) BieuDo.destroy(); // Xóa chart TemHum nếu có
                    TaoBangRelay(dataRes.data);
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

// TemHum line chart
function TaoBieuDoTemHum(data, ctx) {
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

    if (BieuDo) BieuDo.destroy();

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
                y: { beginAtZero: true }
            }
        }
    });
}

// Relay table với ngày + giờ đầy đủ
function TaoBangRelay(data) {
    if (!data || data.length === 0) {
        $('#relayTableContainer').html('<p>Không có dữ liệu</p>');
        return;
    }

    let html = `<table class="table table-bordered table-striped">
                    <thead>
                        <tr>
                            <th>Thời gian</th>
                            <th>Relay 1</th>
                            <th>Relay 2</th>
                            <th>Relay 3</th>
                            <th>Relay 4</th>
                        </tr>
                    </thead>
                    <tbody>`;

    data.forEach(d => {
        html += `<tr>
                    <td>${new Date(d.time).toLocaleString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        })}</td>
                    <td>${d.relay1 === 1 ? '<span style="color:green;font-weight:bold;">BẬT</span>' : '<span style="color:red;">TẮT</span>'}</td>
                    <td>${d.relay2 === 1 ? '<span style="color:green;font-weight:bold;">BẬT</span>' : '<span style="color:red;">TẮT</span>'}</td>
                    <td>${d.relay3 === 1 ? '<span style="color:green;font-weight:bold;">BẬT</span>' : '<span style="color:red;">TẮT</span>'}</td>
                    <td>${d.relay4 === 1 ? '<span style="color:green;font-weight:bold;">BẬT</span>' : '<span style="color:red;">TẮT</span>'}</td>
                 </tr>`;
    });

    html += `</tbody></table>`;

    $('#relayTableContainer').html(html);
}
