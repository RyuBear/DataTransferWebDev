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
    
    public partial class bscode
    {
        public int id { get; set; }
        public string cd { get; set; }
        public string cd_descp { get; set; }
        public string cd_type { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string value1 { get; set; }
        public string value2 { get; set; }
        public string value3 { get; set; }
    
        public virtual bscode bscode1 { get; set; }
        public virtual bscode bscode2 { get; set; }
    }
}
