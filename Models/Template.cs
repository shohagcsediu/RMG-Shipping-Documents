namespace RMG_Shipping_Documents.Models

{
    public class Template
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = "";

        public string FilePath { get; set; } = "";

        public bool IsDefault { get; set; }

    }
}
