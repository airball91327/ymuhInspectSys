using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("AppRoles")]
    public class AppRoles
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [Display(Name = "角色名稱")]
        public string RoleName { get; set; }
        [Display(Name = "描述")]
        public string Description { get; set; }
    }
}