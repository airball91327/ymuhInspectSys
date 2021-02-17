using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    [Table("DEInspectArea")]
    public class DEInspectArea
    {
        public DEInspectArea()
        {
            this.CyclesInAreas = new HashSet<DECyclesInAreas>();
            this.DEInspectDoc = new HashSet<DEInspectDoc>();
            this.DEInspectPrecautions = new HashSet<DEInspectPrecautions>();
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [Required]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [Required]
        [Display(Name = "顯示區域")]
        public bool AreaStatus { get; set; }
        [Display(Name = "異動人員")]
        public int? Rtp { get; set; }
        [NotMapped]
        public string RtpName { get; set; }
        [Display(Name = "異動時間")]
        public DateTime? Rtt { get; set; }

        public virtual ICollection<DECyclesInAreas> CyclesInAreas { get; set; }
        public virtual ICollection<DEInspectDoc> DEInspectDoc { get; set; }
        public virtual ICollection<DEInspectPrecautions> DEInspectPrecautions { get; set; }
    }
}