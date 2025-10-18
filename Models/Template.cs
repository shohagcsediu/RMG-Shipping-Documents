namespace RMG_Shipping_Documents.Models
{
    public class Template
    {
        public int Id { get; set; }
        public string? TemplateName { get; set; }
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
        public string? Version { get; set; }
        public bool IsDefault { get; set; }
        public string? PackingListId { get; set; }
        public string? BuyerName { get; set; }

        // Navigation property
        public ICollection<TemplateFieldMapping>? FieldMappings { get; set; }
    }
}