using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Data;
using Microsoft.EntityFrameworkCore;

namespace RMG_Shipping_Documents.Service
{
    public class ExcelTemplateService
    {
        private readonly ApplicationDbContext _context;

        public ExcelTemplateService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all templates with mappings
        public List<Template> GetAll()
        {
            return _context.ExcelTemplates
                .Include(t => t.FieldMappings)
                .ToList();
        }

        // Get template by Id with mappings
        public Template? GetById(int id)
        {
            return _context.ExcelTemplates
                .Include(t => t.FieldMappings)
                .FirstOrDefault(t => t.Id == id);
        }

        // Get templates by buyer name
        public List<Template> GetByBuyerName(string buyerName)
        {
            return _context.ExcelTemplates
                .Include(t => t.FieldMappings)
                .Where(t => t.BuyerName == buyerName)
                .ToList();
        }

        // Get default template
        public Template? GetDefault()
        {
            return _context.ExcelTemplates
                .Include(t => t.FieldMappings)
                .FirstOrDefault(t => t.IsDefault);
        }

        // Insert new template with mappings
        public void Insert(Template template)
        {
            _context.ExcelTemplates.Add(template);
            _context.SaveChanges();
        }

        // Update existing template
        public void Update(Template template)
        {
            _context.ExcelTemplates.Update(template);
            _context.SaveChanges();
        }

        // Delete template (mappings will be deleted automatically due to cascade)
        public void Delete(int id)
        {
            var item = _context.ExcelTemplates
                .Include(t => t.FieldMappings)
                .FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                _context.ExcelTemplates.Remove(item);
                _context.SaveChanges();
            }
        }

        // Add or update field mappings
        public void SaveFieldMappings(int templateId, List<TemplateFieldMapping> mappings)
        {
            // Remove existing mappings
            var existingMappings = _context.TemplateFieldMappings
                .Where(m => m.TemplateId == templateId)
                .ToList();
            _context.TemplateFieldMappings.RemoveRange(existingMappings);

            // Add new mappings
            foreach (var mapping in mappings)
            {
                mapping.TemplateId = templateId;
                _context.TemplateFieldMappings.Add(mapping);
            }

            _context.SaveChanges();
        }
    }
}