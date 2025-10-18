using Microsoft.AspNetCore.Mvc;
using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Service;
using System.Security.Claims;

namespace RMG_Shipping_Documents.Controllers
{
    public class DeliveryChallanController : Controller
    {
        private readonly DeliveryChallanService _deliveryChallanService;

        public DeliveryChallanController(DeliveryChallanService deliveryChallanService)
        {
            _deliveryChallanService = deliveryChallanService;
        }

        // GET: DeliveryChallan index
        public IActionResult Index()
        {
            var challans = _deliveryChallanService.GetAll();
            return View(challans);
        }

        // GET: DeliveryChallan/Create
        public IActionResult Create()
        {
            var model = new DeliveryChallan
            {
                Date = DateTime.Today,
                Status = "Pending"
            };

            return View("CreateEdit", model);
        }

        // POST: DeliveryChallan/Create to create a delivery challan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DeliveryChallan model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set created by
                    model.CreatedBy = User.FindFirstValue(ClaimTypes.Name);

                    // Insert the challan
                    _deliveryChallanService.Insert(model);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating delivery challan: " + ex.Message);
                }
            }

            return View("CreateEdit", model);
        }

        // GET: DeliveryChallan/Edit/5 valid check and move to ui
        public IActionResult Edit(int id)
        {
            var challan = _deliveryChallanService.GetById(id);
            if (challan == null)
            {
                return NotFound();
            }

            return View("CreateEdit", challan);
        }

        // POST: DeliveryChallan/Edit/5 delivery challan edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DeliveryChallan model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set modified by
                    model.ModifiedBy = User.FindFirstValue(ClaimTypes.Name);

                    // Update the challan
                    _deliveryChallanService.Update(model);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating delivery challan: " + ex.Message);
                }
            }

            return View("CreateEdit", model);
        }

        // GET: DeliveryChallan/Delete/5 to delete specific delivery challan show data in alert
        public IActionResult Delete(int id)
        {
            var challan = _deliveryChallanService.GetById(id);
            if (challan == null)
            {
                return NotFound();
            }

            return View(challan);
        }

        // POST: DeliveryChallan/Delete/5 confirm specific delivery challan deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _deliveryChallanService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error deleting delivery challan: " + ex.Message);
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        // POST: DeliveryChallan/UpdateStatus to update a delivery challan
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            try
            {
                _deliveryChallanService.UpdateStatus(id, status);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}