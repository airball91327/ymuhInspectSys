using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class InspectMembersViewModel
    {
        public IEnumerable<InspectMembers> InspectMembers { get; set; }
        public IEnumerable<InspectMemberAreas> InspectMemberAreas { get; set; }
    }
}