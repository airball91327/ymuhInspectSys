using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("SystemLogs")]
    public class SystemLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "訊息類別")]
        public string LogClass { get; set; }
        [Required]
        [Display(Name = "紀錄時間")]
        public DateTime LogTime { get; set; }
        [Display(Name = "人員代號")]
        public int? UserId { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        [Display(Name = "人員姓名")]
        public string FullName { get; set; }
        [Display(Name = "執行動作")]
        public string Action { get; set; }
    }
}