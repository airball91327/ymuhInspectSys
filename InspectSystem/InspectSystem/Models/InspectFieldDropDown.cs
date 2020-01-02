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
        [Display(Name = "ACID")]
        public int ACID { get; set; }
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemID { get; set; }
        [Required]
        [Display(Name = "欄位代碼")]
        public int FieldID { get; set; }
        [Display(Name = "選項內容")]
        public string Value { get; set; }
    }
}