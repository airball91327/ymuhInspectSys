using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using InspectSystem.Models.DEquipment;

namespace InspectSystem.Models
{
    public class BMEDcontext : DbContext
    {
        public BMEDcontext()
        : base("BMEDconnection")    //BMEDconnection //AzureConnection
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

        // 危險性設備巡檢相關
        public virtual DbSet<DEInspectArea> DEInspectArea { get; set; }
        public virtual DbSet<DEInspectCycle> DEInspectCycle { get; set; }
        public virtual DbSet<DECyclesInAreas> DECyclesInAreas { get; set; }
        public virtual DbSet<DEInspectClass> DEInspectClass { get; set; }
        public virtual DbSet<DEInspectItem> DEInspectItem { get; set; }
        public virtual DbSet<DEInspectField> DEInspectField { get; set; }
        public virtual DbSet<DEInspectFieldDropDown> DEInspectFieldDropDown { get; set; }
        public virtual DbSet<DEInspectDoc> DEInspectDoc { get; set; }
        public virtual DbSet<DEInspectDocDetail> DEInspectDocDetail { get; set; }
        public virtual DbSet<DEInspectDocDetailTemp> DEInspectDocDetailTemp { get; set; }
        public virtual DbSet<DEInspectDocFlow> DEInspectDocFlow { get; set; }
        public virtual DbSet<DEInspectPrecautions> DEInspectPrecautions { get; set; }



        // 使用者相關
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<AppRoles> AppRoles { get; set; }
        public virtual DbSet<UsersInRoles> UsersInRoles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }

        public virtual DbSet<InspectDocStatus> InspectDocStatus { get; set; }
    }
}