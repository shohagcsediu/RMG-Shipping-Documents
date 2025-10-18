namespace RMG_Shipping_Documents.Models
{
    public class TemplateFieldMapping
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string? ExcelFieldName { get; set; }  // From Excel file (e.g., "BoxCount")
        public string? FormFieldName { get; set; }    // From PackingList (e.g., "CartonCount")
        public bool IsRecurring { get; set; }
        public int OrderIndex { get; set; }           // To maintain the order of mappings

        // Navigation property
        public Template? Template { get; set; }
    }
}