using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    public class DEInspectDocVModel
    {
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "巡檢區域")]
        public int AreaId { get; set; }
        public string AreaName{ get; set; }
        [Display(Name = "週期")]
        public int CycleId { get; set; }
        public string CycleName { get; set; }
        [Display(Name = "類別")]
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ApplyDate { get; set; }
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
        public string EngUserName { get; set; }
        [Display(Name = "簽核主管ID")]
        public int? CheckerId { get; set; }
        [Display(Name = "簽核主管")]
        public string CheckerName { get; set; }
        public string CheckerUserName { get; set; }
        [Display(Name = "交班事項")]
        public string Note { get; set; }
        [Display(Name = "流程狀態")]
        public string FlowStatusId { get; set; }
        [Display(Name = "關卡")]
        public string Cls { get; set; }
        [Display(Name = "關卡人員")]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        DEInspectDocFlow flow { get; set; }
    }
}