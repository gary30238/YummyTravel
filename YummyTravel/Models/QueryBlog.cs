using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YummyTravel.Models
{
    public class QueryBlog
    {
        public int? RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string RestaurantPhone { get; set; }
        public string RestaurantImg { get; set; }
        public int? MemberId { get; set; }
        public string NickName { get; set; }
        public string BlogContext { get; set; }
        public string Title { get; set; }
        public int BlogId { get; set; }
        public byte[] ImgBytes { get; set; }
    }
}