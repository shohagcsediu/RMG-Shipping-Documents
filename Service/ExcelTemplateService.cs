using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Data;

namespace RMG_Shipping_Documents.Service

{
    public class ExcelTemplateService
    {
        private readonly ApplicationDbContext _context;

        public ExcelTemplateService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all templates
        public List<Template> GetAll()
        {
            return _context.ExcelTemplates.ToList();
        }

        // Get template by Id
        public Template? GetById(int id)
        {
            return _context.ExcelTemplates.FirstOrDefault(t => t.Id == id);
        }

        // Get default template
        public Template? GetDefault()
        {
            return _context.ExcelTemplates.FirstOrDefault(t => t.IsDefault);
        }

        // Insert new template
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

        // Delete template
        public void Delete(int id)
        {
            var item = _context.ExcelTemplates.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                _context.ExcelTemplates.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
