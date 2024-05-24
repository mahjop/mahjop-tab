using System;

namespace mahjop.Models
{
    public class PageView
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public string Category { get; set; }
        public DateTime VisitTime { get; set; }
        public string UserId { get; set; } // معرف المستخدم
    }
}
