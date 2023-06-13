using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YummyTravel.Models;
using PagedList;
using System.Net.Mail;
using System.IO;
using System.Data.Entity.Validation;
using System.Net;
using Newtonsoft.Json;

namespace YummyTravel.Controllers
{
    public class HomeController : Controller
    {

        FoodTourDBEntities db = new FoodTourDBEntities();
        private int pageSize = 5;
        // GET: Home
        public ActionResult Index()
        {
            //排名前三名餐廳
            var queryTopRestaurant = from o in db.Restaurants
                                     orderby o.Cost descending
                                     select o;
            ViewBag.topRestaurant = queryTopRestaurant.ToList();
            //隨機餐廳
            var query = from o in db.Restaurants
                        orderby Guid.NewGuid()
                        select o;
            var rest = query.ToList();
            return View(rest);
        }

        [HttpPost]
        public ActionResult Index(string tags)
        {
            //搜尋標籤
            TempData["tags"] = tags;
            return RedirectToAction("Page2");
        }

        public ActionResult Page2(string zone, int? id, int? page = 1)
        {
            TempData["id"] = id;
            TempData["zone"] = zone;
            if (zone == null)
            {
                zone = "";
            }
            if (id == null)
            {
                id = 0;
            }
            //搜尋標籤
            string tags = "";
            int memberId = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(Session["memberId"])))
            {
                memberId = Convert.ToInt32(Session["memberID"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(TempData["tags"]))) //判斷標籤
            {
                tags = Convert.ToString(TempData["tags"]);
                TempData.Keep("tags");
            }

            if (id != 0 && !string.IsNullOrEmpty(zone))
            {
                //分頁
                ViewBag.page = page;
                int currentPage = Convert.ToInt32(page) < 1 ? 1 : Convert.ToInt32(page);
                var query = from o in db.Restaurants
                            join t in db.RestaurantToTypes
                            on o.RestaurantId equals t.RestaurantId
                            join p in db.MemberToRestaurants.Where(x => x.MemberId == memberId)
                            on o.RestaurantId equals p.RestaurantId
                            into op
                            from p in op.DefaultIfEmpty()
                            where t.TypeId == id && o.RestaurantAddress.Contains(zone) && o.RestaurantName.Contains(tags)
                            orderby o.RestaurantId
                            select new QueryRestaurant
                            {
                                RestaurantId = o.RestaurantId,
                                RestaurantName = o.RestaurantName,
                                RestaurantAddress = o.RestaurantAddress,
                                RestaurantPhone = o.RestaurantPhone,
                                RestaurantImg = o.RestaurantImg,
                                Rating = o.Rating,
                                Latlng = o.Latlng,
                                MemberId = p.MemberId
                            };
                var dataRestaurant = query.ToPagedList(currentPage, pageSize);
                return View(dataRestaurant);
            }else if (id == 0 && !string.IsNullOrEmpty(zone))
            {
                //分頁
                ViewBag.page = page;
                int currentPage = Convert.ToInt32(page) < 1 ? 1 : Convert.ToInt32(page);
                var query = from o in db.Restaurants
                            join p in db.MemberToRestaurants.Where(x => x.MemberId == memberId)
                            on o.RestaurantId equals p.RestaurantId
                            into op
                            from p in op.DefaultIfEmpty()
                            where o.RestaurantAddress.Contains(zone) && o.RestaurantName.Contains(tags)
                            orderby o.RestaurantId
                            select new QueryRestaurant
                            {
                                RestaurantId = o.RestaurantId,
                                RestaurantName = o.RestaurantName,
                                RestaurantAddress = o.RestaurantAddress,
                                RestaurantPhone = o.RestaurantPhone,
                                RestaurantImg = o.RestaurantImg,
                                Rating = o.Rating,
                                Latlng = o.Latlng,
                                MemberId = p.MemberId
                            };
                var dataRestaurant = query.ToPagedList(currentPage, pageSize);
                return View(dataRestaurant);
            }else if (string.IsNullOrEmpty(zone) && id != 0)
            {
                //分頁
                ViewBag.page = page;
                int currentPage = Convert.ToInt32(page) < 1 ? 1 : Convert.ToInt32(page);
                var query = from o in db.Restaurants
                            join t in db.RestaurantToTypes
                            on o.RestaurantId equals t.RestaurantId
                            join p in db.MemberToRestaurants.Where(x => x.MemberId == memberId)
                            on o.RestaurantId equals p.RestaurantId
                            into op
                            from p in op.DefaultIfEmpty()
                            where t.TypeId == id && o.RestaurantName.Contains(tags)
                            orderby o.RestaurantId
                            select new QueryRestaurant
                            {
                                RestaurantId = o.RestaurantId,
                                RestaurantName = o.RestaurantName,
                                RestaurantAddress = o.RestaurantAddress,
                                RestaurantPhone = o.RestaurantPhone,
                                RestaurantImg = o.RestaurantImg,
                                Rating = o.Rating,
                                Latlng = o.Latlng,
                                MemberId = p.MemberId
                            };
                var dataRestaurant = query.ToPagedList(currentPage, pageSize);
                return View(dataRestaurant);
            }
            else
            {
                //分頁
                ViewBag.page = page;
                int currentPage = Convert.ToInt32(page) < 1 ? 1 : Convert.ToInt32(page);
                var query = from o in db.Restaurants
                            join p in db.MemberToRestaurants.Where(x => x.MemberId == memberId)
                            on o.RestaurantId equals p.RestaurantId
                            into op
                            from p in op.DefaultIfEmpty()
                            where o.RestaurantName.Contains(tags)
                            orderby o.RestaurantId
                            select new QueryRestaurant
                            {
                                RestaurantId = o.RestaurantId,
                                RestaurantName = o.RestaurantName,
                                RestaurantAddress = o.RestaurantAddress,
                                RestaurantPhone = o.RestaurantPhone,
                                RestaurantImg = o.RestaurantImg,
                                Rating = o.Rating,
                                Latlng = o.Latlng,
                                MemberId = p.MemberId
                            };
                var dataRestaurant = query.ToPagedList(currentPage, pageSize);
                return View(dataRestaurant);
            }
        }

