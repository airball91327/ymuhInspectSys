using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectClasses")]
    public class InspectClasses
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "類別代碼")]
        public int ClassID { get; set; }
        [Required]
        [Display(Name = "類別名稱")]
        public string ClassName { get; set; }
        [Required]
        [Display(Name = "排列順序")]
        public int ClassOrder { get; set; }
    }
}