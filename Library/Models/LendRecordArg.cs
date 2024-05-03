using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class LendRecordArg
    {
        [DisplayName("借閱日期")]
        public string LendDate { get; set; }
        [DisplayName("借閱人員編號")]
        public string UserId { get; set; }
        [DisplayName("英文姓名")]
        public string EName { get; set; }
        [DisplayName("中文姓名")]
        public string CName { get; set; }
    }
}