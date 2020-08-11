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
        [ForeignKey("InspectAreas")]
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [ForeignKey("InspectShifts")]
        [Display(Name = "班別代碼")]
        public int ShiftId { get; set; }
        [Key, Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [ForeignKey("InspectClasses")]
        [Display(Name = "類別代碼")]
        public int ClassId { get; set; }
        [Key, Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "顯示項目")]
        public bool ItemStatus { get; set; }
        [Required]
        [Display(Name = "排列順序")]
        public int ItemOrder { get; set; }
        public int? Rtp { get; set; }
        public DateTime? Rtt { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
        public virtual InspectClasses InspectClasses { get; set; }
        public virtual InspectShifts InspectShifts { get; set; }
    }
}