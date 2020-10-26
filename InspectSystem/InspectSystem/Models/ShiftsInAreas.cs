using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("ShiftsInAreas")]
    public class ShiftsInAreas
    {
        public ShiftsInAreas()
        {
            this.InspectClass = new HashSet<InspectClass>();
        }

        [Key, Column(Order = 1)]
        public int AreaId { get; set; }
        [Key, Column(Order = 2)]
        public int ShiftId { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public bool Status { get; set; }
        [Display(Name = "異動人員")]
        public int? Rtp { get; set; }
        [NotMapped]
        public string RtpName { get; set; }
        [Display(Name = "異動時間")]
        public DateTime? Rtt { get; set; }

        public virtual InspectArea InspectArea { get; set; }
        public virtual InspectShift InspectShift { get; set; }
        public virtual ICollection<InspectClass> InspectClass { get; set; }

    }
}