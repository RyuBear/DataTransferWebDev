﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Transfer.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TransferDBEntities : DbContext
    {
        public TransferDBEntities()
            : base("name=TransferDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblSQLColumns> tblSQLColumns { get; set; }
        public virtual DbSet<tblSQLSetting> tblSQLSetting { get; set; }
        public virtual DbSet<tblAdmin> tblAdmin { get; set; }
        public virtual DbSet<tblXMLMapping> tblXMLMapping { get; set; }
        public virtual DbSet<tblExcelSetting> tblExcelSetting { get; set; }
        public virtual DbSet<tblXMLSetting> tblXMLSetting { get; set; }
        public virtual DbSet<tblLog> tblLog { get; set; }
        public virtual DbSet<tblSMTP> tblSMTP { get; set; }
        public virtual DbSet<bscode> bscode { get; set; }
        public virtual DbSet<bscode_kind> bscode_kind { get; set; }
        public virtual DbSet<tblExcelMapping> tblExcelMapping { get; set; }
    }
}
