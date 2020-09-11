using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class VentilatorQryVModel
    {
        [Display(Name = "表單編號")]
        public int DocId { get; set; }
        [Display(Name = "使用狀態")]
        public int StatusId { get; set; }
        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ApplyDateS { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ApplyDateE { get; set; }
        [Display(Name = "文件狀態")]
        public string DocStatus { get; set; }
    }
}