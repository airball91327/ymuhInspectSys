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
            this.InspectDocFlows = new HashSet<InspectDocFlow>();
            this.InspectDocs = new HashSet<InspectDocs>();
        }

        [Key]
        [Required]
        [Display(Name = "流程狀態編號")]
        public int FlowStatusId { get; set; }
        [Required]
        [Display(Name = "流程狀態")]
        public string FlowStatusName { get; set; }

        public virtual ICollection<InspectDocFlow> InspectDocFlows { get; set; }
        public virtual ICollection<InspectDocs> InspectDocs { get; set; }
    }
}