        //[HttpPost]
        //public ActionResult Page2(string tags, int page = 1)
        //{
        //    //搜尋標籤 & 分頁
        //    tags = Convert.ToString(TempData["tags"]);
        //    TempData["tags"] = tags;
        //    TempData.Keep("tags");
        //    int currentPage = 1;
        //    if (tags == Convert.ToString(TempData["tags"]))
        //    {
        //        currentPage = page < 1 ? 1 : page;
        //    }
        //    var query = from o in db.Restaurants
        //                join p in db.MemberToRestaurants
        //                on o.RestaurantId equals p.RestaurantId
        //                into ps
        //                from p in ps.DefaultIfEmpty()
        //                where o.RestaurantName.Contains(tags)
        //                orderby o.RestaurantId
        //                select new QueryRestaurant
        //                {
        //                    RestaurantId = o.RestaurantId,
        //                    RestaurantName = o.RestaurantName,
        //                    RestaurantAddress = o.RestaurantAddress,
        //                    RestaurantPhone = o.RestaurantPhone,
        //                    RestaurantImg = o.RestaurantImg,
        //                    Rating = o.Rating,
        //                    Latlng = o.Latlng,
        //                    MemberId = p.MemberId
        //                };
        //    var dataRestaurant = query.ToPagedList(currentPage, pageSize);
        //    ViewBag.page = page;
        //    return View(dataRestaurant);
        //}

