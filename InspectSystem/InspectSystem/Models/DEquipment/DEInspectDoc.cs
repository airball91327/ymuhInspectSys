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
            this.DEInspectDocFlow = new HashSet<DEInspectDocFlow>();
        }

        [Key]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Required]
        [Display(Name = "巡檢區域")]
        public int AreaId { get; set; }
        public string AreaName{ get; set; }
        [Required]
        [Display(Name = "週期")]
        public int CycleId { get; set; }
        public string CycleName { get; set; }
        [Required]
        [Display(Name = "類別")]
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        [Required]
        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ApplyDate { get; set; }
        [Display(Name = "完成時間")]
        [DataType(DataType.DateTime)]
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
        public virtual ICollection<DEInspectDocFlow> DEInspectDocFlow { get; set; }
    }
}