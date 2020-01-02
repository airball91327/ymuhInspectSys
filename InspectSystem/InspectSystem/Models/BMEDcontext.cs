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
        : base("AZUREconnection")
        {
        }

        public DbSet<InspectAreas> InspectAreas { get; set; }
        public DbSet<InspectClasses> InspectClasses { get; set; }
        public DbSet<InspectItems> InspectItems { get; set; }
        public DbSet<InspectFields> InspectFields { get; set; }
        public DbSet<InspectFieldDropDown> InspectFieldDropDown { get; set; }
        public DbSet<ClassesOfAreas> ClassesOfAreas { get; set; }
        public DbSet<InspectPrecautions> InspectPrecautions { get; set; }
        public DbSet<InspectDocs> InspectDocs { get; set; }
        public DbSet<InspectDocDetails> InspectDocDetails { get; set; }
        public DbSet<InspectDocDetailsTemporary> InspectDocDetailsTemporary { get; set; }
        public DbSet<InspectFlowStatusTable> InspectFlowStatusTable { get; set; }
        public DbSet<InspectDocFlow> InspectDocFlows { get; set; }
        public DbSet<InspectAreaChecker> InspectAreaCheckers { get; set; }
        public DbSet<InspectMembers> InspectMembers { get; set; }
        public DbSet<InspectMemberAreas> InspectMemberAreas { get; set; }
    }
}