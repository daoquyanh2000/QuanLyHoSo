//get all the data from modal
var inputName=$("#inputName").val()
var inputUserName = $("#inputUserName").val()
var inputPassword = $("#inputPassword").val()
var inputPhone = $("#inputPhone").val()
var inputRole = $("#inputRole").val()
var inputActive = $("#inputActive").val()
var inputImage = $("#inputImage").val()
var inputGender = $("#inputGender").val()
var inputDate = $("#inputDate").val()
var inputAddress = $("#inputAddress").val()
var inputIntro = $("#inputIntro").val()
var inputCompany = $("#inputCompany").val()
var InputCompanyRole = $("#InputCompanyRole").val()


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



function newUser() {

}