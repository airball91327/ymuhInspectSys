using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("DB_GEN_STAFF_PW")]
    public class DB_GEN_STAFF_PW
    {
        [Key]
        public string STAFFNO { set; get; }
        public string STAFFNAME { set; get; }
        public string PASSWORD { set; get; }
        public string SDATE { set; get; }
        public string EDATE { set; get; }
    }
}