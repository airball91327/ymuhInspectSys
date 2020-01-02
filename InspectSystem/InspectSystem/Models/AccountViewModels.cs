using InspectSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace InspectSystem.Models
{
    public class LogOnModel
    {
        [Required]
        [Display(Name = "使用者帳號")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "保持登入?")]
        public bool RememberMe { get; set; }

        [Display(Name = "測試模式")]
        public bool TestMode { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "目前密碼")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }
    public class UserInRolesVModel
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }

        public static List<UserInRolesVModel> getRoles()
        {
            List<UserInRolesVModel> rolelist = new List<UserInRolesVModel>();
            UserInRolesVModel rv;

            BMEDcontext db = new BMEDcontext();
            foreach (AppRoles r in db.AppRoles.ToList())
            {
                rv = new UserInRolesVModel();
                rv.RoleName = r.RoleName;
                rv.Description = r.Description;
                rv.IsSelected = false;
                rolelist.Add(rv);
            }
            var rst = rolelist.GroupBy(g => g.RoleName).Select(g => g.First());
            return rst.ToList();
        }
    }
}
