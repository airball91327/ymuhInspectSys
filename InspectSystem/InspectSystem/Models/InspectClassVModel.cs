using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    public class InspectClassVModel
    {
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [Display(Name = "班別代碼")]
        public int ShiftId { get; set; }
        [Display(Name = "類別代碼")]
        public int ClassId { get; set; }
        [Display(Name = "類別名稱")]
        public string ClassName { get; set; }
        [Display(Name = "排列順序")]
        public int ClassOrder { get; set; }
        [Display(Name = "必填欄位是否儲存")]
        public bool IsSaved { get; set; } // To show the required fields are saved or not in edit view.
        [Display(Name = "不正常數量")]
        public int CountErrors { get; set; } // To show the errors of class in detail view for checker.

    }
}