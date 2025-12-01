let BieuDo = null;
let currentData = null;
let currentType = null;

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

            if (!dataRes.data || dataRes.data.length === 0) {
                Swal.fire({
                    position: "top-end",
                    icon: "info",
                    title: "Không có dữ liệu để hiển thị",
                    showConfirmButton: false,
                    timer: 1500
                });
                $('#relayTableContainer').html('');
                if (BieuDo) BieuDo.destroy();
                return;
            }

            currentData = dataRes.data;
            currentType = pickType;

            switch (pickType) {
                case "TemHum":
                    TaoBieuDoTemHum(currentData, historyChartCtx);
                    $('#relayTableContainer').html('');
                    break;

                case "Relay":
                    if (BieuDo) BieuDo.destroy();
                    TaoBangRelay(currentData);
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


    $('#btnExportExcel').on('click', () => {
        if (currentData) ExportExcel(currentData, currentType);
    });

    $('#btnExportCSV').on('click', () => {
        if (currentData) ExportCSV(currentData, currentType);
    });

    $('#btnExportPDF').on('click', () => {
        if (currentData) ExportPDF(currentData, currentType);
    });

    $('#btnDownloadChart').on('click', () => {
        TaiAnh();
    });
}


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
                    <td>${new Date(d.time).toLocaleString('vi-VN')}</td>
                    <td>${d.relay1 ? '<span style="color:green;font-weight:bold;">BẬT</span>' : '<span style="color:red;">TẮT</span>'}</td>
                    <td>${d.relay2 ? '<span style="color:green;font-weight:bold;">BẬT</span>' : '<span style="color:red;">TẮT</span>'}</td>
                    <td>${d.relay3 ? '<span style="color:green;font-weight:bold;">BẬT</span>' : '<span style="color:red;">TẮT</span>'}</td>
                    <td>${d.relay4 ? '<span style="color:green;font-weight:bold;">BẬT</span>' : '<span style="color:red;">TẮT</span>'}</td>
                 </tr>`;
    });

    html += `</tbody></table>`;

    $('#relayTableContainer').html(html);
}


function ExportExcel(data, type) {
    let sheetData = [];

    if (type === "TemHum") {
        sheetData = data.map(d => ({
            "Thời gian": new Date(d.time).toLocaleString('vi-VN'),
            "Độ ẩm (%)": d.hum,
            "Nhiệt độ (°C)": d.tem
        }));
    }

    if (type === "Relay") {
        sheetData = data.map(d => ({
            "Thời gian": new Date(d.time).toLocaleString('vi-VN'),
            "Relay 1": d.relay1,
            "Relay 2": d.relay2,
            "Relay 3": d.relay3,
            "Relay 4": d.relay4
        }));
    }

    const ws = XLSX.utils.json_to_sheet(sheetData);
    const wb = XLSX.utils.book_new();

    XLSX.utils.book_append_sheet(wb, ws, "Report");

    XLSX.writeFile(wb, `BaoCao_${type}.xlsx`);
}


function ExportCSV(data, type) {
    let csv = "";

    if (type === "TemHum") {
        csv += "Time,Humidity(%),Temperature (°C)\n";
        data.forEach(d => {
            csv += `${new Date(d.time).toLocaleString('vi-VN')},${d.hum},${d.tem}\n`;
        });
    }

    if (type === "Relay") {
        csv += "Thời gian,Relay1,Relay2,Relay3,Relay4\n";
        data.forEach(d => {
            csv += `${new Date(d.time).toLocaleString('vi-VN')},${d.relay1},${d.relay2},${d.relay3},${d.relay4}\n`;
        });
    }

    const blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = `BaoCao_${type}.csv`;
    link.click();
}


async function ExportPDF(data, type) {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF("p", "mm", "a4");

    doc.setFontSize(18);
    doc.text(`Report`, 14, 16);

    if (type === "TemHum" && BieuDo) {
        const chartImage = BieuDo.toBase64Image();
        doc.addImage(chartImage, "PNG", 10, 25, 190, 90);
        doc.text("Bảng dữ liệu:", 14, 120);
    }

    let tableColumns = [];
    let tableData = [];

    if (type === "TemHum") {
        tableColumns = ["Time", "Độ ẩm (%)", "Nhiệt độ (°C)"];
        tableData = data.map(d => [
            new Date(d.time).toLocaleString('vi-VN'),
            d.hum,
            d.tem
        ]);
    }

    if (type === "Relay") {
        tableColumns = ["Time", "Relay1", "Relay2", "Relay3", "Relay4"];
        tableData = data.map(d => [
            new Date(d.time).toLocaleString('vi-VN'),
            d.relay1, d.relay2, d.relay3, d.relay4
        ]);
    }

    doc.autoTable({
        startY: type === "TemHum" ? 125 : 25,
        head: [tableColumns],
        body: tableData
    });

    doc.save(`BaoCao_${type}.pdf`);
}

function TaiAnh() {
    if (!BieuDo) {
        Swal.fire({
            icon: "warning",
            title: "Không có biểu đồ để tải",
            showConfirmButton: false,
            timer: 1000
        });
        return;
    }

    const chartImage = BieuDo.toBase64Image();

    const $link = $('<a>')
        .attr('href', chartImage)
        .attr('download', 'Chart_TemHum.png')
        .appendTo('body');

    $link[0].click();
    $link.remove();
};


