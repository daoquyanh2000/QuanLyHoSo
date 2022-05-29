/*
var HoTen = $("#HoTen").val();
var Username = $("#Username").val();
var Password = $("#Password").val();
var SDT = $("#SDT").val();
var Email = $("#Email").val();
var Quyen = $("#Quyen").val();
var TrangThai = $("#TrangThai").val();
var AnhDaiDien = $("#AnhDaiDien").val()
var GioiTinh = $("#GioiTinh").val();
var NgaySinh = $("#NgaySinh").val();
var DiaChi = $("#DiaChi").val();
var QueQuan = $("#QueQuan").val();
var TieuSu = $("#TieuSu").val();
var CongTy = $("#CongTy").val();
var ChucVu = $("#ChucVu").val();*/

function createUser() {
	$.ajax({
		url: 'user/create',
		type: 'post',
		dataType: 'json',
		success: function (res) {
			console.log(res);
        }
    })
}

$(document).ready(function () {
	
});

//xu ly anh trong khi them thanh vien
$('#output').hide();
var loadFile = function (event) {
	var output = document.getElementById('output');
	output.src = URL.createObjectURL(event.target.files[0]);
	output.onload = function () {
		URL.revokeObjectURL(output.src); // free memory
	};
	$('#imgTemp').hide();
	$('#output').show();
};


