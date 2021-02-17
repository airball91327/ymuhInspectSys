using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    [Table("DECyclesInAreas")]
    public class DECyclesInAreas
    {
        public DECyclesInAreas()
        {
            this.DEInspectClass = new HashSet<DEInspectClass>();
        }

        [Key, Column(Order = 1)]
        public int AreaId { get; set; }
        [Key, Column(Order = 2)]
        public int CycleId { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public bool Status { get; set; }
        [Display(Name = "異動人員")]
        public int? Rtp { get; set; }
        [NotMapped]
        public string RtpName { get; set; }
        [Display(Name = "異動時間")]
        public DateTime? Rtt { get; set; }

        public virtual DEInspectArea DEInspectArea { get; set; }
        public virtual DEInspectCycle DEInspectCycle { get; set; }
        public virtual ICollection<DEInspectClass> DEInspectClass { get; set; }

    }
}