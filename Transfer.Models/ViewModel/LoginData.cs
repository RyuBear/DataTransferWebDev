using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transfer.Models.ViewModel
{
    public class LoginData
    {
        public LoginData()
        {
            //Exams = new List<Registration>();
            Exams = new List<string>();
        }


        [Required]
        public string Guid { get; set; }


        [Required]
        [Display(Name = "ID")]
        public string ID { get; set; }

        [Required]
        [Display(Name = "StuName")]
        public string Name { get; set; }

        //[Required]
        //[Display(Name = "認證類別")]
        //public string ClassNo { get; set; }

        [Required]
        [Display(Name = "BeforeNote")]
        public string BeforeTestNote { get; set; }

        [Required]
        [Display(Name = "ExamSubject")]
        public List<string> Exams { get; set; }
    }
}
