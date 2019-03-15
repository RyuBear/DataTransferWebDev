//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class tblSQLSetting
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblSQLSetting()
        {
            this.tblSQLColumns = new HashSet<tblSQLColumns>();
            this.tblXMLSetting = new HashSet<tblXMLSetting>();
            this.tblExcelSetting = new HashSet<tblExcelSetting>();
        }
    
        public string SQLName { get; set; }
        public string SQLStatement { get; set; }
        public int DataRow { get; set; }
        public string SQLType { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string Creator { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string Updator { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSQLColumns> tblSQLColumns { get; set; }
        public virtual tblSQLSetting tblSQLSetting1 { get; set; }
        public virtual tblSQLSetting tblSQLSetting2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblXMLSetting> tblXMLSetting { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblExcelSetting> tblExcelSetting { get; set; }
    }
}
