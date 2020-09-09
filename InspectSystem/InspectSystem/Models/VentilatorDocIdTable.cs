using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("VentilatorDocIdTable")]
    public class VentilatorDocIdTable
    {
        public VentilatorDocIdTable()
        {
            this.VentilatorDoc = new HashSet<VentilatorDoc>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "表單編號")]
        public int DocId { get; set; }
        [Required]
        [Display(Name = "表單日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ApplyDate { get; set; }
        [Required]
        [Display(Name = "設備編號")]
        public string AssetNo { get; set; }

        public virtual ICollection<VentilatorDoc> VentilatorDoc { get; set; }
    }
}