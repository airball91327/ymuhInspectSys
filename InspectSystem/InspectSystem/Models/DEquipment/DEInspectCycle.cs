using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    [Table("DEInspectCycle")]
    public class DEInspectCycle
    {
        public DEInspectCycle()
        {
            this.DECyclesInAreas = new HashSet<DECyclesInAreas>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "週期代碼")]
        public int CycleId { get; set; }
        [Required]
        [Display(Name = "週期名稱")]
        public string CycleName { get; set; }
        [Display(Name = "異動人員")]
        public int? Rtp { get; set; }
        [NotMapped]
        public string RtpName { get; set; }
        [Display(Name = "異動時間")]
        public DateTime? Rtt { get; set; }

        public virtual ICollection<DECyclesInAreas> DECyclesInAreas { get; set; }

    }
}