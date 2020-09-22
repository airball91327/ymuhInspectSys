using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocStatus")]
    public class InspectDocStatus
    {
        public InspectDocStatus()
        {
            this.InspectDocIdTable = new HashSet<InspectDocIdTable>();
        }

        [Key]
        [Required]
        [Display(Name = "案件狀態編號")]
        public string DocStatusId { get; set; }
        [Required]
        [Display(Name = "案件狀態")]
        public string DocStatusDes { get; set; }

        public virtual ICollection<InspectDocIdTable> InspectDocIdTable { get; set; }
    }
}