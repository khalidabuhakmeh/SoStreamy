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
        public ActionResult Index()
        {
            var model = new IndexViewModel();
            using (var session = Application.DocumentStore.OpenSession())
            {
                RavenQueryStatistics stats;
                model.Thoughts = session.Query<Thoughts_All.Result, Thoughts_All>()
                       .Statistics(out stats)
                       .OrderByDescending(x => x.Created)
                       .OfType<Thought>()
                       .Take(10)
                       .ToList();

                model.TotalThoughts = stats.TotalResults;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Purge()
        {
            Application.DocumentStore
                       .DatabaseCommands
                       .DeleteByIndex("Thoughts/All", new IndexQuery { Query = "*" }, true);

            return Json(new {ok = true});
        }
    }
}
