$(function () {

    //add sự kiện nút tạo thành phần mới
    $(document).on('click', '#ThemThanhPhan', function () {
        //clear form 
        ResetForm();
    })
    // Execute a function when the user presses a key on the keyboard
    let input = $('#searchThanhPhan').get(0);
    input.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            $('#searchBtnThanhPhan').click();
        }
    });
    // form validation
    $("#NoiDungThanhPhanForm").validate({
        rules: {
            "TieuDeThanhPhan": {
                required: true,
                maxlength: 20,
                minlength: 3,
            },
            "MaThanhPhan": {
                required: true,
                maxlength: 20,
                minlength: 3,
            },
            "TrangThai": {
                required: true,
            },
            "IDLoaiThanhPhan": {
                required: true,
            },
            "ChuThich": {
                maxlength: 20,
            },
            "KiHieu": {
                maxlength: 20,
            },
        },
        messages: {
            "TieuDeThanhPhan": {
                required: "Bắt buộc nhập tiêu đề",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 3 ký tự"
            },
            "MaThanhPhan": {
                required: "Bắt buộc nhập mã thành phần",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 3 ký tự"
            },
            "TrangThai": {
                required: "Bắt buộc nhập trạng thái",
            },
            "IDLoaiThanhPhan": {
                required: "Bắt buộc nhập loại thành phần",
            },
            "ChuThich": {
                maxlength: "Hãy nhập tối đa 20 ký tự",

            },
            "KiHieu": {
                maxlength: "Hãy nhập tối đa 20 ký tự",
            },
        },
        submitHandler: function () {
            CreateNoiDungThanhPhan();
        }
    });

    //add sự kiện thêm file PDF trong thành phần hồ sơ
    var dt = new DataTransfer();
    var firstTime = 0;
    $(document).on('change', '#inputPDF', function () {
        //nếu người dùng tạo mới hoặc sửa thì phải reset cái dt
        if ($('#HiddenHandleDt').val() == 0) {
            dt = new DataTransfer();
        }
        //0:trạng thái vừa mới chỉnh sửa hoặc đang tạo mới
        //1: trạng thái đang chỉnh sửa hoặc đang tạo mới
        $('#HiddenHandleDt').val(1);
        // thêm mới vào trong dom
        let html = '';
        //nếu chỉnh sửa thành phần thì không cần phải thêm thead vào nữa
        if ($('#HiddenThanhPhanHoSoID').val() != 0) {
            firstTime = 1;
        }
        //thêm thead vào 1 lần duy nhất
        if (firstTime == 0  ) {
            html = `<tr>
                <th>Tên file</th>
                <th>Chức năng</th>
                </tr>`;
        }
        firstTime = 1;
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
        //mở file đầu tiên lên cho người dùng xem luôn
        $($('.ViewPDF:last')).trigger('click');
    })

    //xóa PDF mới
    $(document).on('click', '.DeletePDF', function () {
        if (confirm("Bạn có muốn xóa file này không ?")) {
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
            //ẩn hiện thị pdf
            $('#IframePdf').attr('src', `/Assets/Admin/pdf.js/web/viewer.html?file=`);

        }

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
    $('#IframePdf').hide();
    $('#HiddenHandleDt').val(0);
}

/// code của thành phần
//bấm vào thì hiện thành phần của hồ sơ đấy
function ShowThanhPhan(ID,HoSoState) {
    //set ID của hồ sơ để về sau còn lưu thành phần
    $('#HiddenHoSoID').val(ID);
    //lấy thông tin trong ô tìm kiếm
    let page = $('#ThanhPhanModal').find('.active').text()
    let keyword =$('#searchThanhPhan').val();
    $('.checkboxs').prop('checked', false);
    $.ajax({
        url: "/admin/RecordContent/Search",
        data: {
            ID: $('#HiddenHoSoID').val(),
            keyword: keyword,
            page: page,
            HoSoState: HoSoState,
        },
        type: 'post',
        success: function (res) {
            $('#DeleteAllThanhPhan').addClass('disabled', true);
            $('#ThanhPhanTable').html(res);
            $("#ThanhPhanModal").modal('show');
            if (HoSoState == 1) {
                $('#DeleteAllThanhPhan').hide();
                $('#ThemThanhPhan').hide();
            } else {
                $('#NoiDungThanhPhanForm :input').prop('disabled', false);
                $('#DeleteAllThanhPhan').show();
                $('#ThemThanhPhan').show();


            }
        }
    })
}
function SearchThanhPhan() {
    let page = $('#ThanhPhanModal').find('.active').text()
    let keyword = $('#searchThanhPhan').val();
    $.ajax({
        url: "/admin/RecordContent/Search",
        data: {
            ID: $('#HiddenHoSoID').val(),
            keyword: keyword,
            page: page,
            HoSoState: HoSoState
        },
        type: 'post',
        success: function (res) {
            $('#ThanhPhanTable').html(res);
        }
    })
}
/// code của nội dung thành phần
//bấm vào show nội dung thành phần
function ShowNoiDungThanhPhan(ID, HoSoState) {
    $("#NoiDungThanhPhanModal").modal('toggle');
    $('#HiddenThanhPhanHoSoID').val(ID);
    $('#HiddenHandleDt').val(0);
    //lấy nội dung thành phần từ db
    $.ajax({
        url: "/admin/RecordContent/View",
        data: {
            ID: ID
        },
        type: 'get',
        success: function (res) {
            $("#NoiDungThanhPhanModal").modal('show');
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
                        <button type="button" class="btn btn-outline-secondary oldPDF-Button" onclick="ViewPDF(${e.ID})" >
                            <span>Xem</span>
                        </button>
                        <button type="button" class="btn btn-outline-secondary " onclick="DeletePDF(${e.ID},this)">
                            <span>Xóa </span>
                        </button>
                    </div>
                </td>
            </tr>
            `
            })

            $('#tablePDF').html(html);
            $('#IframePdf').hide();
            //nếu state ==1 thì đóng băng lại chỉ cho xem thôi
            if (HoSoState == 1) {
                $('#NoiDungThanhPhanForm :input').prop('disabled', true);
                $('.oldPDF ').find('button:first').prop('disabled', false);
                $('#NoiDungThanhPhanModal button[type="submit"]').prop('disabled', true);
            } else {
                $('#NoiDungThanhPhanForm :input').prop('disabled', false);
                $('#NoiDungThanhPhanModal button[type="submit"]').prop('disabled', false);

            }
           
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
            $('#IframePdf').hide();
            $('#NoiDungThanhPhanModal').modal('hide');
            console.log(res.data);
            ShowThanhPhan($('#HiddenHoSoID').val(),0);
            $.toast({
                heading: res.heading,
                icon: res.icon,
                text: res.message,
                position: 'top-right',
                stack: 10,
                hideAfter: 7000,
                showHideTransition: 'slide',
            })
            $('#HiddenHandleDt').val(0);
        }
    })
}

function DeleteNoiDungThanhPhan(ID) {
    if (confirm("Bạn có chắc chắn xóa người dùng này không?")) {
        event.preventDefault();
        $.ajax({
            url: "/admin/RecordContent/delete",
            data: { ID: ID },
            type: 'get',
            dataType: 'json',
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
                ShowThanhPhan($('#HiddenHoSoID').val(),0);
            }
        })
    }
}
function DeleteAll() {
    let arr = [];
    let checkboxs = $(".checkboxs:checked");
    $.each(checkboxs, function (index, value) {
        arr.push($(this).data('id'))
    })
    if (confirm(`Bạn thực sự muốn xóa ${arr.length} bản ghi ?`)) {
        $.ajax({
            type: "get",
            url: "/admin/RecordContent/DeleteAll",
            data: $.param({ checkboxs: arr }, true),
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
                ShowThanhPhan($('#HiddenHoSoID').val(),0);
                $('#DeleteAllThanhPhan').toggleClass('disabled');
            }
        })
    }
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
function DeletePDF(ID, e) {
    if (confirm("Bạn thực sự muốn xóa file này ?")) {
        $.ajax({
            url: "/admin/RecordContent/DeletePDF",
            data: {
                ID: ID
            },
            type: 'get',
            success: function (res) {
                /*            ShowNoiDungThanhPhan($('#HiddenThanhPhanHoSoID').val())
                            $("#NoiDungThanhPhanModal").modal('toggle');*/
                $(e).closest('tr').remove();
                $('#IframePdf').hide();
                $.toast({
                    heading: res.heading,
                    icon: res.icon,
                    text: res.message,
                    position: 'top-right',
                    stack: 10,
                    hideAfter: 7000,
                    showHideTransition: 'slide',
                })
            }
        })
    }

}
