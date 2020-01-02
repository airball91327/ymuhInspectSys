$(document).ready(function () {
    //Default class content is first item from tablinks.
    var tabs = document.getElementsByClassName("tablinks");
    tabs.item(0).click();

});

function openClassContent(evt, acid, docID) {
    // Declare all variables
    var i, tablinks;

    // Get all elements with class="tablinks" and remove the class "active"
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    // Add an "active" class to the link button clicked.
    evt.currentTarget.className += " active";

    $.ajax({
        type: "GET",
        url: "/InspectDocTempView/ClassContentOfArea",
        data: { ACID: acid, DocID: docID },
        beforeSend: function () {
            $("#loadingModal").modal("show");
        },
        complete: function () {
            $("#loadingModal").modal("hide");
        },
        success: function (result) {
            //console.log(result); //For debug
            $("#tabContent").html(result);
        },
        error: function (msg) {
            alert("讀取錯誤");
        }
    });
}
