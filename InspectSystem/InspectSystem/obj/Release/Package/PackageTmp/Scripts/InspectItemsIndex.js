$(document).ready(function () {

    /* When a edit button click */
    $(".editBtn").click(function () {
        /* hide edit button */
        $(this).hide();
        /* When Editing, disable other edit buttons */
        $(".editBtn").attr("disabled", true);
        $("#createBtn").attr("disabled", true);
        /* Change the selected row to be edit */
        $(".displayRow" + $(this).attr("id")).hide();
        $(".editRow" + + $(this).attr("id")).show();
    });
    
    $(".fieldBtn").click(function () {

        var number = $(this).attr("value");
        var ACID = $("#hideACIDNo" + number).attr("value");
        var itemID = $("#hideItemIDNo" + number).attr("value");
        var zoneNo = parseInt(ACID) * 100 + parseInt(itemID);

        /* Control all the slide panel */
        $(".fieldPanel").not(".fieldPanelNo" + number).slideUp("slow");
        $(".fieldPanelNo" + number).slideToggle("slow");
        /*Get the search result of fields*/
        $.ajax({
            type: "GET",
            url: "/InspectFields/Search",
            data: { acid: ACID, itemid: itemID },
            success: function (result) {
                //console.log(result); //For debug
                $(".fieldDiv").not("#fieldDivNo" + zoneNo).html("<p></p>");
                $("#fieldDivNo" + zoneNo).html(result);              
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });
    
});