$(function () {

    //add sự kiện nút tạo thành phần mới
    $(document).on('click', '#ThemThanhPhan', function () {
        //clear form 
        ResetForm();
    })
    // form validation
    $("#NoiDungThanhPhanForm").validate({
        rules: {
            "TenNgan": {
                required: true,
                maxlength: 20,
                minlength: 3,
            },
            "TrangThai": {
                required: true,
            },
            "IDKho": {
                required: true,
            },
            "KichThuoc": {
                required: true,
            },
            "MaNgan": {
                required: true,
                maxlength: 20,
            },
        },
        messages: {
            "TenNgan": {
                required: "Bắt buộc nhập tên ngăn",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 3 ký tự"
            },
            "TrangThai": {
                required: "Bắt buộc nhập trạng thái",
            }, "MaNgan": {
                required: "Bắt buộc nhập mã ngăn",
                maxlength: "Hãy nhập tối đa 20 ký tự",
            },
            "IDKho": {
                required: "Bắt buộc nhập kho chứa ngăn",
            },
            "KichThuoc": {
                required: "Bắt buộc nhập kích thước",
            },
        },
        submitHandler: function () {
            CreateNoiDungThanhPhan();
        }
    });
/*    $(document).on('change', '#inputPDF', function () {
        UploadPdf();
    })
    $(document).on('click', '[id*=PDFTable] .ViewPDF', function () {
        var fileId = $(this).data("id");
        $.ajax({
            type: "POST",
            url: "/admin/recordcontent/GetPDF",
            data: "{fileId: " + fileId + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {
                console.log(res.data.PathPDF)
                //check dinh dang
                let name = res.data.PathPDF.split('.')[1]
                if (name == "pdf") {
                    $('#IframePdf').attr('src', `/Assets/Admin/pdf.js/web/viewer.html?file= ${res.data.PathPDF}`);

                } else {
                    $('#IframePdf').attr('src', `${res.data.PathPDF}`);

                }
            }
        })
    })*/
    //thêm file vào PDFTable 
    var dt = new DataTransfer();
    $(document).on('change', '#inputPDF', function () {

        // thêm mới vào trong dom
        let html = '';
        if ($('#HiddenThanhPhanHoSoID').val() == 0) {
            html = `<tr>
                <th>Tên file</th>
                <th>Chức năng</th>
                </tr>`;
        }

        for (var i = 0; i < this.files.length; i++) {
            html += `
            <tr data-date=${this.files[i].lastModified} class="newPDF">
                <td >
                    ${this.files[i].name}
                </td>
                <td>
                    <div class="btn-group btn-group-sm" role="group" aria-label="Basic mixed styles example">
                        <button type="button" class="btn btn-outline-secondary ViewPDF" >
                            <span>Xem</span>
                        </button>
                        <button type="button" class="btn btn-outline-secondary DeletePDF" >
                            <span>Xóa </span>
                        </button>
                    </div>
                </td>
            </tr>
            `
        }

        $('#tablePDF').append(html);

        //thêm mới vào trong datatranfers
        for (let file of this.files) {
            dt.items.add(file);
        }
        // add lại vào files trong trường hợp người dùng submit luôn
        this.files = dt.files;
    })

    //xóa PDF mới
    $(document).on('click', '.DeletePDF', function () {
        let date = $(this).closest('tr').data('date');
        //xóa trong datatranfers dựa vào data-date
        for (let i = 0; i < dt.files.length; i++) {
            if (dt.files[i].lastModified == date) {
                console.log("da xoa");
                dt.items.remove(i);
                break;
            }
        }
        //add lại datatranfers vào files
        $('#inputPDF')[0].files = dt.files;

        //xóa tr trong dom 
        $(this).closest('tr').remove();
    })

    //xem PDF mới
    $(document).on('click', '.ViewPDF', function () {
        $('#IframePdf').show();
        let url=''
        //dựa vào date để tìm ra cái file đấy
        let date = $(this).closest('tr').data('date');
        for (let i = 0; i < dt.files.length; i++) {
            if (dt.files[i].lastModified == date) {

                url = URL.createObjectURL(dt.files[i]);
                $('#IframePdf').attr('src', `/Assets/Admin/pdf.js/web/viewer.html?file= ${url}`);

/*                $('#IframePdf')[0].onload = function () {
                    URL.revokeObjectURL(url);
                    console.log(url);
                }*/
                console.log(url);
                break;
            }
        }
    })

})
function ResetForm() {
    $('#NoiDungThanhPhanModal form :input').val('');
    $('#NoiDungThanhPhanModal').modal('toggle');
    $('#HiddenThanhPhanHoSoID').val(0);
    $('#tablePDF').html('');
    dt = new DataTransfer();
    $('#inputPDF')[0].files = dt.files;
}

