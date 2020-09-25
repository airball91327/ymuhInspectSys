using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocFlow")]
    public class InspectDocFlow
    {
        [Key, Column(Order = 1)]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "關卡號")]
        public int StepId { get; set; }
        [Required]
        [Display(Name = "關卡人員ID")]
        public int UserId { get; set; }
        [NotMapped]
        [Display(Name = "關卡人員")]
        public string UserFullName { get; set; }
        [Display(Name = "意見描述")]
        public string Opinions { get; set; }
        [Required]
        [Display(Name = "流程狀態")]
        public string FlowStatusId { get; set; }
        [Display(Name ="異動人員ID")]
        public int? Rtp { get; set; }
        [Display(Name ="異動時間")]
        public DateTime? Rtt { get; set; }
        [Display(Name = "關卡")]
        public string Cls { get; set; }

        public virtual InspectFlowStatus InspectFlowStatus { get; set; }
        public virtual InspectDocIdTable InspectDocIdTable { get; set; }
    }
}