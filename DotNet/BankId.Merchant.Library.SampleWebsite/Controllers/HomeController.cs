using Microsoft.AspNetCore.Mvc;

namespace BankId.Merchant.Library.SampleWebsite.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View(); 
        }
    }
}
