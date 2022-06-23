﻿$(document).ready(function () {
    ////khi bam nut tao user moi
    $('button[data-bs-toggle="modal"]').click(function () {
        EnableForm();
        $("#HiddenID").val(0);
    })
    //
    var input = document.getElementById("searchInput");
    // Execute a function when the user presses a key on the keyboard
    input.addEventListener("keypress", function (event) {
        // If the user presses the "Enter" key on the keyboard
        if (event.key === "Enter") {
            // Cancel the default action, if needed
            event.preventDefault();
            // Trigger the button element with a click
            document.getElementById("searchBtn").click();
        }
    });
    //xu ly checkbox kieu nhan vien

    // form validation
    $("#userForm").validate({
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
            CreateUser();
        }
    });
    $(document).on('change', '#checkAll', function () {
        $(this).toggleClass('checkAll');
        let checkbox = $(this).closest('table').find('.checkboxs');
        checkbox.prop('checked', $(this).hasClass('checkAll'))
        let checkboxChecked = $(this).closest('table').find('.checkboxs:checked');

        let deleteAll = $(this).closest('main').find('#deleteAll');
        $(deleteAll).toggleClass('disabled', checkboxChecked.length == 0)
    })
    $(document).on('change', '.checkboxs', function () {
        let checkbox = $(this).closest('table').find('.checkboxs');
        let checkboxChecked = $(this).closest('table').find('.checkboxs:checked');
        let checkAll = $(this).closest('table').find('#checkAll');
        state = (checkbox.length == checkboxChecked.length);
        checkAll.prop('checked', state);
        checkAll.toggleClass('checkAll', state);

        let deleteAll = $(this).closest('main').find('#deleteAll');
        $(deleteAll).toggleClass('disabled', checkboxChecked.length == 0);
    })
})



function DeleteUser(ID, event) {
    if (confirm("Bạn có chắc chắn xóa người dùng này không?")) {
        event.preventDefault();
        $.ajax({
            url: "/admin/stack/delete",
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
                SearchUser();
            }
        })
    }
}
function CreateUser() {
    var formData = new FormData($('form#userForm')[0]);
    var userID = $("#HiddenID").val();
    formData.append("ID", userID);
    $.ajax({
        url: "/admin/stack/save",
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
                stack: false,
                hideAfter: 7000,
            })
            SearchUser();
        }
    })
}
function UpdateUser(ID, event) {
    GetData(ID, event);
    EnableForm();
}
function ChangeState(ID) {
    var checkboxID = $(`#button-${ID}`)
    var nextState = checkboxID.prop('checked')
    if (nextState == true)
        var setState = 1;
    else setState = 0;

    if (confirm("Bạn có muốn thay đổi trạng thái không?")) {
        event.preventDefault();
        /*        tr.remove();*/
        $.ajax({
            url: "/admin/stack/Change",
            data: { ID: ID, state: setState },
            type: "get",
            dataType: "json",
            success: function (res) {
                SearchUser();
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
    else {
        (nextState == true) ? checkboxID.prop('checked', false) : checkboxID.prop('checked', true)
    }
}
function GetData(ID, event) {
    event.preventDefault();
    $.ajax({
        url: "/admin/stack/View",
        dataType: "json",
        data: { ID: ID },
        type: "get",
        success: function (res) {
            $("#HiddenID").val(res.data.ID);
            $("#TenNgan").val(res.data.TenNgan);
            $("#MaNgan").val(res.data.MaNgan);
            $("#IDKho").val(res.data.IDKho);
            $("#KichThuoc").val(res.data.KichThuoc);
            $("#TrangThai").val(res.data.TrangThai);
            $("#MoTa").val(res.data.MoTa);
            $('#staticBackdrop').modal('toggle');
        }
    })
}
function EnableForm() {
    $('#userForm :input').each(function () {
        $(this).val("");
        $(this).attr("disabled", false);
        $(`button[type="submit"]`).attr("disabled", false);
    });
}
function DisableForm() {
    $('#userForm :input').each(function () {
        $(this).attr("disabled", true);
    });
    $(`button[type="submit"]`).attr("disabled", true);
}
function ViewUser(ID, event) {
    GetData(ID, event);
    DisableForm();
}
function SearchUser() {
    let keyword = $("#searchInput").val();
    let page = $('.active').find('a').html()
    $.ajax({
        url: "/admin/stack/search/",
        data: {
            keyword: keyword,
            page: page,
        },
        type: "get",
        success: function (res) {
            var table = $("#tableContainer");
            table.html(res);
        }
    })
}
function GetCheckboxAll() {
    let arr = [];
    let checkboxs = $(".checkboxs:checked");
    $.each(checkboxs, function (index, value) {
        arr.push($(this).data('id'))
    })
    if (confirm(`Bạn thực sự muốn xóa ${arr.length} bản ghi ?`)) {
        $.ajax({
            type: "get",
            url: "/admin/storage/DeleteAll",
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
                SearchUser();
                $('#deleteAll').toggleClass('disabled');
            }
        })
    }
}
function ModalExcel() {
    var fileUpload = $("#excelUpload").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();
    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }
    $.ajax({
        url: "/admin/stack/ExcelModal",
        type: "post",
        contentType: false,
        processData: false,
        data: fileData,
        success: function (res) {
            if (res.error) {
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
                var table = $("#excelContainer");
                table.html(res);
                $("#ExcelModal").modal('toggle');
            }

        }
    })
}
function UpExcel() {
    var formdata = new FormData();
    let checkbox = $(".excelCheckbox");
    let tmp = [];
    $.each(checkbox, function (index, value) {
        if (this.checked) {
            tmp.push(1)
        } else {
            tmp.push(0)
        }
    })
    formdata.append("checkbox", tmp);
    $.ajax({
        url: "/admin/stack/excel",
        data: formdata,
        cache: false,
        contentType: false,
        processData: false,
        type: 'post',
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
            $("#ExcelModal").modal('toggle');
            SearchUser();
        },
        error: function (request, status, error) {
            alert(request.responseText);
            console.log(request);
        }
    });
}