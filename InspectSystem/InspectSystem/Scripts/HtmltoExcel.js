function exportExcel(tableId) {
    var timer = null;
    function getExplorer() {
        var explorer = window.navigator.userAgent;
        //ie
        if (explorer.indexOf("MSIE") >= 0) {
            return 'ie';
        }
        //firefox
        else if (explorer.indexOf("Firefox") >= 0) {
            return 'Firefox';
        }
        //Chrome
        else if (explorer.indexOf("Chrome") >= 0) {
            return 'Chrome';
        }
        //Opera
        else if (explorer.indexOf("Opera") >= 0) {
            return 'Opera';
        }
        //Safari
        else if (explorer.indexOf("Safari") >= 0) {
            return 'Safari';
        }
    }

    function clearUp() {
        window.clearInterval(timer);
        CollectGarbage();
    }

    var tableToExcel = (function () {
        var uri = 'data:application/vnd.ms-excel;base64,';
        var template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><meta http-equiv="Content-Type" charset="utf-8"><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>';
        function base64(s) {
            return window.btoa(unescape(encodeURIComponent(s)));
        }
        function format(s, c) {
            return s.replace(/{(\w+)}/g, function (m, p) {
                return c[p];
            });
        }
        return function (table, name) {
            if (!table.nodeType) {
                table = document.getElementById(table);
            }
            var ctx = {
                worksheet: name || 'worksheet',
                table: table.innerHTML
            };
            window.location.href = uri + base64(format(template, ctx));
        }
    })()

    function getExcel(tableId) {
        //整个表格拷贝到EXCEL中
        if (getExplorer() === 'ie') {
            //建立AX物件excel
            var currentTB = document.getElementById(tableId);
            var oXL = new ActiveXObject('Excel.Application');
            //獲取workbook物件
            var oWB = oXL.Workbooks.Add();
            //啟用當前sheet
            var xlsheet = oWB.Worksheets(1);
            //把表格中的內容移到TextRange中
            var sel = document.body.createTextRange();
            sel.moveToElementText(currentTB);
            //全選TextRange中內容
            sel.select;
            //複製TextRange中內容
            sel.execCommand('Copy');
            //貼上到活動的EXCEL中
            xlsheet.Paste();
            //設定excel可見屬性
            oXL.Visible = true;

            try {
                var fname = oXL.Application.GetSaveAsFilename('download.xls', 'Excel Spreadsheets (*.xls), *.xls');
            } catch (e) {
                print('Nested catch caught ' + e);
                alert("請確認:\n1.Microsoft Excel已被安裝.\n2.工具 => Internet 選項=> 安全 => 設置 \"啟用不安全的 ActiveX\"");
            } finally {
                oWB.SaveAs(fname);

                oWB.Close(savechanges = false);
                //xls.visible = false;
                oXL.Quit();
                oXL = null;
                //结束excel进程，退出完成
                //window.setInterval("Cleanup();",1);
                timer = window.setInterval('cleanup();', 1);

            }

        }
        else {
            tableToExcel(tableId);
        }

    }
    getExcel(tableId);
}