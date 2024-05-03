using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class BookCreateArg
    {
        [DisplayName("書名")]
        [Required]
        public string BookName { get; set; }
        [DisplayName("作者")]
        [Required]
        public string Author { get; set; }
        [DisplayName("出版商")]
        [Required]
        public string Publisher { get; set; }
        [DisplayName("內容簡介")]
        [Required]
        public string Note { get; set; }
        [DisplayName("購書日期")]
        [Required]
        public string BoughtDate { get; set; }
        [DisplayName("圖書類別")]
        [Required]
        public string CategoryId { get; set; }
    }
}