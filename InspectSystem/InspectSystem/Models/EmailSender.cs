using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class Mail
    {
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string msg { get; set; }
    }
}