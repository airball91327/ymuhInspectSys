$(document).ready(function () {

    /* Check the min and max value, when user inputs. */
    $(".inputValue").change(function () {

        var id = $(this).attr("id");
        var value = $(this).val();
        var DocId = document.getElementById("InspectDocDetails[" + id + "].DocId").value;
        var areaID = document.getElementById("InspectDocDetails[" + id + "].AreaId").value;
        var classID = document.getElementById("InspectDocDetails[" + id + "].ClassId").value;
        var ItemId = document.getElementById("InspectDocDetails[" + id + "].ItemId").value;
        var FieldId = document.getElementById("InspectDocDetails[" + id + "].FieldId").value;

        var countFields = document.getElementById("countFields").value;
        //console.log("countFields:" + countFields); //For debug
        for (var i = parseInt(id); i < countFields; i++) {
            //console.log("i:"+ i); //For debug
            //console.log("ItemId:" + document.getElementById("InspectDocDetailsTemporary[" + i + "].ItemId").value); //For debug
            //console.log("FieldId:" + document.getElementById("InspectDocDetailsTemporary[" + i + "].FieldId").value); //For debug
            if (ItemId == document.getElementById("InspectDocDetails[" + i + "].ItemId").value) {
                var radios = document.getElementsByName("InspectDocDetails[" + i + "].IsFunctional");
                var targetId = "InspectDocDetails[" + i + "].ErrorDescription";
                if (radios.length == 2) {
                    break;
                }
            }
        }      

        $.ajax({
            type: "GET",
            url: "../InspectDocEdit/CheckValue",
            data: { DocId: DocId, ClassId: classID, ItemId: ItemId, FieldId: FieldId, Value: value },
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
