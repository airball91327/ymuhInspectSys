using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class ReportQryVModel
    {
        [Display(Name = "區域")]
        public int AreaId { get; set; }
        [Display(Name = "週期")]
        public int CycleId { get; set; }
        [Display(Name = "類別")]
        public int ClassId { get; set; }
        [Display(Name = "表單申請日(起始日)")]
        public DateTime ApplyDateFrom { get; set; }
        //[Display(Name = "表單申請日(至)")]
        //public DateTime ApplyDateTo { get; set; }
    }
}