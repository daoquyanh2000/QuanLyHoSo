//add sự kiện đẩy file excel trong hồ sơ
$(document).on('change', '#inputExcel', function () {
    let formData = new FormData();
    if (this.files.length > 0) {
        formData.append("excelFile", this.files[0]);
    }
    $.ajax({
        url: "/admin/HoSoExcel/UnZipFile",
        data: formData,
        type: "post",
        contentType: false,
        processData: false,
        success: function (res) {
            //reset lại inputExcel 
            let dt = new DataTransfer();
            this.files = dt.files;
            console.log(res);
            $('#hoSoExcelTable').html(res);
        },
        error: function (err) {
            console.log(err);
            $('#hoSoExcelTable').html(err.responseText);

        }
    })
})
function ViewThanhPhanExcel(tenThuMuc) {
    $.ajax({
        url: "/admin/HoSoExcel/GetThanhPhanHoSoExcel",
        data: {
            folderName: tenThuMuc,
        },
        type: "get",

        success: function (res) {
            //reset lại inputExcel 
            let dt = new DataTransfer();
            this.files = dt.files;
            console.log(res);
            $('#hoSoExcelTable').html(res);
        },
        error: function (err) {
            console.log(err);
            $('#hoSoExcelTable').html(err.responseText);

        }
    })
}
