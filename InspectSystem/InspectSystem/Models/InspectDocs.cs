using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocs")]
    public class InspectDocs
    {
        public InspectDocs()
        {
            this.InspectDocDetails = new HashSet<InspectDocDetails>();
            this.InspectDocDetailsTemporary = new HashSet<InspectDocDetailsTemporary>();
            this.InspectDocFlows = new HashSet<InspectDocFlow>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "表單編號")]
        public int DocId { get; set; }
        [Required]
        [Display(Name = "日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ApplyDate { get; set; }
        [Display(Name = "完成時間")]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }
        [Required]
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [NotMapped]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [Required]
        [Display(Name = "巡檢人員ID")]
        public int EngId { get; set; }
        [Required]
        [Display(Name = "巡檢人員")]
        public string EngName { get; set; }
        [Required]
        [Display(Name = "簽核主管ID")]
        public int CheckerId { get; set; }
        [Required]
        [Display(Name = "簽核主管")]
        public string CheckerName { get; set; }
        [Required]
        [ForeignKey("InspectFlowStatus")]
        [Display(Name = "流程狀態編號")]
        public int FlowStatusId { get; set; }

        public virtual ICollection<InspectDocDetails> InspectDocDetails { get; set; }
        public virtual ICollection<InspectDocDetailsTemporary> InspectDocDetailsTemporary { get; set; }
        public virtual ICollection<InspectDocFlow> InspectDocFlows { get; set; }

        public virtual InspectFlowStatus InspectFlowStatus { get; set; }
    }
}