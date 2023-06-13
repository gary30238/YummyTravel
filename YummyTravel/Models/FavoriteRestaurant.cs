using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YummyTravel.Models
{
    public class FavoriteRestaurant
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string RestaurantPhone { get; set; }
        public string RestaurantImg { get; set; }
        public int MemberToRestaurantId { get; set; }
    }
}