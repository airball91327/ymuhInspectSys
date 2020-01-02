using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocs")]
    public class InspectDocs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "表單編號")]
        public int DocID { get; set; }
        [Required]
        [Display(Name = "日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date { get; set; }
        [Display(Name = "完成時間")]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }
        [Required]
        [ForeignKey("InspectAreas")]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [NotMapped]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [Required]
        [Display(Name = "巡檢人員ID")]
        public int WorkerID { get; set; }
        [Required]
        [Display(Name = "巡檢人員")]
        public string WorkerName { get; set; }
        [Required]
        [Display(Name = "簽核主管ID")]
        public int CheckerID { get; set; }
        [Required]
        [Display(Name = "簽核主管")]
        public string CheckerName { get; set; }
        [Required]
        [ForeignKey("InspectFlowStatusTable")]
        [Display(Name = "流程狀態編號")]
        public int FlowStatusID { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
        public virtual InspectFlowStatusTable InspectFlowStatusTable { get; set; }
    }
}