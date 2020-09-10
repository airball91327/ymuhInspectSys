using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("VentilatorDoc")]
    public class VentilatorDoc
    {
        public VentilatorDoc()
        {
            this.VentilatorDocDetail = new HashSet<VentilatorDocDetail>();
        }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "表單編號")]
        public int DocId { get; set; }
        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "使用狀態代碼")]
        public int StatusId { get; set; }
        [Required]
        [Display(Name = "日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ApplyDate { get; set; }
        [Display(Name = "完成時間")]
        [DataType(DataType.DateTime)]
        public DateTime? CloseDate { get; set; }
        [Required]
        [Display(Name = "巡檢人員ID")]
        public int EngId { get; set; }
        [Required]
        [Display(Name = "巡檢人員")]
        public string EngName { get; set; }
        [Display(Name = "簽核主管ID")]
        public int? CheckerId { get; set; }
        [Display(Name = "簽核主管")]
        public string CheckerName { get; set; }
        [Required]
        [Display(Name = "設備編號")]
        public string AssetNo { get; set; }
        [Display(Name = "設備名稱")]
        public string AssetName { get; set; }
        [Required]
        [Display(Name = "文件狀態")]
        public string DocStatus { get; set; }

        public virtual VentilatorStatus VentilatorStatus { get; set; }
        public virtual ICollection<VentilatorDocDetail> VentilatorDocDetail { get; set; }
    }
}