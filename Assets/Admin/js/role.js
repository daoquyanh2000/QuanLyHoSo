$(function () {
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
            "TenKieu": {
                required: true,
                maxlength: 20,
                minlength: 3,
            },
            "MaKieu": {
                required: true,
                maxlength: 20,
                minlength: 2,
            },
            "TrangThai": {
                required: true,
            },
        },
        messages: {
            "TenKieu": {
                required: "Bắt buộc nhập tên kiểu người dùng",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 3 ký tự"
            },
            "TrangThai": {
                required: "Bắt buộc nhập trạng thái",
            },
            "MaKieu": {
                required: "Bắt buộc nhập mã kiểu người dùng",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 2 ký tự"
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

function CreateUser() {
    var formData = new FormData($('form#userForm')[0]);
    var userID = $("#HiddenID").val();
    formData.append("ID", userID);
    let checkedID = [];
    let checked = $("form input:checked");
    $.each(checked, function () {
        let tmp = $(this).attr("data-id")
        checkedID.push(tmp);
    })
    formData.append("checkedID", checkedID);
    $.ajax({
        url: "/admin/role/save",
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

function GetData(ID, event) {
    $('form input[type="checkbox"]').prop('checked', false);
    event.preventDefault();
    $.ajax({
        url: "/admin/role/View",
        dataType: "json",
        data: { ID: ID },
        type: "get",
        success: function (res) {
            $("#HiddenID").val(res.data.ID);
            $("#TenKieu").val(res.data.TenKieu);
            $("#MaKieu").val(res.data.MaKieu);
            $("#TrangThai").val(res.data.TrangThai);
            $("#ChuThich").val(res.data.ChuThich);
            $(res.checkbox).each(function (index, value) {
                $(`#checkbox_${value}`).prop('checked', true);
            })
            console.log(res.checkbox)
            $('#staticBackdrop').modal('toggle');
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
    GetData(ID, event);
    DisableForm();
}

function DeleteUser(ID, event) {
    var tr = document.querySelector(`tr[data-id="${ID}"]`)
    if (confirm("Bạn có chắc chắn xóa người dùng này không?")) {
        event.preventDefault();
        tr.remove();
        $.ajax({
            url: "/admin/role/delete",
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
            }
        })
    }
}
function UpdateUser(ID, event) {
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
            url: "/admin/role/Change",
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
    var fileUpload = $("#excelUpload").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();
    for (var i = 0; i < files.length; i++) {
        fileData.append(files[i].name, files[i]);
    }
    $.ajax({
        url: '/admin/user/Excel',
        type: "POST",
        contentType: false,
        processData: false,
        data: fileData,
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
    });
}

function SearchUser() {
    let keyword = $("#searchInput").val();
    let page = $('.active').find('a').html()
    $.ajax({
        url: "/admin/role/search/",
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
            url: "/admin/role/DeleteAll",
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