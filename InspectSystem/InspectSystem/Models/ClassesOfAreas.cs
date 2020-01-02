using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("ClassesOfAreas")]
    public class ClassesOfAreas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "ACID")]
        public int ACID { get; set; }       // ACID = areaID * 100 + classID
        [Required]
        [ForeignKey("InspectAreas")]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [Required]
        [ForeignKey("InspectClasses")]
        [Display(Name = "類別代碼")]
        public int ClassID { get; set; }
        [NotMapped]
        [Display(Name = "是否已儲存")]
        public Boolean IsSaved { get; set; } // To show the class is saved or not in edit view.
        [NotMapped]
        [Display(Name = "不正常數量")]
        public int CountErrors { get; set; } // To show the errors of class in detail view for checker.

        public virtual InspectAreas InspectAreas { get; set; }
        public virtual InspectClasses InspectClasses { get; set; }
    }
}