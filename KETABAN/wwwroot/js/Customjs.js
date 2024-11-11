
function createRadialBarChart(elementId, value, maxValue, label) {
    var options = {
        chart: {
            height: 350,
            type: 'radialBar',
        },
        series: [(value / maxValue) * 100],  // محاسبه درصد نسبت به حداکثر مقدار
        labels: [label],
        colors: ['#2b4a83'],  // تنظیم رنگ نمودار
        plotOptions: {
            radialBar: {
                dataLabels: {
                    name: {
                        fontSize: '22px',
                        color: '#2b4a83'  // رنگ متن توضیح
                    },
                    value: {
                        fontSize: '16px',
                        formatter: function () {
                            return value;  // نمایش مقدار واقعی در وسط نمودار
                        },
                        color: '#2b4a83'  // رنگ عدد در وسط نمودار
                    }
                }
            }
        },
        fill: {
            colors: ['#2b4a83']  // رنگ نوار دایره‌ای
        }
    };

    var chart = new ApexCharts(document.querySelector(elementId), options);
    chart.render();
}

function PersianDatapicker() {


    jalaliDatepicker.startWatch({ time: true });
}
function LoginAlert() {

    const Toast = Swal.mixin({
        toast: true,
        position: "top-end",
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.onmouseenter = Swal.stopTimer;
            toast.onmouseleave = Swal.resumeTimer;
        }
    });
    Toast.fire({
        icon: "success",
        title: "ورود با موفقیت انجام شد"
    }).then((result) => {

        window.location.href = '/Home/Index';
    });


}

function SuccessAlert(Message, targetLocation) {


    // SweetAlert script
    Swal.fire({
        position: "center",
        icon: "success",
        title: Message,
        showConfirmButton: false,
        timer: 2000,
        InputEvent: false,
    }).then((result) => {

        window.location.href = targetLocation;
    });

}



