


function CreateUser() {
	var UserData = {
		HoTen: $("#HoTen").val(),
		Username: $("#Username").val(),
		Password: $("#Password").val(),
		SDT: $("#SDT").val(),
		Email: $("#Email").val(),
		Quyen: $("#Quyen").val(),
		TrangThai: $("#TrangThai").val(),
		AnhDaiDien: $("#AnhDaiDien").val(),
		GioiTinh: $("#GioiTinh ").val(),
		NgaySinh: $("#NgaySinh").val(),
		DiaChi: $("#DiaChi").val(),
		QueQuan: $("#QueQuan").val(),
		TieuSu: $("#TieuSu").val(),
		CongTy: $("#CongTy").val(),
		ChucVu: $("#ChucVu").val(),

	}
	$.ajax({
		url: "/Admin/User/Index",
		type: "POST",
		dataType: "json",
		data: { jsonObj: JSON.stringify(UserData) },
		success: function (res) {
			let html = `<tr>
			<td>${res.ID}</td>
			<td>${res.HoTen}</td>
			<td>${res.UserName}</td>
			<td>${res.Quyen}</td>
			<td>
				<a href="#">Xem </a>|
				<a href="#">Sửa </a>|
				<a href="#">Xóa </a>
			</td>
		</tr>`
			$(".modal").modal('hide');
			$("#table-body").append(html);
		}
	})
}


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




