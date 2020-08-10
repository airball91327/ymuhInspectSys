using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectShifts")]
    public class InspectShifts
    {
        public InspectShifts()
        {
            this.InspectFields = new HashSet<InspectFields>();
            this.InspectItems = new HashSet<InspectItems>();
            this.ShiftsInAreas = new HashSet<ShiftsInAreas>();
        }

        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public int ShiftOrder { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual ICollection<InspectFields> InspectFields { get; set; }
        public virtual ICollection<InspectItems> InspectItems { get; set; }
        public virtual ICollection<ShiftsInAreas> ShiftsInAreas { get; set; }

    }
}