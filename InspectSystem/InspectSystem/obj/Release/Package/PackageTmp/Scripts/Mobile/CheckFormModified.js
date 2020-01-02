// Global variable for check the form has been modified or not.
var hasChanged = false;

$(document).ready(function () {

    // When the form has been modified, set value to "changed".
    $(document).on("change", "#detailsForm", function () {
        hasChanged = true;
    });

    // Show alert before change page.
    window.onbeforeunload = function (event) {
        if (hasChanged == true) {
            event.returnValue = "有修改的資料尚未儲存，是否離開此頁面?";
        }
    };

    // When form submit, system will just save and change page, tempSaveBtn is in the ClassContentOfArea Views.
    $(document).on("submit", "#detailsForm", function () {
        window.onbeforeunload = null;
    });

});