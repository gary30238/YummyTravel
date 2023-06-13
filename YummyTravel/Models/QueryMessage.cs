using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YummyTravel.Models
{
    public class QueryMessage
    {
        public double? Rating { get; set; }
        public int? MemberId { get; set; }
        public string Message { get; set; }
        public string MemberAccount { get; set; }        
        public string Nickname { get; set; }
        public byte[] Image { get; set; }
    }
}