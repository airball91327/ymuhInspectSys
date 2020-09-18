using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class InspectDocDetailViewModel
    {
        public IEnumerable<InspectDocDetail> InspectDocDetail { get; set; }
        public IEnumerable<InspectDocDetailTemp> InspectDocDetailTemp { get; set; }
        public IEnumerable<InspectField> InspectField { get; set; }
        public IEnumerable<InspectItem> InspectItem { get; set; }
        public IEnumerable<InspectFieldDropDown> InspectFieldDropDown { get; set; }
    }
}