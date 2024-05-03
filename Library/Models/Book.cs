using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class Book
    {
        [DisplayName("書籍編號")]
        public int BookId { get; set; }
        [DisplayName("書名")]
        public string BookName { get; set; }
        [DisplayName("作者")]
        public string Author { get; set; }
        [DisplayName("圖書類別")]
        public string Category { get; set; }
        [DisplayName("圖書類別")]
        [Required]
        public string CategoryId { get; set; }
        [DisplayName("購書日期")]
        public string BoughtDate { get; set; }
        [DisplayName("借閱狀態")]
        public string Status { get; set; }
        [DisplayName("借閱狀態")]
        public string StatusId { get; set; }
        [DisplayName("借閱人")]
        public string Keeper { get; set; }
        [DisplayName("借閱人")]
        public string KeeperId { get; set; }
        [DisplayName("出版商")]
        public string Publisher { get; set; }
        [DisplayName("內容簡介")]
        public string Note { get; set; }
    }
}