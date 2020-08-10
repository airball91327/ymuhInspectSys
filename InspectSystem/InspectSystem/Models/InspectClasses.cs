using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectClasses")]
    public class InspectClasses
    {
        public InspectClasses()
        {
            this.ClassesOfAreas = new HashSet<ClassesOfAreas>();
            this.InspectFields = new HashSet<InspectFields>();
            this.InspectItems = new HashSet<InspectItems>();
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "類別代碼")]
        public int ClassID { get; set; }
        [Required]
        [Display(Name = "類別名稱")]
        public string ClassName { get; set; }
        [Required]
        [Display(Name = "排列順序")]
        public int ClassOrder { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual ICollection<ClassesOfAreas> ClassesOfAreas { get; set; }
        public virtual ICollection<InspectFields> InspectFields { get; set; }
        public virtual ICollection<InspectItems> InspectItems { get; set; }
    }
}