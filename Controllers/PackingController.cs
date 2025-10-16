using Microsoft.AspNetCore.Mvc;
using RMG_Shipping_Documents.Data;
using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Service;

using ClosedXML.Excel;

namespace RMG_Shipping_Documents.Controllers
{
    public class PackingController : Controller
    {
        private readonly PackingService _packingService;
        private readonly ExcelTemplateService _templateService;

        public PackingController(ApplicationDbContext context)
        {
            _packingService = new PackingService(context);
            _templateService = new ExcelTemplateService(context);
        }

        // GET: /Packing
        public async Task<IActionResult> Index()
        {
            var list = await _packingService.GetAllAsync();
            return View(list);
        }

        // GET: /Packing/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new PackingList();
            // Initialize with an empty detail row
            model.PackingDetails = new List<PackingDetails> { new PackingDetails() };
            return View(model);
        }

        // POST: /Packing/Create
        [HttpPost]
        
        public async Task<IActionResult> Create(PackingList model)
        {
            try
            {                
                // if (ModelState.IsValid)
                // {

                if (model.PackingDetails != null && model.PackingDetails.Any())
                {
                    // Set the foreign key for each detail
                    foreach (var detail in model.PackingDetails)
                    {
                        detail.PackingListId = model.Id; // This will be set after saving the parent
                    }
                }

                await _packingService.InsertAsync(model);
                return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return View(model);
            }
        }

        // GET: /Packing/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _packingService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Packing/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _packingService.GetByIdAsync(id);
            if (item == null) return NotFound();

            // Ensure we have at least one detail row
            if (item.PackingDetails == null || !item.PackingDetails.Any())
            {
                item.PackingDetails = new List<PackingDetails> { new PackingDetails() };
            }

            // Use the Create view for editing
            return View("Create", item);
        }

        // POST: /Packing/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(PackingList model)
        {
            if (ModelState.IsValid)
            {
                await _packingService.UpdateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateEdit(PackingList model)
        {
            try
            {
                // Check if this is an edit (has ID) or create (no ID)
                if (model.Id > 0)
                {
                    // This is an edit operation
                    // Make sure the PackingDetails are loaded
                    var existingPacking = await _packingService.GetByIdAsync(model.Id);
                    if (existingPacking == null)
                    {
                        return NotFound();
                    }

                    // Update the packing list
                    await _packingService.UpdateAsync(model);
                }
                else
                {
                    // This is a create operation
                    await _packingService.InsertAsync(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");

                // If we're editing and there's an error, we need to reload the PackingDetails
                if (model.Id > 0)
                {
                    var existingModel = await _packingService.GetByIdAsync(model.Id);
                    if (existingModel != null)
                    {
                        model.PackingDetails = existingModel.PackingDetails;
                    }
                }

                return View("Create", model);
            }
        }

        // POST: /Packing/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _packingService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Packing/DownloadExcel/5
        [HttpGet]
        public async Task<IActionResult> DownloadExcel(int id)
        {
            var item = await _packingService.GetByIdAsync(id);
            if (item == null) return NotFound();

            var template = _templateService.GetDefault();
            if (template == null) return NotFound("No default template found.");

            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "PackingTemplates", template.FilePath);
            if (!System.IO.File.Exists(templatePath))
                return NotFound("Excel template not found.");

            using (var workbook = new XLWorkbook(templatePath))
            {
                foreach (var ws in workbook.Worksheets)
                {
                    foreach (var cell in ws.CellsUsed())
                    {
                        string value = cell.GetString();
                        value = value.Replace("{{BuyerName}}", item.BuyerName ?? "")
                                     .Replace("{{Style}}", item.Style ?? "")
                                     .Replace("{{PO}}", item.PO ?? "")
                                     .Replace("{{PackingDate}}", item.CreatedDate.ToString("yyyy-MM-dd"))
                                     .Replace("{{Destination}}", item.Destination ?? "")
                                     .Replace("{{TotalCartons}}", item.Cartons.ToString())
                                     .Replace("{{QtyPack}}", item.OrderQty.ToString())
                                     .Replace("{{TotalGWeight}}", item.TotalGWeight.ToString())
                                     .Replace("{{TotalNWeight}}", item.TotalNWeight.ToString())
                                     .Replace("{{TotalCBM}}", item.TotalCBM.ToString())
                                     .Replace("{{IsApproved}}", item.Status)
                                     .Replace("{{Status}}", item.Status ?? "");
                        cell.Value = value;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    string fileName = $"Packing_{item.BuyerName}_{item.Id}.xlsx";
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }
    }
}
