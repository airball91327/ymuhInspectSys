$(document).ready(function () {

    /* InspectField/Edit and InspectField/Create */
    ChangeAttrByRaioBtn();

    /* Change Attr to disabled when user not selected "float" type. */
    $("input[name='DataType']").click(function () {
        ChangeAttrByRaioBtn();
    });

    //// for datatype dropdownlist, and dynamic inset textbox. ////
    $("#addButton").click(function () {
        var currentCount = parseInt($('#TextBoxCount').val(), 10);
        var newCount = currentCount + 1;

        var newColumnDiv =
            $(document.createElement('div')).attr("class", 'col-md-10');

        var newTextBoxDiv =
            $(document.createElement('div')).attr(
                {
                    "id": 'TextBoxScope' + newCount,
                    "class": "form-group"
                });

        newTextBoxDiv.appendTo(newColumnDiv);

        newTextBoxDiv.after().html(
            '<label>選項 #' + newCount + ' : </label>' +
            '<input type="text"' +
            ' name="textbox' + newCount + '" id="textbox' + newCount + '"' +
            ' value="" class="form-control" required>');

        newColumnDiv.appendTo("#TextBoxesGroup");
        $('#TextBoxCount').val(newCount);
        $('#textbox' + newCount).focus();
    });

    $("#removeButton").click(function () {
        var currentCount = parseInt($('#TextBoxCount').val(), 10);
        if (currentCount == 0) {
            alert("已經沒有項目可以移除。");
            return false;
        }
        $("#TextBoxScope" + currentCount).remove();
        var newCount = currentCount - 1;
        $('#TextBoxCount').val(newCount);
    });
    //// for datatype dropdownlist, and dynamic inset textbox. ////

});

function ChangeAttrByRaioBtn() {

    var dataType = document.getElementsByName("DataType");
    for (i = 0; i < dataType.length; i++) {
        if (dataType[i].checked) {
            checkValue = dataType[i].value;
        }
    }

    if (checkValue == "float") {
        $("#UnitOfData").attr("disabled", false);
        $("#MinValue").attr("disabled", false);
        $("#MaxValue").attr("disabled", false);
        $("#FieldDescription").attr("disabled", true);
    }
    else if (checkValue == "description") {
        $("#UnitOfData").attr("disabled", true);
        $("#MinValue").attr("disabled", true);
        $("#MaxValue").attr("disabled", true);
        $("#FieldDescription").attr("disabled", false);
    }
    else {
        $("#UnitOfData").attr("disabled", true); /* If a element set disabled, it won't return values.*/
        $("#MinValue").attr("disabled", true);
        $("#MaxValue").attr("disabled", true);
        $("#FieldDescription").attr("disabled", true);
    }

    /* for datatype dropdownlist. */
    if (checkValue == "dropdownlist") {
        $(".content-group").hide();
        $(".textbox-group").show();
        $("#textbox1").attr("required");
    }
    else {
        $(".content-group").show();
        $(".textbox-group").hide();
        $("#textbox1").removeAttr("required");
    }
}

function loadDropDown(value) {

    var currentCount = parseInt($('#TextBoxCount').val(), 10);
    var newCount = currentCount + 1;

    var newColumnDiv =
        $(document.createElement('div')).attr("class", 'col-md-10');

    var newTextBoxDiv =
        $(document.createElement('div')).attr(
            {
                "id": 'TextBoxScope' + newCount,
                "class": "form-group"
            });

    newTextBoxDiv.appendTo(newColumnDiv);

    newTextBoxDiv.after().html(
        '<label>選項 #' + newCount + ' : </label>' +
        '<input type="text"' +
        ' name="textbox' + newCount + '" id="textbox' + newCount + '"' +
        ' value="' + value + '" class="form-control" required>');

    newColumnDiv.appendTo("#TextBoxesGroup");
    $('#TextBoxCount').val(newCount);
    $('#textbox' + newCount).focus();
};