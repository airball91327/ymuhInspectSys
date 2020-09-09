using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("VentilatorItem")]
    public class VentilatorItem
    {
        public VentilatorItem()
        {
            this.VentilatorField = new HashSet<VentilatorField>();
        }

        [Key, Column(Order = 1)]
        [Required]
        [Display(Name = "使用狀態代碼")]
        public int StatusId { get; set; }
        [Key, Column(Order = 2)]
        [Required]
        [Display(Name = "類別代碼")]
        public int ClassId { get; set; }
        [Key, Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "顯示項目")]
        public bool ItemStatus { get; set; }
        [Required]
        [Display(Name = "排列順序")]
        public int ItemOrder { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual VentilatorStatus VentilatorStatus { get; set; }

        public virtual ICollection<VentilatorField> VentilatorField { get; set; }
    }
}