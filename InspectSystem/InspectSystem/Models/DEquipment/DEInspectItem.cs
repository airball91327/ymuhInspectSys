using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models.DEquipment
{
    [Table("DEInspectItem")]
    public class DEInspectItem
    {
        public DEInspectItem()
        {
            this.DEInspectField = new HashSet<DEInspectField>();
        }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AreaId { get; set; }
        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CycleId { get; set; }
        [Key, Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClassId { get; set; }
        [Key, Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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
        [Display(Name = "異動人員")]
        public int? Rtp { get; set; }
        [NotMapped]
        public string RtpName { get; set; }
        [Display(Name = "異動時間")]
        public DateTime? Rtt { get; set; }
        [Display(Name = "報表使用欄位")]
        public string IsReport { get; set; }

        public virtual DEInspectClass DEInspectClass { get; set; }
        public virtual ICollection<DEInspectField> DEInspectField { get; set; }

    }
}