        [HttpPost]
        public void Favorite(int account, int favoriteId)
        {
            //喜愛店家
            try
            {
                var query = from o in db.MemberToRestaurants
                            where o.RestaurantId == favoriteId
                            && o.MemberId == account
                            select o;
                if (query.Count() < 1)
                {
                    var newMemberToRestaurant = new MemberToRestaurant
                    {
                        MemberId = account,
                        RestaurantId = favoriteId
                    };
                    db.MemberToRestaurants.Add(newMemberToRestaurant);
                }
                else
                {
                    db.MemberToRestaurants.Remove(query.SingleOrDefault());
                }
                db.SaveChanges();
            }
            catch
            {

            }
        }

        public ActionResult Page3(int? id)
        {
            //隨機相似餐廳
            var query = from o in db.Restaurants where o.RestaurantId == id select o;
            var dataRestaurant = query.SingleOrDefault();
            ViewBag.similarRestaurant = GetSimilarRestaurant(id);
            var queryMessage = from o in db.Messages
                               join p in db.Members
                               on o.MemberId equals p.MemberId
                               where o.RestaurantId == id
                               select new QueryMessage
                               {
                                   MemberId = o.MemberId,
                                   Rating = o.Rating,
                                   MemberAccount = p.Account,
                                   Message = o.Context,
                                   Nickname = p.Nickname,
                                   Image = p.Image
                               };
            var queryBlog = from o in db.Blogs
                            join p in db.Members
                            on o.MemberId equals p.MemberId
                            where o.RestaurantId == id
                            select new QueryBlog
                            {
                                BlogId = o.BlogId,
                                ImgBytes = p.Image,
                                Title = o.Title,
                                NickName = p.Nickname
                            };
            var queryType = from o in db.FoodTypes
                            join p in db.RestaurantToTypes
                            on o.TypeId equals p.TypeId
                            join t in db.Restaurants
                            on p.RestaurantId equals t.RestaurantId
                            where t.RestaurantId == id
                            select o;
            var queryRandomType = from o in db.FoodTypes
                                  join p in db.RestaurantToTypes
                                  on o.TypeId equals p.TypeId
                                  select new QueryRandomType
                                  {
                                      TypeId = o.TypeId,
                                      RestaurantId = p.RestaurantId,
                                      TypeName = o.TypeName
                                  };
            if (!string.IsNullOrEmpty(Convert.ToString(Session["str"])))
            {
                var memberId = Convert.ToInt32(Session["memberID"]);
                var queryMember = from o in db.Members
                                  where o.MemberId == memberId
                                  select o;
                ViewBag.member = queryMember.ToList();
            }
            ViewBag.message = queryMessage.ToList();
            ViewBag.blog = queryBlog.ToList();
            ViewBag.type = queryType.ToList();
            ViewBag.randomType = queryRandomType.ToList();
            
            return View(dataRestaurant);
        }

        //相似餐廳func
        public List<Restaurant> GetSimilarRestaurant(int? id)
        {
            //select distinct r.restaurantid, r.restaurantname, r.restaurantaddress, r.restaurantphone, r.restaurantimg, r.rating from restaurant r inner join restauranttotype t on r.restaurantid = t.restaurantid where t.typeid in (select typeid from restaurant r inner join restauranttotype t on r.restaurantid = t.restaurantid where = 2)
            var queryTypeId = from o in db.RestaurantToTypes where o.RestaurantId == id select o.TypeId;
            var typeId = queryTypeId.ToList()[0];
            var queryRestaurantId = from o in db.RestaurantToTypes where o.TypeId == typeId select o.RestaurantId;
            var randomRestaurant = new List<int?>();
            Random crandom = new Random();
            for (int i = 0; i < 7; i++)
            {
                var num = crandom.Next(queryRestaurantId.Count());
                randomRestaurant.Add(queryRestaurantId.ToList()[num]);
            }
            var queryRandomRestaurant = from o in db.Restaurants where randomRestaurant.Contains(o.RestaurantId) select o;
            return queryRandomRestaurant.ToList();
        }

