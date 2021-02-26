
$(document).ready(function () {

    /* Check the min and max value, when user inputs. */
    $(".inputValue").change(function () {

        var id = $(this).attr("id");
        var value = $(this).val();
        var DocId = document.getElementById("DEInspectDocDetailTemp[" + id + "].DocId").value;
        var CycleId = document.getElementById("DEInspectDocDetailTemp[" + id + "].CycleId").value;
        var ClassId = document.getElementById("DEInspectDocDetailTemp[" + id + "].ClassId").value;
        var ItemId = document.getElementById("DEInspectDocDetailTemp[" + id + "].ItemId").value;
        var FieldId = document.getElementById("DEInspectDocDetailTemp[" + id + "].FieldId").value;

        var countFields = document.getElementById("countFields").value;
        for (var i = parseInt(id); i < countFields; i++) {
            if (ItemId == document.getElementById("DEInspectDocDetailTemp[" + i + "].ItemId").value) {
                var radios = document.getElementsByName("DEInspectDocDetailTemp[" + i + "].IsFunctional");
                var targetId = "DEInspectDocDetailTemp[" + i + "].ErrorDescription";
                if (radios.length == 2) {
                    break;
                }
            }
        }      

        $.ajax({
            type: "GET",
            url: urlCheckValue,
            data: { docId: DocId, cycleId: CycleId, classId: ClassId, itemId: ItemId, fieldId: FieldId, Value: value },
            success: function (result) {
                $("." + id).html(result);
                if (result != "") {
                    for (var j = 0; j < radios.length; j++) {
                        if (radios[j].value == "N") {
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
