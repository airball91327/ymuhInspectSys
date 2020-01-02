using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    public class UsersInRoles
    {
        [Key, Column(Order =1)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        [Key, Column(Order = 2)]
        public int RoleId { get; set; }
    }
}