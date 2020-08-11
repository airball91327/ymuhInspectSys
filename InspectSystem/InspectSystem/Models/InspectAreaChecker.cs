﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectAreaChecker")]
    public class InspectAreaChecker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "編號")]
        public int AreaCheckerId { get; set; }          // AreaCheckerId = CheckerId * 100 + areaID
        [Required]
        [ForeignKey("InspectAreas")]
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [Required]
        [Display(Name = "簽核主管ID")]
        public int CheckerId { get; set; }
        [Required]
        [Display(Name = "簽核主管")]
        public string CheckerName { get; set; }
        [Display(Name = "電子郵件")]
        [EmailAddress(ErrorMessage = "無效的Email格式")]
        public string Email { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
    }
}