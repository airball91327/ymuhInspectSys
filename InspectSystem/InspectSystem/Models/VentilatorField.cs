using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace InspectSystem.Models
{
    [Table("VentilatorField")]
    public class VentilatorField
    {

        [Key, Column(Order = 1)]
        [Required]
        [Display(Name = "使用狀態代碼")]
        public int StatusId { get; set; }
        [Key, Column(Order = 2)]
        [Required]
        [Display(Name = "類別代碼")]
        public int ClassId { get; set; }
        [Key, Column(Order = 3)]
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemId { get; set; }
        [Key, Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "欄位代碼")]
        public int FieldId { get; set; }
        [Display(Name = "欄位名稱")]
        public string FieldName { get; set; }
        [Required]
        [Display(Name = "資料型態")]
        public string DataType { get; set; }        //"string", "float", "boolean", "checkbox", "dropdownlist"
        [Display(Name = "單位")]
        public string UnitOfData { get; set; }
        [Display(Name = "最小值")]
        public decimal? MinValue { get; set; }
        [Display(Name = "最大值")]
        public decimal? MaxValue { get; set; }
        [Required]
        [Display(Name = "顯示欄位")]
        public bool FieldStatus { get; set; }
        [Required]
        [Display(Name = "是否必填")]
        public bool IsRequired { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual VentilatorStatus VentilatorStatus { get; set; }
        public virtual VentilatorClass VentilatorClass { get; set; }
        public virtual VentilatorItem VentilatorItem { get; set; }
    }
}