using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class DocDetailQryVModel
    {
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "申請日期(起)")]
        public string StartDate { get; set; }
        [Display(Name = "申請日期(止)")]
        public string EndDate { get; set; }
        [Display(Name = "區域代碼")]
        public string AreaId { get; set; }
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [Display(Name = "班別代碼")]
        public string ShiftId { get; set; }
        [Required]
        [Display(Name = "班別")]
        public string ShiftName { get; set; }
        [Display(Name = "類別代碼")]
        public string ClassId { get; set; }
        [Display(Name = "類別名稱")]
        public string ClassName { get; set; }
        [Display(Name = "項目代碼")]
        public string ItemId { get; set; }
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Display(Name = "欄位代碼")]
        public string FieldId { get; set; }
        [Display(Name = "欄位名稱")]
        public string FieldName { get; set; }
    }
}