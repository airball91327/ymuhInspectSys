using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectMembers")]
    public class InspectMembers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "員工代號")]
        public int MemberId { get; set; }
        [Required]
        [Display(Name = "員工姓名")]
        public string MemberName { get; set; }
        [Display(Name = "所屬部門")]
        public string Department { get; set; }
    }
}