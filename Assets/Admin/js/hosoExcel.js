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
            $('#HoSoExcelTable').html(res);
        },
        error: function (err) {
            console.log(err);
        }
    })
})
function ViewThanhPhanExcel(tenThuMuc) {
    let fileInput = $('#inputExcel')[0].files[0];
    let formData = new FormData();
    formData.append('file', fileInput);
    formData.append('folderName', tenThuMuc);
    $.ajax({
        url: "/admin/HoSoExcel/GetThanhPhanHoSoExcel",
        data: formData,
        type: "post",
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.state == false) {
                $.toast({
                    heading: res.heading,
                    icon: res.icon,
                    text: res.message,
                    position: 'top-right',
                    stack: 10,
                    hideAfter: 7000,
                    showHideTransition: 'slide',
                })
            } else {
                $('.modal').modal('hide');
                $('#excelContentModal').modal('show');
                $('#ThanhPhanHoSoExcelTable').html(res);
            }

        },

    })
}
function ViewPDFExcel(tenThuMuc) {
    let fileInput = $('#inputExcel')[0].files[0];
    let formData = new FormData();
    formData.append('file', fileInput);
    formData.append('folderName', tenThuMuc);
    $.ajax({
        url: "/admin/HoSoExcel/GetPDFExcel",
        data: formData,
        type: "post",
        contentType: false,
        processData: false,
        success: function (res) {
            $('.modal').modal('hide');
            $('#excelPDFModal').modal('show');
            console.log(res);
            $('#PDFExcelTable').html(res);
        },

    })
}
function PerviewPDF(pathPDF) {
    $('#IframePdfExcel').show();
    $('#IframePdfExcel').attr('src', `/Assets/Admin/pdf.js/web/viewer.html?file= ${pathPDF}`);
}

function SaveExcel() {
    let fileInput = $('#inputExcel')[0].files[0];
    let formData = new FormData();
    formData.append('file', fileInput);
    $.ajax({
        url: "/admin/HoSoExcel/Save",
        data: formData,
        type: "post",
        contentType: false,
        processData: false,
        success: function (res) {
            $.toast({
                heading: res.heading,
                icon: res.icon,
                text: res.message,
                position: 'top-right',
                stack: 10,
                hideAfter: 7000,
                showHideTransition: 'slide',

            })
            $('.modal').modal('hide');
        },

    })
}