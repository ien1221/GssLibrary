using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class BookSearchArg
    {
        [DisplayName("書本ID")]
        public int BookId { get; set; }
        [DisplayName("書名")]
        public string BookName { get; set; }
        [DisplayName("圖書類別")]
        public string CategoryId { get; set; }
        [DisplayName("借閱人")]
        public string KeeperId { get; set; }
        [DisplayName("借閱狀態")]
        public string StatusId { get; set; }
    }
}