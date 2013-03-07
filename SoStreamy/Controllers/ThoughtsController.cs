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
                model.Thoughts = session.Query<Thoughts_All.Result, Thoughts_All>()
                       .OrderByDescending(x => x.Created)
                       .OfType<Thought>()
                       .Take(10)
                       .ToList();
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
