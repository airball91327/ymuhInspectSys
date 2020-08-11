using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace InspectSystem.Models
{
    [Table("InspectFieldDropDown")]
    public class InspectFieldDropDown
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [Required]
        [Display(Name = "班別代碼")]
        public int ShiftId { get; set; }
        [Required]
        [Display(Name = "類別代碼")]
        public int ClassId { get; set; }
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "欄位代碼")]
        public int FieldId { get; set; }
        [Display(Name = "選項內容")]
        public string Value { get; set; }
    }
}