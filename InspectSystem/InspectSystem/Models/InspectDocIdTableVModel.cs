using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class InspectDocIdTableVModel
    {
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "申請日期")]
        public string ApplyDate { get; set; }
        [Display(Name = "結案日期")]
        public string CloseDate { get; set; }
        [Display(Name = "巡檢區域")]
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        [Display(Name = "目前班別")]
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        [Display(Name = "案件狀態")]
        public string DocStatusId { get; set; }
        public string DocStatusDes { get; set; }
        [Display(Name = "流程狀態")]
        public string FlowStatusId { get; set; }
        public string FlowStatusDes { get; set; }
        [Display(Name = "巡檢人員ID")]
        public string EngUserName { get; set; }
        [Display(Name = "巡檢人員")]
        public string EngFullName { get; set; }
    }
}