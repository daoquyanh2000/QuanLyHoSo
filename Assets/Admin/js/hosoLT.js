
var danhMuc = "";
var kho = "";
var ngan = "";
var kyHieu = "";
var maHoSo = "";
var HoSoState=0;
$(function () {
    // tìm kiếm khi bấm enter
    $(document).on('keypress', '#searchInput_0', function (e) {
        if (e.which == 13) {
            $('#searchBtn_0').click();
        }
    })
        .on('keypress', '#searchInput_1', function (e) {
            if (e.which == 13) {
                $('#searchBtn_1').click();
            }
        })
    //Add sự kiện nút tìm kiếm
    $(document).on('click', '#searchBtn_0', function (e) {
        SearchUser(0);
    })
        .on('click', '#searchBtn_1', function (e) {
            SearchUser(1);
        })
    $("#collapseExample4").addClass('show');
    $('a[href="#collapseExample4"]').attr("aria-expanded", true)
    ////khi bam nut tao user moi
    $('button[data-bs-toggle="modal"]').click(function () {
        EnableForm();
        danhMuc = "";
        kho = "";
        ngan = "";
        kyHieu = "";
        maHoSo = "";
        $("#HiddenID").val(0);
        $("#userImg").hide();
        $("#imgTemp").show();
    })

   //
    //xu ly anh trong khi them thanh vien
    $('#userImg').hide();
    //xử lý html table giữa 2 trạng thái hồ sơ

    // form validation
    $("#userForm").validate({
        rules: {
            "TieuDe": {
                required: true,
                maxlength: 20,
                minlength: 3,
            },
            "MaHoSo": {
                required: true,
                maxlength: 50,
                minlength: 3,
            },
            "MaDanhMuc": {
                required: true,
            },
            "MaKho": {
                required: true,
            },
            "MaNgan": {
                required: true,
            },
            "ThoiGianLuuTru": {
                required: true,
                checkDate: true,
            },
            "ThoiHanBaoQuan": {
                required: true,
                number: true,
            },
            "MaLoaiHoSo": {
                required: true,
            },
            "KyHieu": {
                required: true,
                maxlength: 20,
                minlength: 3,
            },
        },
        messages: {
            "TieuDe": {
                required: "Bắt buộc nhập trường này",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 3 ký tự"
            },
            "MaHoSo": {
                required: "Bắt buộc nhập trường này",
                maxlength: "Hãy nhập tối đa 50 ký tự",
                minlength: "Hãy nhập ít nhất 3 ký tự"
            },

            "MaDanhMuc": {
                required: "Bắt buộc nhập trường này",
            },
            "MaKho": {
                required: "Bắt buộc nhập trường này",
            },
            "MaNgan": {
                required: "Bắt buộc nhập trường này",
            },
            "ThoiGianLuuTru": {
                required: "Bắt buộc nhập trường này",
            },
            "ThoiHanBaoQuan": {
                required: "Bắt buộc nhập trường này",
                number: "Hãy nhập số năm",
            },
            "MaLoaiHoSo": {
                required: "Bắt buộc nhập trường này",
            },
            "KyHieu": {
                required: "Bắt buộc nhập trường này",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 3 ký tự"
            },
        },
        submitHandler: function () {
            CreateUser();
        }

    });
    jQuery.validator.addMethod("checkDate", function (value, element) {
        value = new Date(value);
        return (Date.parse(value) > Date.now())
    }, "Hãy nhập ngày lớn hơn hôm nay");
    //chọn tất cả và bỏ chọn tất cả
    $(document).on('change', '#checkAll', function () {
        console.log(this);
        $(this).toggleClass('checkAll');
        let checkbox = $(this).closest('table').find('.checkboxs');
        checkbox.prop('checked', $(this).hasClass('checkAll'))

        let checkboxChecked = $(this).closest('table').find('.checkboxs:checked');
        let deleteAll = $(this).closest('main').find('#deleteAll');
        let HinhThanhAll = $(this).closest('main').find('#HinhThanhAll');
        let GuiDuyetAll = $(this).closest('main').find('#GuiDuyetAll');
        let HuyHinhThanhAll = $(this).closest('main').find('#HuyHinhThanhAll');
        let DeleteAllThanhPhan = $(this).closest('#ThanhPhanModal').find('#DeleteAllThanhPhan');

        $(deleteAll).toggleClass('disabled', checkboxChecked.length == 0)
        $(HinhThanhAll).toggleClass('disabled', checkboxChecked.length == 0);
        $(GuiDuyetAll).toggleClass('disabled', checkboxChecked.length == 0);
        $(HuyHinhThanhAll).toggleClass('disabled', checkboxChecked.length == 0);
        $(DeleteAllThanhPhan).toggleClass('disabled', checkboxChecked.length == 0);
    })
    //xử lý các checkbox
    $(document).on('change', '.checkboxs', function () {
        let checkbox = $(this).closest('table').find('.checkboxs');
        let checkboxChecked = $(this).closest('table').find('.checkboxs:checked');
        let checkAll = $(this).closest('table').find('#checkAll');
        state = (checkbox.length == checkboxChecked.length);
        checkAll.prop('checked', state);
        checkAll.toggleClass('checkAll', state);

        let deleteAll = $(this).closest('main').find('#deleteAll');
        let HinhThanhAll = $(this).closest('main').find('#HinhThanhAll');
        let HuyHinhThanhAll = $(this).closest('main').find('#HuyHinhThanhAll');
        let GuiDuyetAll = $(this).closest('main').find('#GuiDuyetAll');
        let DeleteAllThanhPhan = $(this).closest('#ThanhPhanModal').find('#DeleteAllThanhPhan');

        $(deleteAll).toggleClass('disabled', checkboxChecked.length == 0);
        $(HinhThanhAll).toggleClass('disabled', checkboxChecked.length == 0);
        $(GuiDuyetAll).toggleClass('disabled', checkboxChecked.length == 0);
        $(HuyHinhThanhAll).toggleClass('disabled', checkboxChecked.length == 0);
        $(DeleteAllThanhPhan).toggleClass('disabled', checkboxChecked.length == 0);
    })
    $(document).on('click', '#vinhVien', function () {
        $(this).toggleClass('internal');
        state = $(this).hasClass('internal');
        if (state) {
            $(this).html("Năm");
            $('#ThoiHanBaoQuan').prop('disabled', state);
            $('#ThoiHanBaoQuan').val("Vĩnh viễn", state);
        }
        else {
            $(this).html("Vĩnh Viễn");
            $('#ThoiHanBaoQuan').prop('disabled', state);
            $('#ThoiHanBaoQuan').val("", state);
        }
    })

    $(document).on('change', '#MaDanhMuc', function () {
        danhMuc = $(this).val();
        maHoSo = danhMuc + "." + kho + "." + ngan + "." + kyHieu;
        $('#MaHoSo').val(maHoSo);
    })
    $(document).on('change', '#MaKho', function () {
        kho = $(this).val();
        maHoSo = danhMuc + "." + kho + "." + ngan + "." + kyHieu;
        $('#MaHoSo').val(maHoSo);
        GetNgan(kho)
    })
    $(document).on('change', '#MaNgan', function () {
        ngan = $(this).val();
        maHoSo = danhMuc + "." + kho + "." + ngan + "." + kyHieu;
        $('#MaHoSo').val(maHoSo);
    })
    $(document).on('keyup', '#KyHieu', function () {
        kyHieu = $(this).val();
        maHoSo = danhMuc + "." + kho + "." + ngan + "." + kyHieu;
        $('#MaHoSo').val(maHoSo);
    })

})

