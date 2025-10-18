using Microsoft.AspNetCore.Mvc;
using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Service;
using RMG_Shipping_Documents.Data;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;

namespace RMG_Shipping_Documents.Controllers
{
    public class ExcelTemplateController : Controller
    {
        private readonly ExcelTemplateService _excelTemplateService;
        private readonly ApplicationDbContext _context;

        public ExcelTemplateController(ExcelTemplateService excelTemplateService, ApplicationDbContext context)
        {
            _excelTemplateService = excelTemplateService;
            _context = context;
        }

        // GET: ExcelTemplate
        public IActionResult Index(string buyerName, string packingListId)
        {
            ViewBag.BuyerName = buyerName ?? "All Buyers";
            ViewBag.PackingListId = packingListId;

            var templates = string.IsNullOrEmpty(buyerName)
                ? _excelTemplateService.GetAll()
                : _excelTemplateService.GetByBuyerName(buyerName);

            return View(templates);
        }

        // GET: ExcelTemplate/Create
        public IActionResult Create(string buyerName, string packingListId)
        {
            ViewBag.BuyerName = buyerName;
            ViewBag.PackingListId = packingListId;
            return View();
        }

        // POST: ExcelTemplate/Create to create template
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Template template, IFormFile file, string buyerName, string packingListId,
            List<string> excelFields, List<string> formFields, List<bool> isRecurringList)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    template.BuyerName = buyerName;
                    template.PackingListId = packingListId;

