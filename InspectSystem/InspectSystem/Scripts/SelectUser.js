
$(function () {
    $("#btnQuname").click(function () {
        var keynam = $("#Susername").val();
        if (keynam == "") {
            return false;
        }
        else {
            $('#imgLOADING').show();
            $.ajax({
                contentType: "application/json; charset=utf-8",
                url: '../../AppUsers/GetUsersByKeyname',
                type: "GET",
                data: { keyname: keynam },
                dataType: "json",
                success: function (data) {
                    var jsdata = JSON.parse(data);
                    var appenddata;
                    appenddata += "<option value = ''>請選擇</option>";
                    $.each(jsdata, function (key, value) {
                        appenddata += "<option value = '" + value.uid + "'>" + value.uname + " </option>";
                    });
                    $('#Suserid').html(appenddata);
                    $('#imgLOADING').hide();
                },
                error: function (msg) {
                    $('#imgLOADING').hide();
                    alert(msg);
                }
            });
        }
    });

    $("#btnQdname").click(function () {
        var keynam = $("#Sdptname").val();
        if (keynam == "") {
            return false;
        }
        else {
            $('#imgLOADING').show();
            $.ajax({
                contentType: "application/json; charset=utf-8",
                url: '../../AppUsers/GetUsersInDptByKeyname',
                type: "GET",
                data: { keyname: keynam },
                dataType: "json",
                success: function (data) {
                    var jsdata = JSON.parse(data);
                    var appenddata;
                    appenddata += "<option value = ''>請選擇</option>";
                    $.each(jsdata, function (key, value) {
                        appenddata += "<option value = '" + value.uid + "'>" + value.uname + " </option>";
                    });
                    $('#Suserid').html(appenddata);
                    $('#imgLOADING').hide();
                },
                error: function (msg) {
                    $('#imgLOADING').hide();
                    alert(msg);
                }
            });
        }
    });
})