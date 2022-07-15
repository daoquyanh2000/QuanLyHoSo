$(function () {

    //add sự kiện nút tạo thành phần mới
    $(document).on('click', '#ThemThanhPhan', function () {
        //clear form 
        $('#NoiDungThanhPhanModal form :input').val('');
        $('#NoiDungThanhPhanModal').modal('toggle');
        $("#HiddenThanhPhanID").val(0);

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
/*    const dt = new DataTransfer();

    $(document).on('change', '#inputPDF', function () {
        // Ajout des fichiers dans l'objet DataTransfer
        html = '';
        for (var i = 0; i < this.files.length; i++) {
            html += `
            <tr data-index="${i}">
                <td >
                    ${this.files.item(i).name}
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
        for (let file of this.files) {
            dt.items.add(file);
        }
        //ve html
        // Mise à jour des fichiers de l'input file après ajout
        this.files = dt.files;
    })
    $(document).on('click', '.DeletePDF', function () {
        let index = $('.DeletePDF').closest('tr').data('index');
        dt.items.remove(index);
        // Mise à jour des fichiers de l'input file après suppression
        $('#inputPDF')[0].files = dt.files;
        $(this).closest('tr').remove();

    })*/
})

function CreateNoiDungThanhPhan() {
    var formData = new FormData($('form#NoiDungThanhPhanForm')[0]);

    //check xem up ảnh chưa
    let fileUpload = $('input#MultiImage').get(0);
    let files = fileUpload.files;
    if (files.length == 0) {
        alert("bạn chưa tải ảnh lên");
        return;
    }
    //thêm ảnh vào formdata
    for (let i = 0; i < files.length; i++) {
        formData.append(files[i].name, files[i]);
    }
    //thêm dữ liệu vào formdata
    var userID = $("#HiddenThanhPhanID").val();
    formData.append("IDHoSo", userID);

    $.ajax({
        url: "/admin/recordcontent/save",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'post',
        dataType: 'json',
        success: function (res) {
            $('#NoiDungThanhPhanModal').modal('hide');
            console.log(res.data);
            
            ShowThanhPhan($('#HiddenThanhPhanID').val());
        }
    })
}

//bấm vào thì hiện thành phần của hồ sơ đấy
function ShowThanhPhan(ID, state) {
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
            $('#HiddenThanhPhanID').val(ID);

        }
    })
}
//bấm vào show nội dung thành phần
function ShowNoiDungThanhPhan(ID, state) {
    $("#NoiDungThanhPhanModal").modal('toggle');
    //lấy nội dung thành phần từ db
}

function UploadPdf() {
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
}
$(function () {

});