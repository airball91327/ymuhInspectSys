using InspectSystem.Models.DEquipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspectSystem.Models.DEquipment
{
    public class DEInspectDocDetailVModel
    {
        public IEnumerable<DEInspectDocDetail> InspectDocDetail { get; set; }
        public IEnumerable<DEInspectDocDetailTemp> InspectDocDetailTemp { get; set; }
        public IEnumerable<DEInspectField> InspectFields { get; set; }
        public IEnumerable<DEInspectItem> InspectItems { get; set; }
        public IEnumerable<DEInspectFieldDropDown> InspectFieldDropDowns { get; set; }
    }
}