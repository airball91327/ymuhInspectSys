using InspectSystem.Models.DEquipment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectFlowStatus")]
    public class InspectFlowStatus
    {
        public InspectFlowStatus()
        {
            this.InspectDocFlow = new HashSet<InspectDocFlow>();
            this.DEInspectDocFlow = new HashSet<DEInspectDocFlow>();
        }

        [Key]
        [Required]
        [Display(Name = "流程狀態編號")]
        public string FlowStatusId { get; set; }
        [Required]
        [Display(Name = "流程狀態")]
        public string FlowStatusDes { get; set; }

        public virtual ICollection<InspectDocFlow> InspectDocFlow { get; set; }
        public virtual ICollection<DEInspectDocFlow> DEInspectDocFlow { get; set; }
    }
}