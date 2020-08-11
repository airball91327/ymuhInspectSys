using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocFlow")]
    public class InspectDocFlow
    {
        [Key, Column(Order = 1)]
        [ForeignKey("InspectDocs")]
        [Display(Name = "表單編號")]
        public int DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "關卡號")]
        public int StepId { get; set; }
        [Required]
        [Display(Name = "關卡人員ID")]
        public int StepOwnerId { get; set; }
        [NotMapped]
        [Display(Name = "關卡人員")]
        public string StepOwnerName { get; set; }
        [Required]
        [Display(Name = "巡檢人員ID")]
        public int EngId { get; set; }
        [Required]
        [Display(Name = "簽核主管ID")]
        public int CheckerId { get; set; }
        [Display(Name = "意見描述")]
        public string Opinions { get; set; }
        [Required]
        [ForeignKey("InspectFlowStatusTable")]
        [Display(Name = "流程狀態編號")]
        public int FlowStatusId { get; set; }
        [Required]
        [Display(Name ="異動人員ID")]
        public int EditorId { get; set; }
        [Display(Name ="異動人員")]
        public string EditorName { get; set; }
        [Display(Name ="異動時間")]
        public DateTime? EditTime { get; set; }

        public virtual InspectFlowStatus InspectFlowStatusTable { get; set; }
        public virtual InspectDocs InspectDocs { get; set; }
    }
}