        [HttpPost]
        public void Message(int restaurantId, int memberId, string context, double score)
        {
            try
            {
                var query = from o in db.Messages
                            where o.RestaurantId == restaurantId
                            && o.MemberId == memberId
                            select o;
                if (query.Count() < 1)
                {
                    var newMessage = new Message
                    {
                        RestaurantId = restaurantId,
                        MemberId = memberId,
                        Context = context,
                        Rating = score
                    };
                    db.Messages.Add(newMessage);
                }
                else
                {
                    foreach (var item in query.ToList())
                    {
                        item.Context = context;
                        item.Rating = score;
                    }
                }
                db.SaveChanges();
            }
            catch
            {

            }
        }

        public ActionResult AdminReview()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["memberID"])))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var query = from o in db.NewRestaurants select o;
                var list = query.ToList();
                return View(list);
            }
        }

        [HttpPost]
        public ActionResult ReviewFail(string review, int id)
        {
            var query = from o in db.NewRestaurants where o.newId == id select o;
            var restaurant = query.SingleOrDefault();
            restaurant.Review = review;
            db.SaveChanges();
            return Json(Url.Action("AdminReview", "Home"));
        }

        [HttpPost]
        public ActionResult ReviewSuccess(string review, int id)
        {
            string result = "";
            var query = from o in db.NewRestaurants where o.newId == id select o;
            var restaurant = query.SingleOrDefault();
            restaurant.Review = review;
            string FindplaceUrl = "https://maps.googleapis.com/maps/api/place/findplacefromtext/json?inputtype=textquery&fields=place_id,geometry&key=API_KEY";
            string DetailsUrl = "https://maps.googleapis.com/maps/api/place/details/json?fields=rating,formatted_phone_number,opening_hours&key=API_KEY&language=zh-TW";
            string address = restaurant.RestaurantAddress;
            FindplaceUrl += $"&input={address}";
            string placeId = GetplaceId(FindplaceUrl, result).candidates[0].place_id;
            DetailsUrl += $"&place_id={placeId}";
            string latlng = GetplaceId(FindplaceUrl, result).candidates[0].geometry.location.lat + "," + GetplaceId(FindplaceUrl, result).candidates[0].geometry.location.lng;
            var newRestaurant = new Restaurant
            {
                RestaurantName = restaurant.RestaurantName,
                RestaurantAddress = restaurant.RestaurantAddress,
                RestaurantPhone = restaurant.RestaurantPhone,
                Mon = restaurant.Mon,
                Tue = restaurant.Tue,
                Wed = restaurant.Wed,
                Thu = restaurant.Thu,
                Fri = restaurant.Fri,
                Sat = restaurant.Sat,
                Sun = restaurant.Sun,
                Rating = 0,
                Latlng = latlng,
                PlaceId = placeId,
                Type = "N",
                Cost = 0,
                RestaurantImg = restaurant.RestaurantImg
            };
            db.Restaurants.Add(newRestaurant);
            db.SaveChanges();
            var num = (from o in db.Restaurants select o.RestaurantId).Max();
            var newRestaurantToType = new RestaurantToType
            {
                RestaurantId = num,
                TypeId = 10
            };
            db.RestaurantToTypes.Add(newRestaurantToType);
            db.SaveChanges();
            return Json(Url.Action("AdminReview", "Home"));
        }
        public GoogleFindplaceResponse GetplaceId(string url, string result)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = request.GetResponse())
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<GoogleFindplaceResponse>(result);
        }

        public GoogleDetailsResponse GetDetails(string url, string result)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = request.GetResponse())
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<GoogleDetailsResponse>(result);
        }

        public class GoogleFindplaceResponse
        {

            public string status { get; set; }
            public candidates[] candidates { get; set; }
        }

        public class candidates
        {
            public string place_id { get; set; }
            public geometry geometry { get; set; }
        }

        public class geometry
        {
            public location location { get; set; }
        }

        public class location
        {
            public string lat { get; set; }
            public string lng { get; set; }
        }

        public class GoogleDetailsResponse
        {
            public result result { get; set; }
            public string status { get; set; }
        }

        public class result
        {
            public string formatted_phone_number { get; set; }
            public double rating { get; set; }
            public opening_hours opening_hours { get; set; }
        }

        public class opening_hours
        {
            public string[] weekday_text { get; set; }
        }

        public ActionResult NewRestaurant()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["memberID"])))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult NewRestaurant(string name, string address, string phone, HttpPostedFileBase img, string monStartTime, string monEndTime, string tueStartTime, string tueEndTime, string wedStartTime, string wedEndTime, string thuStartTime, string thuEndTime, string friStartTime, string friEndTime, string satStartTime, string satEndTime, string sunStartTime, string sunEndTime, int typeId)
        {
            TempData["name"] = name;
            TempData["address"] = address;
            TempData["phone"] = phone;
            TempData["img"] = img;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone) || img == null && img.ContentLength == 0 || string.IsNullOrEmpty(monStartTime) || string.IsNullOrEmpty(monEndTime) || string.IsNullOrEmpty(tueStartTime) || string.IsNullOrEmpty(tueEndTime) || string.IsNullOrEmpty(wedStartTime) || string.IsNullOrEmpty(wedEndTime) || string.IsNullOrEmpty(thuStartTime) || string.IsNullOrEmpty(thuEndTime) || string.IsNullOrEmpty(friStartTime) || string.IsNullOrEmpty(friEndTime) || string.IsNullOrEmpty(satStartTime) || string.IsNullOrEmpty(satEndTime) || string.IsNullOrEmpty(sunStartTime) || string.IsNullOrEmpty(sunEndTime))
            {
                TempData["text"] = "上傳失敗，請檢查欄位資訊是否正確";
                return View();
            }
            else
            {
                //存到資料夾
                var FileName = Path.GetFileName(img.FileName);
                var FilePath = Path.Combine(Server.MapPath("~/img/"), FileName);
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                img.SaveAs(FilePath);

                ////轉成byte 方法一 直接轉
                //byte[] FileBytes;
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    img.InputStream.CopyTo(ms);
                //    FileBytes = ms.GetBuffer();
                //}
                var newStore = new NewRestaurant
                {
                    MemberId = Convert.ToInt32(Session["memberID"]),
                    RestaurantName = name,
                    RestaurantAddress = address,
                    RestaurantPhone = phone,
                    RestaurantImg = $"{baseUrl}/img/{FileName}",
                    //ImgBytes = FileBytes,
                    Mon = monStartTime + " - " + monEndTime,
                    Tue = tueStartTime + " - " + tueEndTime,
                    Wed = wedStartTime + " - " + wedEndTime,
                    Thu = thuStartTime + " - " + thuEndTime,
                    Fri = friStartTime + " - " + friEndTime,
                    Sat = satStartTime + " - " + satEndTime,
                    Sun = sunStartTime + " - " + sunEndTime,
                    Review = "review",
                    TypeId = typeId
                };
                db.NewRestaurants.Add(newStore);
                db.SaveChanges();
                TempData["text"] = "上傳成功，審核時間1~3天";
                return View();
            }
        }

        public ActionResult MyRestaurant()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["memberID"])))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var memberId = Convert.ToInt32(Session["memberID"]);
                var query = from o in db.NewRestaurants where o.MemberId == memberId select o;
                var list = query.ToList();
                var query2 = from o in db.NewRestaurants where o.MemberId == memberId && o.Review == "success" select o;
                var list2 = query.ToList();
                ViewBag.myRestaurant = list2;
                return View(list);
            }
        }

        public ActionResult NewBlog(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["memberID"])))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var query = from o in db.Restaurants where o.RestaurantId == id select o;
                var restaurant = query.SingleOrDefault();
                return View(restaurant);
            }
        }

        [HttpPost]
        public ActionResult NewBlog(string title, string context, int memberId, int restaurantId)
        {
            try
            {
                var query = from o in db.Blogs where o.MemberId == memberId && o.RestaurantId == restaurantId select o;
                if (query.Count() < 1)
                {
                    var newBlog = new Blog
                    {
                        Title = title,
                        RestaurantId = restaurantId,
                        MemberId = memberId,
                        BlogContext = context
                    };
                    db.Blogs.Add(newBlog);
                }
                else
                {
                    foreach (var item in query.ToList())
                    {
                        item.Title = title;
                        item.BlogContext = context;
                    }
                }
                db.SaveChanges();
                return Json(Url.Action("Member", "Home"));
            }
            catch
            {
                return Json(Url.Action("Member", "Home"));
            }
        }

        public ActionResult EditBlog(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["memberID"])))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var query = from o in db.Blogs where o.BlogId == id select o;
                var list = query.SingleOrDefault();
                return View(list);
            }
        }

        public ActionResult RemoveBlog(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["memberID"])))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var query = from o in db.Blogs where o.BlogId == id select o;
                db.Blogs.Remove(query.SingleOrDefault());
                db.SaveChanges();
                return RedirectToAction("Member");
            }
        }

        public ActionResult BlogText(int? id)
        {
            var query = from o in db.Blogs where o.BlogId == id select o;
            var list = query.ToList();
            return View(list);
        }

        public ActionResult Member()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["memberID"])))
            {
                return RedirectToAction("Index");
            }
            else
            {
                var memberId = Convert.ToInt32(Session["memberID"]);
                var query = from o in db.Restaurants
                            join p in db.MemberToRestaurants on o.RestaurantId equals p.RestaurantId
                            where p.MemberId == memberId
                            select new FavoriteRestaurant
                            {
                                RestaurantId = o.RestaurantId,
                                RestaurantAddress = o.RestaurantAddress,
                                RestaurantName = o.RestaurantName,
                                RestaurantPhone = o.RestaurantPhone,
                                RestaurantImg = o.RestaurantImg,
                                MemberToRestaurantId = p.MemberToRestaurantId
                            };
                var queryBlog = from o in db.Restaurants
                                join p in db.Blogs on o.RestaurantId equals p.RestaurantId
                                where p.MemberId == memberId
                                select new QueryBlog
                                {
                                    RestaurantName = o.RestaurantName,
                                    BlogContext = p.BlogContext,
                                    BlogId = p.BlogId
                                };
                var queryMember = from o in db.Members
                                  where o.MemberId == memberId
                                  select o;
                ViewBag.member = queryMember.ToList();
                ViewBag.blog = queryBlog.ToList();
                var list = query.ToList();
                return View(list);
            }
        }

        public ActionResult FavoriteDelete(int? id)
        {
            //喜愛店家
            try
            {
                var memberId = Convert.ToInt32(Session["memberID"]);
                var query = from o in db.MemberToRestaurants
                            where o.RestaurantId == id
                            && o.MemberId == memberId
                            select o;
                db.MemberToRestaurants.Remove(query.SingleOrDefault());
                db.SaveChanges();
                return RedirectToAction("Member");
            }
            catch
            {
                return RedirectToAction("Member");
            }
        }

        [HttpPost]
        public ActionResult LogIn(string actionResult, string account, string password)
        {
            string url = $"/Home/{actionResult}";
            //登入
            var qurey = from o in db.Members
                        where o.Account == account & o.Password == password
                        select o;
            if (qurey.FirstOrDefault() == null)
            {
                TempData["message"] = "帳號或密碼輸入錯誤";
                Session["tempaccount"] = account;
                return Redirect(url);
            }
            else
            {
                Member data = qurey.FirstOrDefault();
                Session["memberImg"] = data.Image;
                Session["memberID"] = data.MemberId;
                Session["str"] = Convert.ToString(data.Nickname);
                return Redirect(url);
            }
        }
        [HttpPost]
        public ActionResult LogOut()
        {
            //登出
            Session["str"] = "遊客";
            Session["memberID"] = "";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SignIn(Member newMember, string actionResult, string account, string password, string checkpwdinput)
        {
            string url = $"/Home/{actionResult}";
            if (password != checkpwdinput)
            {
                TempData["message"] = "密碼不一致";
                Session["tempaccount"] = account;
                return Redirect(url);
            }
            var qurey = from o in db.Members
                        where o.Account == account
                        select o;
            try
            {
                Member data = qurey.Single();
                TempData["message"] = "帳號已被註冊過了";
                return Redirect(url);
            }
            catch (Exception)
            {
                db.Members.Add(newMember);
                db.SaveChanges();
                TempData["message"] = "帳號新增完成";
                Member data = qurey.FirstOrDefault();
                Session["memberID"] = data.MemberId;
                Session["str"] = Convert.ToString(data.Nickname);
                return Redirect(url);
            }
        }
        [HttpPost]
        public ActionResult Forgot(string account, string actionResult)
        {
            string url = $"/Home/{actionResult}";
            var qurey = from o in db.Members
                        where o.Account == account
                        select o;
            if (qurey.FirstOrDefault() == null)
            {
                TempData["message"] = "查無此帳號";
                return Redirect(url);
            }
            else
            {
                Member data = qurey.Single();
                MailMessage msg = new MailMessage();
                msg.To.Add(data.Account);
                //msg.To.Add("b@b.com");可以發送給多人
                //msg.CC.Add("c@c.com");
                //msg.CC.Add("c@c.com");可以抄送副本給多人 
                //這裡可以隨便填，不是很重要
                msg.From = new MailAddress("yummytravelgg@gmail.com", "YummyTravel", System.Text.Encoding.UTF8);
                /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
                msg.Subject = "YummyTravel--密碼郵件";//郵件標題
                msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼
                msg.Body = "您的密碼是:" + data.Password; //郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                                                             //msg.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
                msg.IsBodyHtml = true;//是否是HTML郵件 
                                      //msg.Priority = MailPriority.High;//郵件優先級 
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("yummytravelgg@gmail.com", "123!@#qwe"); //這裡要填正確的帳號跟密碼
                client.Host = "smtp.gmail.com"; //設定smtp Server
                client.Port = 25; //設定Port
                client.EnableSsl = true; //gmail預設開啟驗證
                client.Send(msg); //寄出信件
                client.Dispose();
                msg.Dispose();
                TempData["message"] = "密碼郵件已寄送成功，請至信箱查看！";
                return Redirect(url);
            }
        }
        [HttpPost]
        public ActionResult MemberInfo(HttpPostedFileBase img, string actionResult, string account, string password, string checkpwdinput, string nickname)
        {
            string url = $"/Home/{actionResult}";

            var qurey = from o in db.Members
                        where o.Account == account
                        select o;
            Member data = qurey.FirstOrDefault();
            try
            {
                //轉成byte 方法一 直接轉
                byte[] FileBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    img.InputStream.CopyTo(ms);
                    FileBytes = ms.GetBuffer();
                }
                data.Image = FileBytes;
            }
            catch (Exception)
            {
            }
            //修改密碼
            if (!string.IsNullOrEmpty(password))
            {
                if (password != checkpwdinput)
                {
                    TempData["message"] = "密碼不一致";
                    Session["tempaccount"] = account;
                    return Redirect(url);
                }
                data.Password = password;
            }
            //修改暱稱
            if (!string.IsNullOrEmpty(nickname))
            {
                data.Nickname = nickname;
                Session["str"] = nickname;
            }
            db.SaveChanges();
            TempData["message"] = "資訊更新完成";
            return Redirect(url);
        }
    }
}