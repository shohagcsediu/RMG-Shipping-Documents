using System.ComponentModel.DataAnnotations;

namespace RMG_Shipping_Documents.Models
{
    public class Gatepass
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string GatePassNo { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; } = "";

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? IssuedTo { get; set; }

        [StringLength(50)]
        public string? TruckNo { get; set; }

        [StringLength(100)]
        public string? DriverName { get; set; }

        [StringLength(255)]
        public string? TransportCompany { get; set; }

        [StringLength(50)]
        public string? MobileNo { get; set; }

        [StringLength(50)]
        public string? LicenseNo { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }
    }
}