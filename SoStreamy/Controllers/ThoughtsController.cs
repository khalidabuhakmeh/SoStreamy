using System.Web.Mvc;
using Raven.Abstractions.Data;

namespace SoStreamy.Controllers
{
    public class ThoughtsController : Controller
    {
        public ActionResult Index()
        {
            return View();
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
