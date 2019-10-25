using ItBlog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace ItBlog.Controllers
{
    public class HomeController : Controller
    {
        BlogContext db = new BlogContext();

        public async Task<ActionResult> Index(string Category)
        { 
            IQueryable<string> CategoryQuery = from m in db.Articles
                                            orderby m.Category
                                            select m.Category;
            var articles = from m in db.Articles
                         select m;
            if (!string.IsNullOrEmpty(Category))
            {
                articles = articles.Where(s => s.Category.Contains(Category));
            }
            var articleCategoryVM = new ArticleCategoryViewModel
            {
                Categories = new SelectList(await CategoryQuery.Distinct().ToListAsync()),
                Articles = await articles.ToListAsync()
            };
            ViewBag.Articles = articles;
            return View(articleCategoryVM);
        }
        [Authorize]
        [HttpGet]

        [Authorize]
        public ActionResult MyArticles()
        {
            List<Article> yourArticles = new List<Article>();
            foreach(var b in db.Articles)
            {
                if(b.UserMail==User.Identity.Name)
                {
                    yourArticles.Add(b);
                }
            }
            ViewBag.Articles = yourArticles;
            return View();
        }
        public RedirectResult YourArticle(Article article)
        {
            return Redirect($"/Home/ShowFull?id={article.ArticleId}");
        }
        [HttpGet]
        [Authorize]
        public ActionResult EditArticle(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Article article = db.Articles.Find(id);
            if (article != null)
            {
                return View(article);
            }
            return HttpNotFound();
        }
        [HttpPost]
        [Authorize]
        public RedirectResult EditArticle(Article article)
        {
            article.Time = DateTime.Now;
            article.UserMail = User.Identity.Name;
            db.Entry(article).State = EntityState.Modified;
            db.SaveChanges();
            return YourArticle(article);
        }
        [Authorize]
        public ActionResult AddArticle()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public RedirectResult AddArticle(Article article)
        {
            article.Time = DateTime.Now;
            article.UserMail = User.Identity.Name;
            db.Articles.Add(article);
            db.SaveChanges();
            return YourArticle(article);
        }
        public ActionResult ShowFull(int id)
        {
            List<Article> articles = db.Articles.ToList<Article>();
            ViewBag.Article = articles[id - 1];
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}