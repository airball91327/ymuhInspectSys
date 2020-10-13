using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InspectSystem.Models
{
    public class SelectUserVModel
    {
        [Display(Name = "選擇人員")]
        public int Suserid { get; set; }
        [Display(Name = "姓名(關鍵字)")]
        public string Susername { get; set; }
        [Display(Name = "部門名稱")]
        public string Sdptid { get; set; }
        [Display(Name = "部門(關鍵字)")]
        public string Sdptname { get; set; }
    }
}