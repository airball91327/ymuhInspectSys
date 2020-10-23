using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectArea")]
    public class InspectArea
    {
        public InspectArea()
        {
            this.ShiftsInAreas = new HashSet<ShiftsInAreas>();
            this.InspectDoc = new HashSet<InspectDoc>();
            this.InspectPrecautions = new HashSet<InspectPrecautions>();
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

        public virtual ICollection<ShiftsInAreas> ShiftsInAreas { get; set; }
        public virtual ICollection<InspectDoc> InspectDoc { get; set; }
        public virtual ICollection<InspectPrecautions> InspectPrecautions { get; set; }
    }
}