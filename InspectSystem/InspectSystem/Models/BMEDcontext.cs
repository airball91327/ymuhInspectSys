using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace InspectSystem.Models
{
    public class BMEDcontext : DbContext
    {
        public BMEDcontext()
        : base("BMEDconnection")
        {
        }

        public virtual DbSet<InspectAreas> InspectAreas { get; set; }
        public virtual DbSet<InspectClasses> InspectClasses { get; set; }
        public virtual DbSet<InspectItems> InspectItems { get; set; }
        public virtual DbSet<InspectFields> InspectFields { get; set; }
        public virtual DbSet<InspectFieldDropDown> InspectFieldDropDown { get; set; }
        public virtual DbSet<InspectPrecautions> InspectPrecautions { get; set; }
        public virtual DbSet<InspectDocs> InspectDocs { get; set; }
        public virtual DbSet<InspectDocDetails> InspectDocDetails { get; set; }
        public virtual DbSet<InspectDocDetailsTemporary> InspectDocDetailsTemporary { get; set; }
        public virtual DbSet<InspectFlowStatus> InspectFlowStatus { get; set; }
        public virtual DbSet<InspectDocFlow> InspectDocFlows { get; set; }
        public virtual DbSet<InspectAreaChecker> InspectAreaCheckers { get; set; }
        public virtual DbSet<InspectMembers> InspectMembers { get; set; }
        public virtual DbSet<InspectMemberAreas> InspectMemberAreas { get; set; }
        public virtual DbSet<InspectShifts> InspectShifts { get; set; }
        public virtual DbSet<ShiftsInAreas> ShiftsInAreas { get; set; }
        public virtual DbSet<ClassesOfAreas> ClassesOfAreas { get; set; }

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<AppRoles> AppRoles { get; set; }
        public virtual DbSet<UsersInRoles> UsersInRoles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
    }
}