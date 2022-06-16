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
/*        rules: {
            "Ten": {
                required: true,
                maxlength: 20,
                minlength: 3,
            },
            "TrangThai": {
                required: true,
            },
        },
        messages: {
            "TenKieu": {
                required: "Bắt buộc nhập kiểu người dùng",
                maxlength: "Hãy nhập tối đa 20 ký tự",
                minlength: "Hãy nhập ít nhất 3 ký tự"
            },
            "TrangThai": {
                required: "Bắt buộc nhập trạng thái",
            },
        },*/
        submitHandler: function () {
            CreateUser();
        }
    });
    //create user
})

function CreateUser() {
    var formData = new FormData($('form#userForm')[0]);
    var userID = $("#HiddenID").val();
    formData.append("ID", userID);
    $.ajax({
        url: "/admin/storage/save",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'post',
        dataType: 'json',
        success: function (res) {
            $('.modal').modal('toggle');
            $.toast({
                heading: res.heading,
                icon: res.icon,
                text: res.message,
                position: 'top-right',
                stack: false,
                hideAfter: 3000,
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
            $('.modal').modal('toggle');
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
                    hideAfter: 3000,
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
                    hideAfter: 3000,
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
                hideAfter: 3000,
                showHideTransition: 'slide',
            })
            SearchUser();
        }
    });
}

function SearchUser() {
    let value = $("#searchInput").val();
    $.ajax({
        url: "/admin/role/search?keyword=" + value,
        type: "get",
        success: function (res) {
            var table = $("#tableContainer");
            table.html(res);
        }
    })
}