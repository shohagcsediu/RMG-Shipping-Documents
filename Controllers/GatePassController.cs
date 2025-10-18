using Microsoft.AspNetCore.Mvc;
using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Service;
using System.Security.Claims;

namespace RMG_Shipping_Documents.Controllers
{
    public class GatepassController : Controller
    {
        private readonly GatepassService _gatepassService;

        public GatepassController(GatepassService gatepassService)
        {
            _gatepassService = gatepassService;
        }

        // GET: Gatepass
        public IActionResult Index()
        {
            var gatepasses = _gatepassService.GetAll();
            return View(gatepasses);
        }

        // GET: Gatepass/Create
        public IActionResult Create()
        {
            var model = new Gatepass
            {
                Date = DateTime.Today
            };

            ViewBag.TruckNumbers = _gatepassService.GetUniqueTruckNumbers();
            return View("CreateEdit", model);
        }

        // POST: Gatepass/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Gatepass model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set created by
                    model.CreatedBy = User.FindFirstValue(ClaimTypes.Name);

                    // Insert the gatepass
                    _gatepassService.Insert(model);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating gatepass: " + ex.Message);
                }
            }

            return View("CreateEdit", model);
        }

        // GET: Gatepass/Edit/5
        public IActionResult Edit(int id)
        {
            var gatepass = _gatepassService.GetById(id);
            if (gatepass == null)
            {
                return NotFound();
            }

            ViewBag.TruckNumbers = _gatepassService.GetUniqueTruckNumbers();
            return View("CreateEdit", gatepass);
        }

        // POST: Gatepass/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Gatepass model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set modified by
                    model.ModifiedBy = User.FindFirstValue(ClaimTypes.Name);

                    // Update the gatepass
                    _gatepassService.Update(model);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating gatepass: " + ex.Message);
                }
            }

            return View("CreateEdit", model);
        }

        // GET: Gatepass/Details/5
        public IActionResult Details(int id)
        {
            var gatepass = _gatepassService.GetById(id);
            if (gatepass == null)
            {
                return NotFound();
            }

            return View(gatepass);
        }

        // GET: Gatepass/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var gatepass = _gatepassService.GetById(id);
            if (gatepass == null)
                return Json(new { success = false, message = "Gatepass not found" });

            _gatepassService.Delete(id);
            return Json(new { success = true });
        }


        // POST: Gatepass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _gatepassService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error deleting gatepass: " + ex.Message);
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        // POST: Gatepass/DeleteAjax/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAjax(int id)
        {
            try
            {
                var gatepass = _gatepassService.GetById(id);
                if (gatepass == null)
                {
                    return Json(new { success = false, message = "Gatepass not found" });
                }

                _gatepassService.Delete(id);
                return Json(new { success = true, message = "Gatepass deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting gatepass: " + ex.Message });
            }
        }

        // GET: Gatepass/GetTruckDetails
        [HttpGet]
        public IActionResult GetTruckDetails(string truckNo)
        {
            try
            {
                var lastChallan = _gatepassService.GetLastDeliveryChallanForTruck(truckNo);
                if (lastChallan == null)
                {
                    return Json(new { success = false });
                }

                return Json(new
                {
                    success = true,
                    driverName = lastChallan.Driver,
                    transportCompany = lastChallan.TransportCompany,
                    mobileNo = lastChallan.MobileNo,
                    licenseNo = lastChallan.LicenseNo
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Gatepass/GetDeliveryChallansForTruck
        [HttpGet]
        public IActionResult GetDeliveryChallansForTruck(string truckNo)
        {
            try
            {
                var challans = _gatepassService.GetDeliveryChallansForTruck(truckNo);
                return PartialView("_DeliveryChallanList", challans);
            }
            catch (Exception ex)
            {
                return PartialView("_DeliveryChallanList", new List<DeliveryChallan>());
            }
        }

        // GET: Gatepass/GetDetails/5
        [HttpGet]
        
        public IActionResult GetDetails(int id)
        {
            var gatepass = _gatepassService.GetById(id);
            if (gatepass == null)
                return Json(new { success = false });

            // Generate QR code as base64 image using QRCoder
            string qrCodeBase64 = "";
            using (var qrGenerator = new QRCoder.QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode($"{gatepass.GatePassNo}|{gatepass.Date:yyyy-MM-dd}|{gatepass.CompanyName}|{gatepass.IssuedTo}", QRCoder.QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCoder.PngByteQRCode(qrCodeData))
                {
                    var qrBytes = qrCode.GetGraphic(20);
                    qrCodeBase64 = "data:image/png;base64," + Convert.ToBase64String(qrBytes);
                }
            }

            return Json(new
            {
                success = true,
                gatepass = new
                {
                    gatePassNo = gatepass.GatePassNo,
                    date = gatepass.Date,
                    companyName = gatepass.CompanyName,
                    issuedTo = gatepass.IssuedTo,
                    qrCode = qrCodeBase64
                }
            });
        }

    }
}