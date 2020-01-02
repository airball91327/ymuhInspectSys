$(document).ready(function () {

    var value = $("#sendToChecker").val();
    if (value == "true") {
        $("#sendToChecker").removeAttr("disabled");
    }
    else {
        $("#sendToChecker").attr("disabled", true);
    }

});
