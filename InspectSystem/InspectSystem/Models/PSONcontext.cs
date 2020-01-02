using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InspectSystem.Models
{
    public class PSONcontext: DbContext
    {
        public PSONcontext()
        :base("PSONconnection") { }

        public DbSet<DB_GEN_STAFF_PW> DB_GEN_STAFF_PWs { get; set; }
        
    }
}