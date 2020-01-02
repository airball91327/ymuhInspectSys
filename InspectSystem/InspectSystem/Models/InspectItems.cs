using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectItems")]
    public class InspectItems
    {
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "ACID")]
        public int ACID { get; set; }         //對應ClassesOfAreas的ID、程式產生
        [ForeignKey("InspectAreas")]
        public int AreaID { get; set; }
        [ForeignKey("InspectClasses")]
        public int ClassID { get; set; }
        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemID { get; set; }
        [Required]
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "顯示項目")]
        public Boolean ItemStatus { get; set; }
        [Required]
        [Display(Name = "排列順序")]
        public int ItemOrder { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
        public virtual InspectClasses InspectClasses { get; set; }

    }
}