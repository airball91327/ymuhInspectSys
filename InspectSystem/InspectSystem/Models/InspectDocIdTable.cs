using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocIdTable")]
    public class InspectDocIdTable
    {
        public InspectDocIdTable()
        {
            this.InspectDoc = new HashSet<InspectDoc>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Required]
        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ApplyDate { get; set; }
        [Display(Name = "結案日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CloseDate { get; set; }
        [Required]
        [Display(Name = "巡檢區域")]
        public int AreaId { get; set; }
        [Required]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }

        public virtual ICollection<InspectDoc> InspectDoc { get; set; }
    }
}