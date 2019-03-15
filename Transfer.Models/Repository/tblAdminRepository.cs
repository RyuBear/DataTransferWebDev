using System;
using System.Linq;
using Transfer.Models.ViewModel;
using System.Collections.Generic;

namespace Transfer.Models.Repository
{
    public class tblAdminRepository : GenericRepository<tblAdmin>
    {
        /// <summary>
        /// 取得 系統管理員 資訊
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public tblAdmin CheckUser(string Account, string Password)
        {
            LoginData data = new LoginData();
            Password = PasswordUtility.SHA512Encryptor(Password);
            tblAdmin admin = this.Get(x => x.PersonalID.Equals(Account) && x.Password.Equals(Password) && x.UseStatus == true);
            return admin;
        }


        /// <summary>
        /// 更新密碼
        /// </summary>
        /// <returns></returns>
        public bool UptPassword(string Account, string password)
        {
            List<tblAdmin> users = this.GetSome(x => x.PersonalID.Equals(Account) && x.UseStatus == true).ToList();
            foreach (var user in users)
            {
                try
                {
                    user.Password = PasswordUtility.SHA512Encryptor(password);
                    this.Update(user);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
