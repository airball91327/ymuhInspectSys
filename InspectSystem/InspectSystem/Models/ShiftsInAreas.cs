using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("ShiftsInAreas")]
    public class ShiftsInAreas
    {
        public int AreaId { get; set; }
        public int ShiftId { get; set; }
        public string Status { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
        public virtual InspectShifts InspectShifts { get; set; }

    }
}