using Microsoft.AspNetCore.Mvc;
using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Service;
using RMG_Shipping_Documents.Data;

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
        public IActionResult Index(string buyerName)
        {
            // Get all templates
            var templates = _excelTemplateService.GetAll();

            // Pass the buyer name to the view
            //ViewBag.BuyerName = buyerName ?? "All Buyers";

            return View(templates);
        }

        // GET: ExcelTemplate/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExcelTemplate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Template template, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (file != null && file.Length > 0)
                    {
                        // Create a unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        // Define the path to save the file
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "templates");

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Save the file
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // Set the file path in the template
                        template.FilePath = "/uploads/templates/" + fileName;
                    }

                    // Insert the template
                    _excelTemplateService.Insert(template);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error uploading template: " + ex.Message);
                }
            }

            return View(template);
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
                    // Get the existing template
                    var existingTemplate = _excelTemplateService.GetById(template.Id);
                    if (existingTemplate == null)
                    {
                        return NotFound();
                    }

                    // Update template properties
                    existingTemplate.TemplateName = template.TemplateName;
                    existingTemplate.IsDefault = template.IsDefault;

                    // Handle file upload if a new file is provided
                    if (file != null && file.Length > 0)
                    {
                        // Delete the old file if it exists
                        if (!string.IsNullOrEmpty(existingTemplate.FilePath))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingTemplate.FilePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Create a unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        // Define the path to save the file
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "templates");

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Save the file
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // Update the file path
                        existingTemplate.FilePath = "/uploads/templates/" + fileName;
                    }

                    // Update the template
                    _excelTemplateService.Update(existingTemplate);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating template: " + ex.Message);
                }
            }

            return View(template);
        }

        // GET: ExcelTemplate/Delete/5
        public IActionResult Delete(int id)
        {
            var template = _excelTemplateService.GetById(id);
            if (template == null)
            {
                return NotFound();
            }

            return View(template);
        }

        // POST: ExcelTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Get the template
                var template = _excelTemplateService.GetById(id);
                if (template == null)
                {
                    return NotFound();
                }

                // Delete the file if it exists
                if (!string.IsNullOrEmpty(template.FilePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", template.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Delete the template
                _excelTemplateService.Delete(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error deleting template: " + ex.Message);
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        // POST: ExcelTemplate/SetDefault
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetDefault(int id)
        {
            try
            {
                // Get all templates
                var templates = _excelTemplateService.GetAll();

                // Set all templates to non-default
                foreach (var template in templates)
                {
                    template.IsDefault = false;
                    _excelTemplateService.Update(template);
                }

                // Set the selected template as default
                var defaultTemplate = _excelTemplateService.GetById(id);
                if (defaultTemplate != null)
                {
                    defaultTemplate.IsDefault = true;
                    _excelTemplateService.Update(defaultTemplate);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error setting default template: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ExcelTemplate/Download/5
        public IActionResult Download(int id)
        {
            try
            {
                // Get the template
                var template = _excelTemplateService.GetById(id);
                if (template == null)
                {
                    return NotFound();
                }

                // Get the file path
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", template.FilePath.TrimStart('/'));

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                // Get the file content
                var fileBytes = System.IO.File.ReadAllBytes(filePath);

                // Return the file
                return File(fileBytes, "application/octet-stream", template.TemplateName + Path.GetExtension(filePath));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error downloading template: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}