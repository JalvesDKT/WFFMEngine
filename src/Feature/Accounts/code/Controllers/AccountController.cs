using System.Web.Mvc;

namespace DKT.Feature.Accounts.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
    }
}