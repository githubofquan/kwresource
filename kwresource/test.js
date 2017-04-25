$(function () {
    var formData = {};

    formData.field1 = "giá trị 1a";
    formData.field2 = "giá trị 2a";

    $.ajax({
        type: "POST",
        url: "/Home/GetForm",
        data: '{formData: ' + JSON.stringify(formData) + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
});