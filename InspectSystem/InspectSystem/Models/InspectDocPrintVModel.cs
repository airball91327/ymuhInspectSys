using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class InspectDocPrintVModel
    {
        public DateTime ApplyDate { get; set; }
        public IEnumerable<InspectDocDetail> ShiftDetails1 { get; set; }
        public IEnumerable<InspectDocDetail> ShiftDetails2 { get; set; }
        public IEnumerable<InspectDocDetail> ShiftDetails3 { get; set; }

    }
}