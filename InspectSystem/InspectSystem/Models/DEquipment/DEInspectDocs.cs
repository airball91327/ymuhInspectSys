using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    [Table("DEInspectDoc")]
    public class DEInspectDoc
    {
        public DEInspectDoc()
        {
            this.DEInspectDocDetail = new HashSet<DEInspectDocDetail>();
            this.DEInspectDocDetailTemp = new HashSet<DEInspectDocDetailTemp>();
        }

        [Key, Column(Order = 1)]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "週期")]
        public int CycleId { get; set; }
        [Key, Column(Order = 3)]
        [Display(Name = "類別")]
        public int ClassId { get; set; }
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

        public virtual ICollection<DEInspectDocDetail> DEInspectDocDetail { get; set; }
        public virtual ICollection<DEInspectDocDetailTemp> DEInspectDocDetailTemp { get; set; }
    }
}