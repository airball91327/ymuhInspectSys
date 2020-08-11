using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectAreas")]
    public class InspectAreas
    {
        public InspectAreas()
        {
            this.ClassesOfAreas = new HashSet<ClassesOfAreas>();
            this.InspectAreaChecker = new HashSet<InspectAreaChecker>();
            this.InspectFields = new HashSet<InspectFields>();
            this.InspectItems = new HashSet<InspectItems>();
            this.ShiftsInAreas = new HashSet<ShiftsInAreas>();
            this.InspectMembers = new HashSet<InspectMembers>();
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [Required]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public string Status { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }
        [NotMapped]
        [Display(Name = "預設簽核主管ID")]
        public int CheckerId { get; set; }
        [NotMapped]
        [Display(Name = "預設簽核主管名稱")]
        public string CheckerName { get; set; }

        public virtual ICollection<ClassesOfAreas> ClassesOfAreas { get; set; }
        public virtual ICollection<InspectAreaChecker> InspectAreaChecker { get; set; }
        public virtual ICollection<InspectFields> InspectFields { get; set; }
        public virtual ICollection<InspectItems> InspectItems { get; set; }
        public virtual ICollection<ShiftsInAreas> ShiftsInAreas { get; set; }
        public virtual ICollection<InspectMembers> InspectMembers { get; set; }
    }
}