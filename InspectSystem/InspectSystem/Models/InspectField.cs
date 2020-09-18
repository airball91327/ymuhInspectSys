using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace InspectSystem.Models
{
    [Table("InspectField")]
    public class InspectField
    {
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AreaId { get; set; }
        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShiftId { get; set; }
        [Key, Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClassId { get; set; }
        [Key, Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemId { get; set; }
        [NotMapped]
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Key, Column(Order = 5)]
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
        public double? MinValue { get; set; }
        [Display(Name = "最大值")]
        public double? MaxValue { get; set; }
        [Required]
        [Display(Name = "顯示欄位")]
        public bool FieldStatus { get; set; }
        [Required]
        [Display(Name = "是否必填")]
        public bool IsRequired { get; set; }
        [Display(Name = "顯示昨日數值")]
        public bool? ShowPastValue { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual InspectItem InspectItem { get; set; }
    }
}