                    // Handle file upload
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "templates");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        template.FilePath = "/uploads/templates/" + fileName;
                        template.FileType = Path.GetExtension(file.FileName);
                    }

                    // Initialize field mappings
                    template.FieldMappings = new List<TemplateFieldMapping>();

                    // Add field mappings
                    if (excelFields != null && formFields != null)
                    {
                        for (int i = 0; i < excelFields.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(excelFields[i]) && !string.IsNullOrEmpty(formFields[i]))
                            {
                                template.FieldMappings.Add(new TemplateFieldMapping
                                {
                                    ExcelFieldName = excelFields[i],
                                    FormFieldName = formFields[i],
                                    IsRecurring = isRecurringList != null && i < isRecurringList.Count && isRecurringList[i],
                                    OrderIndex = i
                                });
                            }
                        }
                    }

                    _excelTemplateService.Insert(template);

                    TempData["Success"] = "Template created successfully!";
                    return RedirectToAction(nameof(Index), new { buyerName = buyerName, packingListId = packingListId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading template: " + ex.Message);
                }
            }

            ViewBag.BuyerName = buyerName;
            ViewBag.PackingListId = packingListId;
            return View(template);
        }

        // POST: Get Excel Columns
        [HttpPost]
        public IActionResult GetExcelColumns(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        stream.Position = 0;

                        using (var workbook = new XLWorkbook(stream))
                        {
                            var worksheet = workbook.Worksheet(1);
                            var columns = new List<string>();

                            // Get headers from first row
                            var firstRow = worksheet.FirstRowUsed();
                            if (firstRow != null)
                            {
                                foreach (var cell in firstRow.CellsUsed())
                                {
                                    var header = cell.Value.ToString();
                                    if (!string.IsNullOrWhiteSpace(header))
                                    {
                                        columns.Add(header);
                                    }
                                }
                            }

                            return Json(new { success = true, columns = columns });
                        }
                    }
                }

                return Json(new { success = false, message = "No file uploaded" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: ExcelTemplate/Edit/5
        public IActionResult Edit(int id)
        {
            var template = _excelTemplateService.GetById(id);
            if (template == null)
            {
                return NotFound();
            }

            return View(template);
        }

        // POST: ExcelTemplate/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Template template, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingTemplate = _excelTemplateService.GetById(template.Id);
                    if (existingTemplate == null)
                    {
                        return NotFound();
                    }

                    existingTemplate.TemplateName = template.TemplateName;
                    existingTemplate.IsDefault = template.IsDefault;

                    // Handle file upload if new file provided
                    if (file != null && file.Length > 0)
                    {
                        // Delete old file
                        if (!string.IsNullOrEmpty(existingTemplate.FilePath))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingTemplate.FilePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "templates");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        existingTemplate.FilePath = "/uploads/templates/" + fileName;
                        existingTemplate.FileType = Path.GetExtension(file.FileName);
                    }

                    _excelTemplateService.Update(existingTemplate);

                    TempData["Success"] = "Template updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating template: " + ex.Message);
                }
            }

            return View(template);
        }

        // POST: ExcelTemplate/DeleteTemplate (NEW - For Modal Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTemplate(int id, string buyerName, string packingListId)
        {
            try
            {
                var template = _excelTemplateService.GetById(id);
                if (template == null)
                {
                    TempData["Error"] = "Template not found";
                    return RedirectToAction(nameof(Index), new { buyerName = buyerName, packingListId = packingListId });
                }

                // Delete the physical file
                if (!string.IsNullOrEmpty(template.FilePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", template.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Delete the template (mappings will be deleted automatically due to cascade)
                _excelTemplateService.Delete(id);

                TempData["Success"] = "Template deleted successfully!";
                return RedirectToAction(nameof(Index), new { buyerName = buyerName, packingListId = packingListId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting template: " + ex.Message;
                return RedirectToAction(nameof(Index), new { buyerName = buyerName, packingListId = packingListId });
            }
        }

        // POST: ExcelTemplate/SetDefault to select a template as default
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetDefault(int id)
        {
            try
            {
                var templates = _excelTemplateService.GetAll();

                foreach (var template in templates)
                {
                    template.IsDefault = false;
                    _excelTemplateService.Update(template);
                }

                var defaultTemplate = _excelTemplateService.GetById(id);
                if (defaultTemplate != null)
                {
                    defaultTemplate.IsDefault = true;
                    _excelTemplateService.Update(defaultTemplate);
                }

                TempData["Success"] = "Default template set successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error setting default template: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ExcelTemplate/Download/5 to download template
        public IActionResult Download(int id, int packingListId)
        {
            try
            {
                // Get template with mappings
                var template = _excelTemplateService.GetById(id);
                if (template == null)
                {
                    TempData["Error"] = "Template not found";
                    return RedirectToAction(nameof(Index));
                }

                // Get packing list data with details
                var packingList = _context.PackingList
                    .Include(p => p.PackingDetails)
                    .FirstOrDefault(p => p.Id == packingListId);

                if (packingList == null)
                {
                    TempData["Error"] = "Packing list not found";
                    return RedirectToAction(nameof(Index));
                }

                // Get template file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", template.FilePath.TrimStart('/'));

                if (!System.IO.File.Exists(filePath))
                {
                    TempData["Error"] = "Template file not found";
                    return RedirectToAction(nameof(Index));
                }

                // Load workbook and fill with data
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var mappings = template.FieldMappings?.OrderBy(m => m.OrderIndex).ToList() ?? new List<TemplateFieldMapping>();

                    // Get header row
                    var headerRow = worksheet.FirstRowUsed();
                    var columnIndexes = new Dictionary<string, int>();

                    // Map Excel column names to their column numbers
                    if (headerRow != null)
                    {
                        foreach (var cell in headerRow.CellsUsed())
                        {
                            var header = cell.Value.ToString();
                            if (!string.IsNullOrWhiteSpace(header))
                            {
                                columnIndexes[header] = cell.Address.ColumnNumber;
                            }
                        }
                    }

                    // Start from row 2 (after header)
                    int currentRow = 2;

                    // Fill data for each packing detail item
                    foreach (var detail in packingList.PackingDetails)
                    {
                        foreach (var mapping in mappings)
                        {
                            if (columnIndexes.ContainsKey(mapping.ExcelFieldName))
                            {
                                int colIndex = columnIndexes[mapping.ExcelFieldName];
                                var value = GetPackingListFieldValue(detail, packingList, mapping.FormFieldName);

                                if (value != null)
                                {
                                    worksheet.Cell(currentRow, colIndex).Value = value.ToString();
                                }
                            }
                        }
                        currentRow++;
                    }

                    // Save to memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        var fileName = $"{template.TemplateName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error downloading template: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Helper method to get field value from packing details ONLY
        private object? GetPackingListFieldValue(PackingDetails detail, PackingList packingList, string fieldName)
        {
            try
            {
                return fieldName switch
                {
                    // PackingDetails Fields ONLY
                    "SL" => detail.SL,
                    "NoOfCarton" => detail.NoOfCarton,
                    "CartonStart" => detail.CartonStart,
                    "CartonEnd" => detail.CartonEnd,
                    "SizeName" => detail.SizeName,
                    "Ratio" => detail.Ratio,
                    "ArticleNo" => detail.ArticleNo,
                    "PcsPack" => detail.PcsPack,
                    "PacCarton" => detail.PacCarton,
                    "OrderQtyPcs" => detail.OrderQtyPcs,
                    "TotalPcsSize" => detail.TotalPcsSize,
                    "TotalPacs" => detail.TotalPacs,
                    "GWt" => detail.GWt,
                    "NWt" => detail.NWt,
                    "TotalGWt" => detail.TotalGWt,
                    "TotalNWt" => detail.TotalNWt,
                    "Length" => detail.Length,
                    "Weight" => detail.Weight,
                    "Height" => detail.Height,
                    "CBM" => detail.CBM,

                    // Default
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }
    }
}