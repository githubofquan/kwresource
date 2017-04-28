$(function () {
    $("#test").click(function () {
        $("#test").hide();
        var sessionid = generateUUID();

        sendTest("mua ban xe tai cu", "https://izifix.com/duong-bo/mua-ban-xe-tai", "2900", "100", "20", "1000000", sessionid, "1");
        sendTest("mua xe tai cu", "https://izifix.com/duong-bo/mua-ban-xe-tai", "2400", "100", "20", "2000000", sessionid, "2");
        sendTest("mua ban xe tai", "https://izifix.com/duong-bo/mua-ban-xe-tai", "720", "100", "20", "3000000", sessionid, "3");
        sendTest("mua xe tải cũ", "https://izifix.com/duong-bo/mua-ban-xe-tai", "720", "100", "20", "4000000", sessionid, "4");
        sendTest("mua bán xe tải cũ", "https://izifix.com/duong-bo/mua-ban-xe-tai", "720", "100", "20", "5000000", sessionid, "5");
        sendTest("xe tai van chuyen", "https://izifix.com/duong-bo/cho-thue-xe-tai", "720", "100", "20", "6000000", sessionid, "6");
        sendTest("xe tải vận chuyển", "https://izifix.com/duong-bo/cho-thue-xe-tai", "720", "100", "20", "7000000", sessionid, "7");

    });

    $("#sentemail").click(function () {
        $("#sentemail").hide();
     
        sendTest("quan.ton@eduu.vn");

    });

    $("#test2").click(function () {
        $("#test2").hide();
        var sessionid = $("#sessionid").val().trim();
        GetResult(sessionid, "1");
        GetResult(sessionid, "2");
        GetResult(sessionid, "3");
    });

    $("#test3").click(function () {
        $("#test3").hide();
        var sessionid = $("#sessionid").val().trim();
        Calculate(sessionid);
    });

});

function generateUUID() {
    var d = new Date().getTime();
    if (window.performance && typeof window.performance.now === "function") {
        d += performance.now();; //use high-precision timer if available
    }
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
}

function sendTest(kw, lp, vl, cr, kd, cost, sessionid, stt) {    
        var formData = {};
        formData.kw = kw;
        formData.lp = lp;
        formData.vl = vl;
        formData.cr = cr;
        formData.kd = kd;
        formData.cost = cost;
        formData.sessionid = sessionid;
        formData.stt = stt;
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
}

function GetResult(sessionid, stt) {
    var formData = {};
    formData.sessionid = sessionid;
    formData.stt = stt;
    $.ajax({
        type: "POST",
        url: "/Home/GetResult",
        data: '{formData: ' + JSON.stringify(formData) + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log(stt + ' : '+response);
        },
        failure: function (response) {
            console.log(stt + ' : ' + response);
        },
        error: function (response) {
            console.log(stt + ' : ' + response);
        }
    });
}

function Calculate(sessionid) {
    var formData = {};
    formData.sessionid = sessionid;
    $.ajax({
        type: "POST",
        url: "/Home/Calculate",
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
}