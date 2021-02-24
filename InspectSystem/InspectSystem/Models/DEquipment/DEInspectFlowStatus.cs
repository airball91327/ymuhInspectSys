using InspectSystem.Models.DEquipment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    [Table("DEInspectFlowStatus")]
    public class DEInspectFlowStatus
    {
        public DEInspectFlowStatus()
        {
            this.DEInspectDocFlow = new HashSet<DEInspectDocFlow>();
        }

        [Key]
        [Required]
        [Display(Name = "流程狀態編號")]
        public string FlowStatusId { get; set; }
        [Required]
        [Display(Name = "流程狀態")]
        public string FlowStatusDes { get; set; }

        public virtual ICollection<DEInspectDocFlow> DEInspectDocFlow { get; set; }
    }
}