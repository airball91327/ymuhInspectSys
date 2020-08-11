using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("ShiftsInAreas")]
    public class ShiftsInAreas
    {
        [Key, Column(Order = 1)]
        public int AreaId { get; set; }
        [Key, Column(Order = 2)]
        public int ShiftId { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public string Status { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
        public virtual InspectShifts InspectShifts { get; set; }

    }
}