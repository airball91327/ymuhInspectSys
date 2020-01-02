$(document).ready(function () {

    $("input:radio").change(function () {

        // Set variables.
        var objName = this.name.toString();
        var cutName = objName.split(".", 1);
        var targetId = cutName + ".ErrorDescription";
        var value = this.value;
        //console.log(value);
        // Controll ErrorDescription textbox.
        if (value == "n") {
            document.getElementById(targetId).setAttribute("Required", "Required");
        }
        else {
            document.getElementById(targetId).removeAttribute("Required");
        }
    });

});