using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class InspectDocDetailViewModels
    {
        public IEnumerable<InspectDocDetail> InspectDocDetails { get; set; }
        public IEnumerable<InspectDocDetailTemp> InspectDocDetailsTemp { get; set; }
        public IEnumerable<InspectField> InspectFields { get; set; }
        public IEnumerable<InspectItem> InspectItems { get; set; }
        public IEnumerable<InspectFieldDropDown> InspectFieldDropDowns { get; set; }
    }
}