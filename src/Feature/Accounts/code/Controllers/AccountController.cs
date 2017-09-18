using System.Web.Mvc;

namespace XC.Feature.Accounts.Controllers
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