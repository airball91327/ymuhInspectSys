using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("VentilatorStatus")]
    public class VentilatorStatus
    {
        public VentilatorStatus()
        {
            this.VentilatorClass = new HashSet<VentilatorClass>();
            this.VentilatorItem = new HashSet<VentilatorItem>();
            this.VentilatorField = new HashSet<VentilatorField>();
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "使用狀態代碼")]
        public int StatusId { get; set; }
        [Required]
        [Display(Name = "使用狀態")]
        public string StatusName { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public bool Status { get; set; }
        [Required]
        [Display(Name = "排列順序")]
        public int StatusOrder { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual ICollection<VentilatorClass> VentilatorClass { get; set; }
        public virtual ICollection<VentilatorItem> VentilatorItem { get; set; }
        public virtual ICollection<VentilatorField> VentilatorField { get; set; }
    }
}