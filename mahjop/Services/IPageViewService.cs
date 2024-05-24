using mahjop.Data;
using mahjop.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace mahjop.Services
{
    public interface IPageViewService
    {
        Task AddPageView(string pageName, string category, string userId);
        void ClearAllPageViews();
        int GetPageViewsCount(string pageName, string category);
    }

    public class PageViewService : IPageViewService
    {
        private readonly ApplicationDbContext _context;

        public PageViewService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddPageView(string pageName, string category, string userId)
        {
           

            _context.PageViews.Add(new PageView { PageName = pageName, Category = category, VisitTime = DateTime.Now, UserId = userId });
            await _context.SaveChangesAsync();
        }

        public int GetPageViewsCount(string pageName, string category)
        {
            return _context.PageViews.Count(p => p.PageName == pageName && p.Category == category);
        }
        public void ClearAllPageViews()
        {
            _context.PageViews.RemoveRange(_context.PageViews);
            _context.SaveChanges();
        }

    }
}
