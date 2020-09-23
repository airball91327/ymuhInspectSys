using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    public class InspectDocQryVModel
    {

        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "班別")]
        public string ShiftId { get; set; }
        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? ApplyDate { get; set; }
        [Display(Name = "完成時間")]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }
        [Display(Name = "結案日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CloseDate { get; set; }
        [Display(Name = "巡檢人員ID")]
        public int? EngId { get; set; }
        [Display(Name = "巡檢人員")]
        public string EngName { get; set; }

    }
}