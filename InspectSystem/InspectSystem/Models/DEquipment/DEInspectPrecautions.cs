using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    [Table("DEInspectPrecautions")]
    public class DEInspectPrecautions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PrecautionId { get; set; }
        [Required]
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [Required]
        [Display(Name = "注意事項")]
        public string Content { get; set; }

        public virtual DEInspectArea DEInspectArea { get; set; }
    }
}