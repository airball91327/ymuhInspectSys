using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("VentilatorDocDetail")]
    public class VentilatorDocDetail
    {
        [Key, Column(Order = 1)]
        [Required]
        [Display(Name = "表單編號")]
        public int DocId { get; set; }
        [Key, Column(Order = 2)]
        [Required]
        [Display(Name = "使用狀態代碼")]
        public int StatusId { get; set; }
        [Required]
        [Display(Name = "使用狀態")]
        public string StatusName { get; set; }
        [Display(Name = "使用狀態排列順序")]
        public int? StatusOrder { get; set; }
        [Key, Column(Order = 3)]
        [Required]
        [Display(Name = "類別代碼")]
        public int ClassId { get; set; }
        [Required]
        [Display(Name = "類別名稱")]
        public string ClassName { get; set; }
        [Display(Name = "類別排列順序")]
        public int? ClassOrder { get; set; }
        [Key, Column(Order = 4)]
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Display(Name = "項目排列順序")]
        public int? ItemOrder { get; set; }
        [Key, Column(Order = 5)]
        [Required]
        [Display(Name = "欄位代碼")]
        public int FieldId { get; set; }
        [Display(Name = "欄位名稱")]
        public string FieldName { get; set; }
        [Display(Name = "單位")]
        public string UnitOfData { get; set; }
        [Display(Name = "數值")]
        public string Value { get; set; }
        [Display(Name = "是否正常")]
        public string IsFunctional { get; set; }
        [Display(Name = "備註說明")]
        public string ErrorDescription { get; set; }
        [Required]
        [Display(Name = "資料型態")]
        public string DataType { get; set; }    //"string", "float", "boolean", "checkbox", "dropdownlist"
        [Display(Name = "最小值")]
        public decimal? MinValue { get; set; }
        [Display(Name = "最大值")]
        public decimal? MaxValue { get; set; }
        [Required]
        [Display(Name = "是否必填")]
        public bool IsRequired { get; set; }
        [Display(Name = "下拉選單元件")]
        public string DropDownItems { get; set; }

        public virtual VentilatorDoc VentilatorDoc { get; set; }

    }
}