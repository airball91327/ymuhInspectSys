using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectPrecautions")]
    public class InspectPrecautions
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrecautionID { get; set; }
        [Required]
        [ForeignKey("InspectAreas")]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [Required]
        [Display(Name = "注意事項")]
        public string Content { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
    }
}