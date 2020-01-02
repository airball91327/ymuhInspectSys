using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using InspectSystem.Models;
using System.Security.Cryptography;
using System.Text;

namespace InspectSystem.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        BMEDcontext context = new BMEDcontext();
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            AppRoles role;
            AppUser user;
            foreach (string username in usernames)
            {
                user = context.AppUsers.Where(u => u.UserName == username).FirstOrDefault();
                foreach (string rolename in roleNames)
                {
                    role = context.AppRoles.Where(a => a.RoleName == rolename).FirstOrDefault();
                    if (user != null && role != null)
                    {
                        if (context.UsersInRoles.Where(u => u.UserName == username)
                            .Where(u => u.RoleId == role.RoleId).Count() == 0)
                        {
                            UsersInRoles ur = new UsersInRoles();
                            ur.UserId = user.Id;
                            ur.UserName = username;
                            ur.RoleId = role.RoleId;
                            context.UsersInRoles.Add(ur);
                            context.SaveChanges();
                        }
                    }
                }
            }
            
        }

        public override void CreateRole(string roleName)
        {
            if (!RoleExists(roleName))
            {
                AppRoles role = new AppRoles();
                role.RoleName = roleName;
                role.RoleId = 1;
                context.AppRoles.Add(role);
                context.SaveChanges();                
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            return context.AppRoles
                .Select(u => u.RoleName).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            return context.AppRoles.Join(context.UsersInRoles, r => r.RoleId, u => u.RoleId,
                (r, u) => new
                {
                    r.RoleName,
                    u.UserName
                }).Where(r => r.UserName == username)
                .Select(u => u.RoleName).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return context.AppRoles.Join(context.UsersInRoles, r => r.RoleId, u => u.RoleId,
                (r, u) => new
                {
                    r.RoleName,
                    u.UserName
                }).Where(r => r.RoleName == roleName)
                .Select(u => u.UserName).ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool result = false;
            int cnt = context.AppRoles
                .Join(context.UsersInRoles, r => r.RoleId, u => u.RoleId,
                (r, u) => new
                {
                    r.RoleName,
                    u.UserName
                })
                .Where(r => r.RoleName == roleName)
                .Where(r => r.UserName == username)
                .Count();
            if (cnt > 0)
                result = true;

            return result;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            UsersInRoles user;
            List<UsersInRoles> users = new List<UsersInRoles>();
            foreach (string username in usernames)
            {
                users = context.UsersInRoles.Where(u => u.UserName == username).ToList();
                foreach (string r in roleNames)
                {
                    user = users.Where(u => u.RoleId == context.AppRoles.Where(a => a.RoleName == r).FirstOrDefault().RoleId)
                        .FirstOrDefault();
                    if (user != null)
                    {
                        context.UsersInRoles.Remove(user);
                    }
                }
            }
            context.SaveChanges();
        }

        public override bool RoleExists(string roleName)
        {
            bool result = false;
            int cnt = context.AppRoles
                .Where(r => r.RoleName == roleName)
                .Count();
            if (cnt > 0)
                result = true;

            return result;
        }
    }
}