// Global variable for check the form has been modified or not.
var hasChanged = false;

$(document).ready(function () {

    //Default class content is first item from tablinks.
    var tabs = document.getElementsByClassName("tablinks");
    tabs.item(0).click();

    // Show alert before change page.
    window.onbeforeunload = function (event) {
        event.returnValue = "是否離開此頁面?";
    };

    // When the form has been modified, set value to "changed".
    $(document).on("change", "#detailsForm", function () {
        hasChanged = true;
    });

    // When form submit, system will just save and change page, tempSaveBtn is in the ClassContentOfArea Views.
    $(document).on("submit", "#detailsForm", function () {
        window.onbeforeunload = null;
    });

});

function openClassContent(evt, acid, docID) {

    // Declare all variables
    var i, tablinks;

    // If form is modified and user want to change page, alert user.
    if (hasChanged == true) {

        if (confirm("有資料尚未儲存，是否離開?")) {

            // Get all elements with class="tablinks" and remove the class "active"
            tablinks = document.getElementsByClassName("tablinks");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active", "");
            }

            // Add an "active" class to the link button clicked.
            evt.currentTarget.className += " active";

            $.ajax({
                type: "GET",
                url: "/InspectDocEdit/ClassContentOfArea",
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
            hasChanged = false;
        }
    }
    else {

        // Get all elements with class="tablinks" and remove the class "active"
        tablinks = document.getElementsByClassName("tablinks");
        for (i = 0; i < tablinks.length; i++) {
            tablinks[i].className = tablinks[i].className.replace(" active", "");
        }

        // Add an "active" class to the link button clicked.
        evt.currentTarget.className += " active";

        $.ajax({
            type: "GET",
            url: "/InspectDocEdit/ClassContentOfArea",
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
        hasChanged = false;
    }
}

function getFlowList(evt, docID) {
    // Declare all variables
    var i, tablinks;


    if (hasChanged == true) {

        if (confirm("有資料尚未儲存，是否離開?")) {
            // Get all elements with class="tablinks" and remove the class "active"
            tablinks = document.getElementsByClassName("tablinks");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active", "");
            }

            // Add an "active" class to the link button clicked.
            evt.currentTarget.className += " active";

            $.ajax({
                type: "GET",
                url: "/InspectDocChecker/GetFlowList",
                data: { DocID: docID },
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
            hasChanged = false;
        }
    }
    else {
        // Get all elements with class="tablinks" and remove the class "active"
        tablinks = document.getElementsByClassName("tablinks");
        for (i = 0; i < tablinks.length; i++) {
            tablinks[i].className = tablinks[i].className.replace(" active", "");
        }

        // Add an "active" class to the link button clicked.
        evt.currentTarget.className += " active";

        $.ajax({
            type: "GET",
            url: "/InspectDocChecker/GetFlowList",
            data: { DocID: docID },
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
        hasChanged = false;
    }
}
