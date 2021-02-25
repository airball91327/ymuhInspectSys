using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspectSystem.Models.DEquipment
{
    public class DEAssignVModel
    {
    }

    public class DEAssign
    {
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "流程提示")]
        public string Hint { get; set; }
        [Required]
        [Display(Name = "簽核選項")]
        public string AssignCls { get; set; }
        [Display(Name = "意見描述")]
        public string AssignOpn { get; set; }
        [Required]
        [Display(Name = "流程關卡")]
        public string FlowCls { get; set; }
        [Required]
        [Display(Name = "關卡人員")]
        public int? FlowUid { get; set; }
        public string ClsNow { get; set; }
        [Display(Name = "允許驗收人結案?")]
        public bool CanClose { get; set; }
        public string AssetNo { get; set; }
    }
}