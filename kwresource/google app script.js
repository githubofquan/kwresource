function onOpen() {

    var ss = SpreadsheetApp.getActiveSpreadsheet();
    var menuEntries = [];
    menuEntries.push({ name: "Gửi dữ liệu", functionName: "sendForm" });
    menuEntries.push({ name: "Lấy dữ liệu", functionName: "getData" });
    ss.addMenu("AutoTool", menuEntries);
}

function sendForm() {

    var field1 = "quan.ton@eduu.vn";
    var field2 = "mailofquan@gmail.com";

    var payload = {
        "field1": field1,
        "field2": field2,
    };

    var data = {
        "method": "post",
        "payload": payload
    };

    var myWebsite = "http://kwresource.magica.top/Home/GetForm";
    var response = UrlFetchApp.fetch(myWebsite, data);
    Browser.msgBox(response);

}

function getData() {
    return;
}

function getEditDistance(a, b) {
    if (a.length === 0) return b.length;
    if (b.length === 0) return a.length;

    var matrix = [];

    // increment along the first column of each row
    var i;
    for (i = 0; i <= b.length; i++) {
        matrix[i] = [i];
    }

    // increment each column in the first row
    var j;
    for (j = 0; j <= a.length; j++) {
        matrix[0][j] = j;
    }

    // Fill in the rest of the matrix
    for (i = 1; i <= b.length; i++) {
        for (j = 1; j <= a.length; j++) {
            if (b.charAt(i - 1) == a.charAt(j - 1)) {
                matrix[i][j] = matrix[i - 1][j - 1];
            } else {
                matrix[i][j] = Math.min(matrix[i - 1][j - 1] + 1, // substitution
                    Math.min(matrix[i][j - 1] + 1, // insertion
                        matrix[i - 1][j] + 1)); // deletion
            }
        }
    }

    return matrix[b.length][a.length];
};