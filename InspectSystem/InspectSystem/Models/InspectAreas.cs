using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectAreas")]
    public class InspectAreas
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [Required]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [NotMapped]
        [Display(Name = "預設簽核主管ID")]
        public int CheckerID { get; set; }
        [NotMapped]
        [Display(Name = "預設簽核主管名稱")]
        public string CheckerName { get; set; }
    }
}