function SearchUser(state) {
    let url = '';
    let keyword = $(`#searchInput_${state}`).val();
    let page = $('.pagination').find('a').html();
    if (state == 0) {
        url = "/admin/HoSoLT/search/"
        HoSoState = 0;
    }
    else {
        url = "/admin/HoSoHT/search/"
        HoSoState = 1;
    }

    $.ajax({
        url: url,
        data: {
            keyword: keyword,
            page: page,
        },
        type: "get",
        success: function (res) {
            let table = '';
            if (state == 0) {
                table = $("#HoSoLTTable");
            } else {
                table = $('#HoSoHTTable');
            }
            $(table).html(res);
        }
    })
}
function GetNgan(MaKho) {
    let html = '';

    $.ajax({
        url: "/admin/hosolt/GetNgan",
        data: {
            MaKho: MaKho,
        },
        type: 'get',
        success: function (res) {
            res.data.forEach((val, index) => {
                html += `<option value="${val.MaNgan}">${val.TenNgan} </option>`;
            })
            if (res.data.length == 0) {
                $('#MaNgan').html(html)
            }
            else {
                $('#MaNgan').html(html).change();
            }
            $('#KyHieu').keyup();
        }
    })
}

//ID muốn thay đổi và state muốn thay đổi
function ChangeState(ID, state) {
    if (confirm("Bạn có muốn thay đổi trạng thái hồ sơ không?")) {
        $.ajax({
            url: "/admin/hosolt/Change",
            data: {
                ID: ID,
                state: state
            },
            type: "get",
            dataType: "json",
            success: function (res) {
                if (state == 1 || state == 100) {
                    SearchUser(0);
                }
                else {
                    SearchUser(1);
                }
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
function DongBang(ID, state) {
    if (confirm("Bạn có muốn thay đổi trạng thái băng hồ sơ không?")) {
        $.ajax({
            url: "/admin/hosolt/Change",
            data: { ID: ID, state: state },
            type: "get",
            dataType: "json",
            success: function (res) {
                SearchUser(state);
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

function EnableForm() {
    $('#userForm :input').each(function () {
        $('label[class="error"]').remove();
        $(this).val("");
        $(this).attr("disabled", false);
        $(`button[type="submit"]`).attr("disabled", false);
    });
}
function DisableForm() {
    $('label[class="error"]').remove();
    $('#userForm :input').each(function () {
        $(this).attr("disabled", true);
    });
    $(`button[type="submit"]`).attr("disabled", true);
}
function ViewUser(ID, event) {
    GetData(ID, event, 0);
    DisableForm();
}
function UpdateUser(ID, event) {
    EnableForm();
    GetData(ID, event, 1);
}
function GetData(ID, event, state) {
    event.preventDefault();
    $.ajax({
        url: "/admin/hosolt/View",
        dataType: "json",
        data: { ID: ID },
        type: "get",
        success: function (res) {
            console.log(res);
            $("#HiddenID").val(res.data.ID);
            $("#TieuDe").val(res.data.TieuDe);
            $("#MaDanhMuc").val(res.data.MaDanhMuc);
            $("#MaKho").val(res.data.MaKho);
            $("#MaHoSo").val(res.data.MaHoSo);
            $("#MaNgan").val(res.data.MaNgan);
            $("#KyHieu").val(res.data.KyHieu);
            $("#MaLoaiHoSo").val(res.data.MaLoaiHoSo);
            $("#MoTa").val(res.data.MoTa);
            $("#ThoiGianLuuTru").val(reformatDateString(res.data.ThoiGianLuuTru));

            $('#staticBackdrop').modal('toggle');
            if (res.data.AnhBia != null) {
                $("#imgTemp").hide();
                $("#userImg").show();
                $("#userImg").attr("src", res.data.AnhBia);
            } else {
                $("#imgTemp").show();
                $("#userImg").hide();
            }
            let THBQ = $("#ThoiHanBaoQuan");
            let check = res.data.ThoiHanBaoQuan == 0 || res.data.ThoiHanBaoQuan == null;
            let vinhVien = $('#vinhVien');
            if (check) {
                $('#vinhVien').addClass('internal');
                vinhVien.html("Năm");
                THBQ.prop('disabled', check);
                THBQ.val("Vĩnh viễn", check);
            } else {
                $('#vinhVien').removeClass('internal');
                vinhVien.html("Vĩnh viễn");
                THBQ.prop('disabled', check);
                THBQ.val(res.data.ThoiHanBaoQuan);
            }
            if (state == 0) {
                $(THBQ).prop('disabled', true)
            }
        }
    })
}
function CreateUser() {
    var formData = new FormData($('form#userForm')[0]);
    var userID = $("#HiddenID").val();
    formData.append("ID", userID);
    $.ajax({
        url: "/admin/hosolt/save",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'post',
        dataType: 'json',
        success: function (res) {
            $('#staticBackdrop').modal('toggle');
            $.toast({
                heading: res.heading,
                icon: res.icon,
                text: res.message,
                position: 'top-right',
                stack: 10,
                hideAfter: 7000,
                showHideTransition: 'slide',
            })
            SearchUser(0);
        }
    })
}
function DeleteUser(ID, event) {
    if (confirm("Bạn có chắc chắn xóa người dùng này không?")) {
        event.preventDefault();
        $.ajax({
            url: "/admin/hosolt/delete",
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
                SearchUser(0);
            }
        })
    }
}
/*state =0 -> lưu trữ
state = 1 -> hình thành
state = 2 -> gửi duyệt
state = 10 -> xóa
state =100  -> đóng băng
*/
//thay đổi trạng thái của nhiều bản ghi
function SetStateAll(state) {
    let arr = [];
    let checkboxs = $(".checkboxs:checked");
    $.each(checkboxs, function (index, value) {
        arr.push($(this).data('id'))
    })
    if (confirm(`Bạn thực sự muốn thay đổi trạng thái ${arr.length} bản ghi ?`)) {
        $.ajax({
            type: "get",
            url: "/admin/hosolt/DeleteAll",
            data: $.param({ checkboxs: arr, state: state }, true),
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
                //nếu đóng băng hoặc đang hình thành
                if (state == 1 || state == 100 || state ==10) {
                    SearchUser(0);
                }
                else {
                    SearchUser(1);
                }
                $('#deleteAll').toggleClass('disabled');
                $('#HinhThanhAll').toggleClass('disabled');
                $('#GuiDuyetAll').toggleClass('disabled');
                $('#HuyHinhThanhAll').toggleClass('disabled');
                $('#DeleteAllThanhPhan').toggleClass('disabled');
            }
        })
    }
}
function loadFile(event) {
    var userImg = document.getElementById('userImg');
    userImg.src = URL.createObjectURL(event.target.files[0]);
    userImg.onload = function () {
        URL.revokeObjectURL(userImg.src); // free memory
    };
    $('#imgTemp').hide();
    $('#userImg').show();
};
function reformatDateString(mydate) {
    var date = new Date(mydate);
    return dateString = new Date(date.getTime() - (date.getTimezoneOffset() * 60000))
        .toISOString()
        .split("T")[0];
}
function ModalExcel() {
    var fileUpload = $("#excelUpload").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();
    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }
    $.ajax({
        url: "/admin/hosoLT/ExcelModal",
        type: "post",
        contentType: false,
        processData: false,
        data: fileData,
        success: function (res) {
            var table = $("#excelContainer");
            table.html(res);
            $("#ExcelModal").modal('toggle');
        },
        fail: function (res) {
            console.log(res)
        }
    })
}

function UpExcel() {
    $.ajax({
        url: "/admin/HoSoLT/Excel",
        type: 'get',
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
            $("#ExcelModal").modal('toggle');
            SearchUser(0);
        }
    });
}
function ShowTr(ID, nameTable) {
    $(`table#${nameTable} tr[id^="tr_"]`).hide();
    $(`table#${nameTable} tr[id^="tr_${ID}"]`).css('display', 'table-row')
}
function HideTr(nameTable) {
    $(`table#${nameTable} tr[id^="tr_"]`).hide();
}
