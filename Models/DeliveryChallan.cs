using System.ComponentModel.DataAnnotations;

namespace RMG_Shipping_Documents.Models
{
    public class DeliveryChallan
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ChallanNo { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100)]
        public string Recipient { get; set; } = "";

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = "";

        [StringLength(255)]
        public string? TruckCBM { get; set; }

        [StringLength(255)]
        public string? AgdlCBM { get; set; }

        [StringLength(100)]
        public string? Driver { get; set; }

        [StringLength(50)]
        public string? TruckNo { get; set; }

        [StringLength(50)]
        public string? MobileNo { get; set; }

        [StringLength(50)]
        public string? LicenseNo { get; set; }

        [StringLength(255)]
        public string? DepoteName { get; set; }

        [StringLength(255)]
        public string? LockNo { get; set; }

        public DateTime? InDate { get; set; }

        public DateTime? OutDate { get; set; }

        [StringLength(255)]
        public string? TransportCompany { get; set; }

        [StringLength(500)]
        public string? DescriptionGoods { get; set; }

        [StringLength(255)]
        public string? Unit { get; set; }

        public int? Quanitity { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }
    }
}