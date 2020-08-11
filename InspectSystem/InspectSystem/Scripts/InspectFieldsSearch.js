$(document).ready(function () {

    /* InspectFields/Search */
    $(".fieldEditBtn").click(function () {

        var rowNumber = $(this).attr("value");
        var ACID = $("#hideACIDNo" + rowNumber).attr("value");
        var ItemId = $("#hideItemIdNo" + rowNumber).attr("value");
        var FieldId = $("#hideFieldIdNo" + rowNumber).attr("value");
        var zoneNo = parseInt(ACID) * 100 + parseInt(ItemId);
        var editModalNo = "editModalNo" + zoneNo;
        var editModalContentNo = "editModalContentNo" + zoneNo;

        document.getElementById(editModalNo).style.display = "block";

        $.ajax({
            type: "GET",
            url: "../InspectFields/Edit",
            data: { acid: ACID, ItemId: ItemId, FieldId: FieldId },
            success: function (result) {
                //console.log(result); //For debug
                $(".createModalContent").html("<p></p>"); //Clean other create modal's html content to prevent radio button's error.
                $("#" + editModalContentNo).html(result);
            },
            error: function (msg) {
                alert(msg);
            }
        });

    });

    $(".fieldCreateBtn").click(function () {

        var ACID = fieldCreateACID;
        var ItemId = fieldCreateItemId;
        var zoneNo = parseInt(ACID) * 100 + parseInt(ItemId);
        var createModalNo = "createModalNo" + zoneNo;
        var createModalContentNo = "createModalContentNo" + zoneNo;

        document.getElementById(createModalNo).style.display = "block";

        $.ajax({
            type: "GET",
            url: "../InspectFields/Create",
            data: { acid: ACID, ItemId: ItemId },
            success: function (result) {
                //console.log(result); //For debug
                $(".editModalContent").html("<p></p>");  //Clean other edit modal's html content to prevent radio button's error.
                $("#" + createModalContentNo).html(result);
            },
            error: function (msg) {
                alert(msg);
            }
        });
    });

});