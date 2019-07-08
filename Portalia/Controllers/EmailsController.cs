using System.Web.Mvc;
using Portalia.Models;

namespace Portalia.Controllers
{
    public class EmailsController : Controller
    {
        public ActionResult Index()
        {
            return PartialView("Welcomeemailtemplate", new RegisterViewModel { Email = "test", Password = "test" });
            
        }
    }
}