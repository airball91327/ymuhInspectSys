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
        : base("AzureConnection")    //BMEDconnection //AzureConnection
        {
        }

        // 一般巡檢相關
        public virtual DbSet<InspectArea> InspectArea { get; set; }
        public virtual DbSet<InspectShift> InspectShift { get; set; }
        public virtual DbSet<ShiftsInAreas> ShiftsInAreas { get; set; }
        public virtual DbSet<InspectClass> InspectClass { get; set; }
        public virtual DbSet<InspectItem> InspectItem { get; set; }
        public virtual DbSet<InspectField> InspectField { get; set; }
        public virtual DbSet<InspectFieldDropDown> InspectFieldDropDown { get; set; }
        public virtual DbSet<InspectDocIdTable> InspectDocIdTable { get; set; }
        public virtual DbSet<InspectDoc> InspectDoc { get; set; }
        public virtual DbSet<InspectDocDetail> InspectDocDetail { get; set; }
        public virtual DbSet<InspectDocDetailTemp> InspectDocDetailTemp{ get; set; }
        public virtual DbSet<InspectDocFlow> InspectDocFlow { get; set; }
        public virtual DbSet<InspectFlowStatus> InspectFlowStatus { get; set; }
        public virtual DbSet<InspectPrecautions> InspectPrecautions { get; set; }

        // 呼吸器相關
        //public virtual DbSet<VentilatorDoc> VentilatorDoc { get; set; }
        //public virtual DbSet<VentilatorDocIdTable> VentilatorDocIdTable { get; set; }
        //public virtual DbSet<VentilatorDocDetail> VentilatorDocDetail { get; set; }
        //public virtual DbSet<VentilatorStatus> VentilatorStatus { get; set; }
        //public virtual DbSet<VentilatorClass> VentilatorClass { get; set; }
        //public virtual DbSet<VentilatorItem> VentilatorItem { get; set; }
        //public virtual DbSet<VentilatorField> VentilatorField { get; set; }


        // 使用者相關
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<AppRoles> AppRoles { get; set; }
        public virtual DbSet<UsersInRoles> UsersInRoles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }

        public System.Data.Entity.DbSet<InspectSystem.Models.InspectDocStatus> InspectDocStatus { get; set; }
    }
}