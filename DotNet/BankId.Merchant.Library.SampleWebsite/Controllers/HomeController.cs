using Microsoft.AspNetCore.Mvc;

namespace BankId.Merchant.Library.SampleWebsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(); 
        }
    }
}