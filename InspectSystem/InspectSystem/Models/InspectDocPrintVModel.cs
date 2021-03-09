using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class InspectDocPrintVModel
    {
        public DateTime ApplyDate { get; set; }
        [Display(Name = "值班人員")]
        public string Shift1Engineer { get; set; }
        [Display(Name = "覆核者")]
        public string Shift1Checker { get; set; }
        [Display(Name = "交班事項")]
        public string Shift1Note { get; set; }
        public string Shift2Engineer { get; set; }
        public string Shift2Checker { get; set; }
        public string Shift2Note { get; set; }
        public string Shift3Engineer { get; set; }
        public string Shift3Checker { get; set; }
        public string Shift3Note { get; set; }
        public InspectDoc ShiftDoc1 { get; set; }
        public IEnumerable<InspectDocDetail> ShiftDetails1 { get; set; }
        public InspectDoc ShiftDoc2 { get; set; }
        public IEnumerable<InspectDocDetail> ShiftDetails2 { get; set; }
        public InspectDoc ShiftDoc3 { get; set; }
        public IEnumerable<InspectDocDetail> ShiftDetails3 { get; set; }

    }
}