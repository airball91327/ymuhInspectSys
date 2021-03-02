using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    public class DEInspectDocQryVModel
    {

        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "區域")]
        public string AreaId { get; set; }
        [Display(Name = "週期")]
        public string CycleId { get; set; }
        [Display(Name = "類別")]
        public string ClassId { get; set; }
        [Display(Name = "流程狀態")]
        public string FlowStatus { get; set; }
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