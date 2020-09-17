using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectShift")]
    public class InspectShift
    {
        public InspectShift()
        {
            this.ShiftsInAreas = new HashSet<ShiftsInAreas>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "班別代碼")]
        public int ShiftId { get; set; }
        [Required]
        [Display(Name = "班別名稱")]
        public string ShiftName { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual ICollection<ShiftsInAreas> ShiftsInAreas { get; set; }

    }
}