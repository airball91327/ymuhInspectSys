using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectFlowStatusTable")]
    public class InspectFlowStatusTable
    {
        [Key]
        [Required]
        [Display(Name = "流程狀態編號")]
        public int FlowStatusID { get; set; }
        [Required]
        [Display(Name = "流程狀態")]
        public string FlowStatusName { get; set; }
    }
}