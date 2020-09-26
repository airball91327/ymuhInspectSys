using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDoc")]
    public class InspectDoc
    {
        public InspectDoc()
        {
            this.InspectDocDetail = new HashSet<InspectDocDetail>();
            this.InspectDocDetailTemp = new HashSet<InspectDocDetailTemp>();
        }

        [Key, Column(Order = 1)]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "班別")]
        public int ShiftId { get; set; }
        [Required]
        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ApplyDate { get; set; }
        [Display(Name = "完成時間")]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }
        [Display(Name = "結案日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CloseDate { get; set; }
        [Display(Name = "巡檢人員ID")]
        public int? EngId { get; set; }
        [Display(Name = "巡檢人員")]
        public string EngName { get; set; }
        [Display(Name = "簽核主管ID")]
        public int? CheckerId { get; set; }
        [Display(Name = "簽核主管")]
        public string CheckerName { get; set; }
        [Display(Name = "交班事項")]
        public string Note { get; set; }

        public virtual InspectDocIdTable InspectDocIdTable { get; set; }
        public virtual ICollection<InspectDocDetail> InspectDocDetail { get; set; }
        public virtual ICollection<InspectDocDetailTemp> InspectDocDetailTemp { get; set; }
    }
}