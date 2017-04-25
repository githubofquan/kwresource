$(function () {
    $("#test").click(function () {
        var formData = {};

        formData.field1 = $("#email").val().trim();
        formData.field2 = $("#email2").val().trim();

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

});