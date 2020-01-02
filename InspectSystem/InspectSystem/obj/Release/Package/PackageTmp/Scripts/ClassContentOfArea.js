
$(document).ready(function () {

    /* Check the min and max value, when user inputs. */
    $(".inputValue").change(function () {

        var id = $(this).attr("id");
        var value = $(this).val();
        var areaID = document.getElementById("InspectDocDetailsTemporary[" + id + "].AreaID").value;
        var classID = document.getElementById("InspectDocDetailsTemporary[" + id + "].ClassID").value;
        var itemID = document.getElementById("InspectDocDetailsTemporary[" + id + "].ItemID").value;
        var fieldID = document.getElementById("InspectDocDetailsTemporary[" + id + "].FieldID").value;

        var countFields = document.getElementById("countFields").value;
        //console.log("countFields:" + countFields); //For debug
        for (var i = parseInt(id); i < countFields; i++) {
            //console.log("i:"+ i); //For debug
            //console.log("itemID:" + document.getElementById("InspectDocDetailsTemporary[" + i + "].ItemID").value); //For debug
            //console.log("FieldID:" + document.getElementById("InspectDocDetailsTemporary[" + i + "].FieldID").value); //For debug
            if (itemID == document.getElementById("InspectDocDetailsTemporary[" + i + "].ItemID").value) {
                var radios = document.getElementsByName("InspectDocDetailsTemporary[" + i + "].IsFunctional");
                var targetId = "InspectDocDetailsTemporary[" + i + "].ErrorDescription";
                if (radios.length == 2) {
                    break;
                }
            }
        }      

        $.ajax({
            type: "GET",
            url: "/InspectDocDetails/CheckValue",
            data: { AreaID: areaID, ClassID: classID, ItemID: itemID, FieldID: fieldID, Value: value },
            success: function (result) {
                //console.log(result); //For debug
                //console.log(id); //For debug
                $("." + id).html(result);
                if (result != "") {
                    for (var j = 0; j < radios.length; j++) {
                        if (radios[j].value == "n") {
                            //console.log("自動切換至不正常"); //For debug
                            radios[j].checked = true;
                            document.getElementById(targetId).setAttribute("Required", "Required");
                            break;
                        }
                    }
                }
            },
            error: function (msg) {
                alert("讀取錯誤");
            }
        });
    });
});
