using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class InspectDocDetailsViewModels
    {
        public IEnumerable<InspectDocDetails> InspectDocDetails { get; set; }
        public IEnumerable<InspectDocDetailsTemporary> InspectDocDetailsTemporary { get; set; }
        public IEnumerable<InspectFields> InspectFields { get; set; }
        public IEnumerable<InspectItems> InspectItems { get; set; }
        public IEnumerable<InspectFieldDropDown> InspectFieldDropDowns { get; set; }
    }
}