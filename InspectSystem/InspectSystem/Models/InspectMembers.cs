using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectMembers")]
    public class InspectMembers
    {
        public InspectMembers()
        {
            this.InspectAreas = new HashSet<InspectArea>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "員工代號")]
        public int MemberId { get; set; }
        [NotMapped]
        public string MemberUserName { get; set; }
        [Required]
        [Display(Name = "員工姓名")]
        public string MemberName { get; set; }
        [Display(Name = "所屬部門")]
        public string DptId { get; set; }
        [NotMapped]
        public string DptName { get; set; }

        public virtual ICollection<InspectArea> InspectAreas { get; set; }
    }
}