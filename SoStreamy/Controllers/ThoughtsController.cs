using System;
using System.Linq;
using System.Web.Mvc;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using SoStreamy.Models;

namespace SoStreamy.Controllers
{
    public class ThoughtsController : Controller
    {
        protected int Size = 10;

        public ActionResult Index()
        {
            var model = new IndexViewModel();
            using (var session = Application.DocumentStore.OpenSession())
            {
                RavenQueryStatistics stats;
                model.Thoughts = session.Query<Thoughts_All.Result, Thoughts_All>()
                       .Statistics(out stats)
                       .Where(x => x.Created <= DateTime.UtcNow)
                       .OrderByDescending(x => x.Created)
                       .OfType<Thought>()
                       .Take(Size)
                       .ToList();

                model.TotalThoughts = stats.TotalResults;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult More(int page, DateTimeOffset loaded)
        {
            using (var session = Application.DocumentStore.OpenSession())
            {
                RavenQueryStatistics stats;
                var thoughts = session.Query<Thoughts_All.Result, Thoughts_All>()
                                      .Statistics(out stats)
                                      .OrderByDescending(x => x.Created)
                                      .Where(x => x.Created <= loaded)
                                      .Skip(page * Size)
                                      .Take(Size)
                                      .OfType<Thought>()
                                      .ToList()
                                      .Select(i => new
                                      {
                                          name = i.Name,
                                          date = i.Created.ToString(),
                                          thought = i.Text
                                      });

                return Json(new { nextPage = page + 1, thoughts, ok = 1 });
            }
        }

        [HttpPost]
        public ActionResult Purge()
        {
            Application.DocumentStore
                       .DatabaseCommands
                       .DeleteByIndex("Thoughts/All", new IndexQuery { Query = "*" }, true);

            return Json(new { ok = true });
        }
    }
}
