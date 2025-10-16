using Microsoft.AspNetCore.Mvc;

namespace RMG_Shipping_Documents.Controllers
{
    public class PackingListController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
