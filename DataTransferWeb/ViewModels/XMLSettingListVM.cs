using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Transfer.Models;

namespace DataTransferWeb.ViewModels
{
    public class XMLSettingListVM
    {
        [Display(Name = "XML Name")]
        public string XMLName { get; set; }
        
        [Display(Name = "SQL Name")]
        public string SQLName { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "XML Setting List")]

        public IEnumerable<tblXMLSetting> settings { get; set; }
    }
}
