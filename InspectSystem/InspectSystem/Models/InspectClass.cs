﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectClass")]
    public class InspectClass
    {
        public InspectClass()
        {
            this.InspectItem = new HashSet<InspectItem>();
            this.InspectField = new HashSet<InspectField>();
        }

        [Key, Column(Order = 1)]
        public int AreaId { get; set; }
        [Key, Column(Order = 2)]
        public int ShiftId { get; set; }
        [Key, Column(Order = 3)]
        [Display(Name = "類別代碼")]
        public int ClassId { get; set; }
        [Required]
        [Display(Name = "類別名稱")]
        public string ClassName { get; set; }
        [Required]
        [Display(Name = "顯示類別")]
        public bool ClassStatus { get; set; }
        [Required]
        [Display(Name = "排列順序")]
        public int ClassOrder { get; set; }
        [Display(Name = "異動人員")]
        public int? Rtp { get; set; }
        [NotMapped]
        public string RtpName { get; set; }
        [Display(Name = "異動時間")]
        public DateTime? Rtt { get; set; }
        [Display(Name = "報表使用欄位")]
        public string IsReport { get; set; }

        public virtual ShiftsInAreas ShiftsInAreas { get; set; }
        public virtual ICollection<InspectItem> InspectItem { get; set; }
        public virtual ICollection<InspectField> InspectField { get; set; }
    }
}