/// code của thành phần
//bấm vào thì hiện thành phần của hồ sơ đấy
function ShowThanhPhan(ID, state) {
    //set ID của hồ sơ để về sau còn lưu thành phần
    $('#HiddenHoSoID').val(ID);

    $('.checkboxs').prop('checked', false);
    $.ajax({
        url: "/admin/RecordContent/Search",
        data: {
            ID: ID
        },
        type: 'post',
        success: function (res) {
            console.log(res);
            $('#DeleteAllThanhPhan').addClass('disabled', true);
            $('#ThanhPhanTable').html(res);
            $("#ThanhPhanModal").modal('show');

        }
    })
}

/// code của nội dung thành phần
//bấm vào show nội dung thành phần
function ShowNoiDungThanhPhan(ID, state) {
    $("#NoiDungThanhPhanModal").modal('toggle');
    $('#HiddenThanhPhanHoSoID').val(ID);
    //lấy nội dung thành phần từ db
    $.ajax({
        url: "/admin/RecordContent/View",
        data: {
            ID: ID
        },
        type: 'get',
        success: function (res) {
            console.log(res);

            //update dữ liệu TPHS từ db
            let TPHS = res.TPHS;
            $('#ChuThich').val(TPHS.ChuThich)
            $('#IDLoaiThanhPhan').val(TPHS.IDLoaiThanhPhan)
            $('#KiHieu').val(TPHS.KiHieu)
            $('#MaThanhPhan').val(TPHS.MaThanhPhan)
            $('#TieuDeThanhPhan').val(TPHS.TieuDe)
            $('#TrangThai').val(TPHS.TrangThai)

            //update dữ liệu file từ db
            let files = res.PDFs;
            //vẽ ra html 
            //khi tạo mới thì phải thêm thead

            let html = `<tr>
                <th>Tên file</th>
                <th>Chức năng</th>
                </tr>`;
            $(files).each(function (index,e) {
                html += `
            <tr data-id=${e.ID} class="oldPDF">
                <td >
                    ${e.TenPDF}
                </td>
                <td>
                    <div class="btn-group btn-group-sm" role="group" aria-label="Basic mixed styles example">
                        <button type="button" class="btn btn-outline-secondary " onclick="ViewPDF(${e.ID})" >
                            <span>Xem</span>
                        </button>
                        <button type="button" class="btn btn-outline-secondary " onclick="DeletePDF(${e.ID}">
                            <span>Xóa </span>
                        </button>
                    </div>
                </td>
            </tr>
            `
            })
            $('#tablePDF').html(html);
            
        }
    })
}

function CreateNoiDungThanhPhan() {
    // var formData = new FormData($('form#NoiDungThanhPhanForm')[0]);
    var formData = new FormData();
    let fileUpload = $('input#inputPDF').get(0);
    let files = fileUpload.files;
/*    //check xem up ảnh chưa
    let fileUpload = $('input#inputPDF').get(0);
    let files = fileUpload.files;
    if (files.length == 0) {
        alert("bạn chưa tải ảnh lên");
        return;
    }*/
    //thêm dữ liệu vào formdata
    var IDHoSo = $("#HiddenHoSoID").val();
    var IdTPHS = $("#HiddenThanhPhanHoSoID").val();
    formData.append("ID", IdTPHS);
    formData.append("IDHoSo", IDHoSo);
    formData.append("TieuDe", $('#TieuDeThanhPhan').val());
    formData.append("IDLoaiThanhPhan", $('#IDLoaiThanhPhan').val());
    formData.append("KiHieu", $('#KiHieu').val());
    formData.append("MaThanhPhan", $('#MaThanhPhan').val());
    formData.append("TrangThai", $('#TrangThai').val());
    formData.append("ChuThich", $('#ChuThich').val());
    //thêm files vào formdata
    for (let file of files) {
        formData.append("FilePDF", file);
    }
    $.ajax({
        url: "/admin/recordcontent/Save",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'post',
        dataType: 'json',
        success: function (res) {
            $('#NoiDungThanhPhanModal').modal('hide');
            console.log(res.data);
            ShowThanhPhan($('#HiddenHoSoID').val());
        }
    })
}
function ViewPDF(ID) {
    $.ajax({
        url: "/admin/RecordContent/ViewPDF",
        data: {
            ID: ID
        },
        type: 'post',
        success: function (res) {
            $('#IframePdf').show();
            $('#IframePdf').attr('src', `/Assets/Admin/pdf.js/web/viewer.html?file= ${res.data}`);
        }
    })
}
function DeletePDF(ID) {
    $.ajax({
        url: "/admin/RecordContent/DeletePDF",
        data: {
            ID: ID
        },
        type: 'get',
        success: function (res) {
           
        }
    })
}
/*function UploadPdf() {
    var formData = new FormData($('form#NoiDungThanhPhanForm').get(0));
    //check xem up ảnh chưa
    let fileUpload = $('input#inputPDF').get(0);
    let files = fileUpload.files;
    if (files.length == 0) {
        alert("bạn chưa tải ảnh lên");
        return;
    }
    $.ajax({
        url: "/admin/recordcontent/SavePDF",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'post',
        success: function (res) {
            console.log(res);

            $('#tablePDF').html(res);
        },
        error: function (err) {
            console.log(err);
        }

    })
}*/
$(function () {

});