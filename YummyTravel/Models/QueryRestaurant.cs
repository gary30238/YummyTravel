using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YummyTravel.Models
{
    public class QueryRestaurant
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string RestaurantPhone { get; set; }
        public string RestaurantImg { get; set; }
        public double? Rating { get; set; }
        public int? MemberId { get; set; }
        public string Latlng { get; set; }
        public byte[] ImgBytes { get; set; }
        public int FoodType { get; set; }
    }
}