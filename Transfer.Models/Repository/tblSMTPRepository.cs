namespace Transfer.Models.Repository
{
    public class tblSMTPRepository : GenericRepository<tblSMTP>
    {
        /// <summary>
        /// 取得 Mail Server 資訊
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public tblSMTP get(string mail)
        {
            tblSMTP smtp = this.Get(x => x.Email.Equals(mail));
            return smtp;
        }
    }
}
