$(function () {
    $('a[href="#collapseExample"]').attr("aria-expanded", true)
    $("#collapseExample").addClass('show');
    ////khi bam nut tao user moi
    $('button[data-bs-toggle="modal"]').click(function () {
        EnableForm();

        $("#HiddenID").val(0);
        $("#userImg").hide();
        $("#imgTemp").show();
    })
    //
    //xu ly anh trong khi them thanh vien
    $('#userImg').hide();

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
    //hide error message when keydown

    // form validation
    $("#userForm").validate({
        rules: {
            "HoTen": {
                required: true,
                maxlength: 20,
                minlength: 6,
            },
            "UserName": {
                required: true,
                maxlength: 20,
                minlength: 6,
            },
            "Password": {
                required: true,
                maxlength: 20,
                minlength: 6,
            },
            "SDT": {
                required: true,
                phoneUS: true,
            },
            "Email": {
                required: true,
                email: true,
            },
            "Quyen": {
                required: true,
            },
            "TrangThai": {
                required: true,
            },
        },
        messages: {
            "HoTen": {
                required: "Bắt buộc nhập họ tên",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 6 ký tự"
            },
            "UserName": {
                required: "Bắt buộc nhập tên tài khoản",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 6 ký tự"
            },
            "Password": {
                required: "Bắt buộc nhập password",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 6 ký tự"
            },
            "SDT": {
                required: "Bắt buộc nhập số điện thoại",
                maxlength: "Hãy nhập tối đa 10 ký tự",
                minlength: "Hãy nhập ít nhất 8 ký tự"
            },
            "Email": {
                required: "Bắt buộc nhập email",
                email: "Hãy nhập đúng định dạng email"
            },
            "Quyen": {
                required: "Bắt buộc nhập kiểu người dùng",
            },
            "TrangThai": {
                required: "Bắt buộc nhập trạng thái",
            },
        },
        submitHandler: function () {
            CreateUser();
        }
    });
    jQuery.validator.addMethod("phoneUS", function (phone_number, element) {
        phone_number = phone_number.replace(/\s+/g, "");
        return this.optional(element) || phone_number.length == 10 &&
            phone_number.match(/[0-9\-\(\)\s]+/);
    }, "Hãy nhập đúng định dạng điện thoại");

    $(document).on('change', '#checkAll', function () {
        console.log(this);
        $(this).toggleClass('checkAll');
        let checkbox = $(this).closest('table').find('.checkboxs');
        checkbox.prop('checked', $(this).hasClass('checkAll'))
        let checkboxChecked = $(this).closest('table').find('.checkboxs:checked');

        let deleteAll = $(this).closest('main').find('#deleteAll');
        console.log(deleteAll);
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

function CreateUser() {
    {
        var formData = new FormData($('form#userForm')[0]);

        var userID = $("#HiddenID").val();
        formData.append("ID", userID);
        // formData.append("AnhDaiDien", "");
        $.ajax({
            url: "/admin/user/save",
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
}

function reformatDateString(mydate) {
    var date = new Date(mydate);
    return dateString = new Date(date.getTime() - (date.getTimezoneOffset() * 60000))
        .toISOString()
        .split("T")[0];
}
function GetData(ID, event) {
    event.preventDefault();
    $.ajax({
        url: "/admin/user/View",
        dataType: "json",
        data: { ID: ID },
        type: "get",
        success: function (res) {
            var hello = res.data;
            console.log(hello)
            $("#HiddenID").val(res.data.ID);
            $("#HoTen").val(res.data.HoTen);
            $("#UserName").val(res.data.UserName);
            $("#Password").val(res.data.Password);
            $("#MaKieu").val(res.data.MaKieu);
            $("#TrangThai").val(res.data.TrangThai);
            $("#NgaySinh").val(reformatDateString(res.data.NgaySinh));
            $("#SDT").val(res.data.SDT);
            $("#Email").val(res.data.Email);
            $("#GioiTinh").val(res.data.GioiTinh);
            $("#DiaChi").val(res.data.DiaChi);
            $("#QueQuan").val(res.data.QueQuan);
            $("#CongTy").val(res.data.CongTy);
            $("#ChucVu").val(res.data.ChucVu);
            $("#TieuSu").val(res.data.TieuSu);
            $('#staticBackdrop').modal('toggle');
            if (res.data.AnhDaiDien != "") {
                $("#imgTemp").hide();
                $("#userImg").show();
                $("#userImg").attr("src", res.data.AnhDaiDien);
            } else {
                $("#imgTemp").show();
                $("#userImg").hide();
            }
        }
    })
}
function DisableForm() {
    $('#userForm :input').each(function () {
        $(this).attr("disabled", true);
    });
    $(`button[type="submit"]`).attr("disabled", true);
}
function ViewUser(ID, event) {
    $("#hiddenAction").val(0);

    GetData(ID, event);
    DisableForm();
}

function DeleteUser(ID, event) {
    if (confirm("Bạn có chắc chắn xóa người dùng này không?")) {
        event.preventDefault();
        $.ajax({
            url: "/admin/user/delete",
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
function UpdateUser(ID, event) {
    $("#hiddenAction").val(0);
    GetData(ID, event);
    EnableForm();
}

function EnableForm() {
    $('#userForm :input').each(function () {
        $(this).val("");
        $(this).attr("disabled", false);
        $(`button[type="submit"]`).attr("disabled", false);
    });
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
            url: "/admin/user/Change",
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
        url: "/admin/user/excel",
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
        }
    });
}

function ModalExcel() {
    var fileUpload = $("#excelUpload").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();
    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }
    $.ajax({
        url: "/admin/user/ExcelModal",
        type: "post",
        contentType: false,
        processData: false,
        data: fileData,
        success: function (res) {
            var table = $("#excelContainer");
            table.html(res);
            $("#ExcelModal").modal('toggle');
        }
    })
}
function SearchUser() {
    let keyword = $("#searchInput").val();
    let page = $('.active').find('a').html()
    $.ajax({
        url: "/admin/user/search/",
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
var loadFile = function (event) {
    var userImg = document.getElementById('userImg');
    userImg.src = URL.createObjectURL(event.target.files[0]);
    userImg.onload = function () {
        URL.revokeObjectURL(userImg.src); // free memory
    };
    $('#imgTemp').hide();
    $('#userImg').show();
};

function GetCheckboxAll() {
    let arr = [];
    let checkboxs = $(".checkboxs:checked");
    $.each(checkboxs, function (index, value) {
        arr.push($(this).data('id'))
    })
    if (confirm(`Bạn thực sự muốn xóa ${arr.length} bản ghi ?`)) {
        $.ajax({
            type: "get",
            url: "/admin/user/DeleteAll",
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
function MultiImage() {
    let fileUpload  = $('input#MultiImage').get(0);
    let files = fileUpload.files;
    let formData = new FormData();
    for (let i = 0; i < files.length; i++) {
        formData.append(files[i].name, files[i]);
    }
    $.ajax({
        type: 'POST',
        url: "/admin/user/GetAnhDaiDien",
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
                console.log(res.data)
        },
        error: function (err) {
            console.log(err)
        }
    })

}