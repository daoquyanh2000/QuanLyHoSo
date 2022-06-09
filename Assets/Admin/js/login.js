function Login() {
    var formData = new FormData($('form#loginForm')[0]);
    $.ajax({
        url: "/admin/login/index",
        data: formData,
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
                hideAfter: 5000,
                showHideTransition: 'slide',
            })
            if (res.status == true) {
                window.location = '/Admin/Home';
            }
        }
    })
}