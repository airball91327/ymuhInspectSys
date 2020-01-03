using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using InspectSystem.Models;
using System.Security.Cryptography;
using System.Text;
using WebMatrix.WebData;

namespace InspectSystem.Providers
{
    public class CustomMembershipProvider : ExtendedMembershipProvider
    {
        BMEDcontext context = new BMEDcontext();
        PSONcontext pcontext = new PSONcontext();
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

        public override bool EnablePasswordReset
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string PasswordStrengthRegularExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {

            string oldpwd = GetMD5Hash(oldPassword);
            AppUser userObj;
            if (Roles.IsUserInRole("Admin"))
                userObj = context.AppUsers.Where(x => x.UserName == username)
                .FirstOrDefault();
            else
                userObj = context.AppUsers.Where(x => x.UserName == username && x.Password == oldpwd)
                    .FirstOrDefault();

            if (userObj != null || Roles.IsUserInRole("Admin"))
            {
                userObj.Password = GetMD5Hash(newPassword);
                userObj.LastActivityDate = DateTime.Now;
                context.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args =
           new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            //if (RequiresUniqueEmail && !string.IsNullOrEmpty(GetUserNameByEmail(email)))
            //{
            //    status = MembershipCreateStatus.DuplicateEmail;
            //    return null;
            //}

            MembershipUser user = GetUser(username, true);

            if (user == null)
            {
                var userObj = new AppUser();
                //userObj.Id = 1;
                userObj.UserName = username;
                userObj.Password = GetMD5Hash(password);
                userObj.Email = email;
                userObj.DateCreated = DateTime.Now;
                userObj.LastActivityDate = DateTime.Now;
                userObj.Status = "Y";
                context.AppUsers.Add(userObj);
                context.SaveChanges();

                status = MembershipCreateStatus.Success;

                return GetUser(username, true);
            }
            else
            {
                status = MembershipCreateStatus.Success;
                return user;
            }

        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            var user = context.AppUsers.Where(x => x.UserName == username).FirstOrDefault();

            if (user != null)
                return user.Password;

            return "";
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {

            var user = context.AppUsers.Where(x => x.UserName == username && x.Status == "Y").FirstOrDefault();

            if (user != null)
            {
                MembershipUser memUser = new MembershipUser("CustomMembershipProvider", username, user.Id, user.Email, string.Empty, string.Empty,
                                        true, false, user.DateCreated, DateTime.MinValue, user.LastActivityDate ?? DateTime.MinValue, DateTime.UtcNow, DateTime.UtcNow);
                return memUser;
            }

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            var userObj = context.AppUsers.Where(x => x.UserName == username && x.Status == "Y").FirstOrDefault();
            if (userObj != null)
            {
                string pwd = ProductPassword();
                userObj.Password = GetMD5Hash(pwd);
                userObj.LastActivityDate = DateTime.Now;
                context.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return pwd;
            }
            return "";
        }

        private string ProductPassword()
        {
            string allowedChars = "";
            allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "1,2,3,4,5,6,7,8,9,0,!,@,#,$,%,&,?";

            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);
            string passwordString = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                passwordString += temp;
            }
            return passwordString;
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            var userObj = context.AppUsers.Where(x => x.UserName == user.UserName)
                .FirstOrDefault();

            if (userObj != null)
            {
                userObj.Email = user.Email;
                userObj.LastActivityDate = DateTime.Now;
                context.Entry(userObj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }


        }

        public override bool ValidateUser(string username, string password)
        {
            //測試用-----------------------------------------------------------------------------------
            //if (username == "admin" && password == "nckupwd")
            //    return true;
            //-----------------------------------------------------------------------------------------
            //string sha1Pswd = GetMD5Hash(password);

            var userObj = pcontext.DB_GEN_STAFF_PWs.Where(x => x.STAFFNO == username && x.PASSWORD == password)
                .FirstOrDefault();

            if (userObj != null)
                return true;

            return false;
        }

        private static string GetMD5Hash(string password)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            throw new NotImplementedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteAccount(string userName)
        {
            throw new NotImplementedException();
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
        {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public override bool IsConfirmed(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreateDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastPasswordFailureDate(string userName)
        {
            throw new NotImplementedException();
        }
    }
}