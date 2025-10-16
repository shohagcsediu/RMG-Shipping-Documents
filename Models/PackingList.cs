using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMG_Shipping_Documents.Models
{
    [Table("PackingList")]
    public class PackingList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Main Packing List Fields - ALL NULLABLE
        [MaxLength(200)]
        public string? Company { get; set; }

        [MaxLength(200)]
        public string? BuyerName { get; set; }

        [MaxLength(200)]
        public string? Style { get; set; }

        [MaxLength(200)]
        public string? PO { get; set; }

        [MaxLength(200)]
        public string? StyleBuyer { get; set; }

        [MaxLength(200)]
        public string? ShippedBy { get; set; }

        [MaxLength(500)]
        public string? ShortDescription { get; set; }

        [MaxLength(100)]
        public string? Size { get; set; }

        [MaxLength(100)]
        public string? Color { get; set; }

        [MaxLength(200)]
        public string? Brand { get; set; }

        [MaxLength(200)]
        public string? Packing { get; set; }

        [MaxLength(100)]
        public string? PcsPerSet { get; set; }

        [MaxLength(200)]
        public string? ProductNo { get; set; }

        [MaxLength(200)]
        public string? CountryOfOrigin { get; set; }

        [MaxLength(200)]
        public string? PortOfLoading { get; set; }

        [MaxLength(200)]
        public string? CountryOfDestination { get; set; }

        [MaxLength(200)]
        public string? PortOfDischarge { get; set; }

        [MaxLength(100)]
        public string? Incoterm { get; set; }

        [MaxLength(200)]
        public string? ORMSNo { get; set; }

        [MaxLength(200)]
        public string? LMPONo { get; set; }

        [MaxLength(200)]
        public string? ORMSStyleNo { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        // Buyer Specific Fields - ALL NULLABLE
        [MaxLength(200)]
        public string? ItemNo { get; set; }

        [MaxLength(200)]
        public string? POD { get; set; }

        [MaxLength(200)]
        public string? BOI { get; set; }

        [MaxLength(200)]
        public string? WWHK { get; set; }

        [MaxLength(100)]
        public string? NoOfColor { get; set; }

        [MaxLength(200)]
        public string? KeyCode { get; set; }

        [MaxLength(200)]
        public string? SupplierCode { get; set; }

        // Summary Fields - Already nullable ✓
        public int? Cartons { get; set; }
        public int? OrderQtyPac { get; set; }
        public int? OrderQtyPcs { get; set; }
        public int? ShipQtyPac { get; set; }
        public int? ShipQtyPcs { get; set; }
        public int? TotalGWeight { get; set; }
        public int? TotalNWeight { get; set; }
        public int? TotalCBM { get; set; }

        [MaxLength(200)]
        public string? Destination { get; set; }

        public int? SerialNO { get; set; }

        [MaxLength(200)]
        public string? SizeName { get; set; }

        [MaxLength(200)]
        public string? OrderQty { get; set; }

        public int? ActualShipment { get; set; }

        [MaxLength(200)]
        public string? Variant { get; set; }

        public int? Percentage { get; set; }

        // Audit Fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        // Navigation Property
        public virtual ICollection<PackingDetails> PackingDetails { get; set; } = new List<PackingDetails>();
    }

    [Table("PackingDetails")]
    public class PackingDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("PackingList")]
        public int PackingListId { get; set; }
        public virtual PackingList? PackingList { get; set; }

        public int? SL { get; set; }
        public int? NoOfCarton { get; set; }

        [MaxLength(50)]
        public string? CartonStart { get; set; }

        [MaxLength(50)]
        public string? CartonEnd { get; set; }

        [MaxLength(100)]
        public string? SizeName { get; set; }

        [MaxLength(100)]
        public string? Ratio { get; set; }

        [MaxLength(200)]
        public string? ArticleNo { get; set; }

        public int? PcsPack { get; set; }
        public int? PacCarton { get; set; }
        public int? OrderQtyPcs { get; set; }
        public int? TotalPcsSize { get; set; }
        public int? TotalPacs { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GWt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NWt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalGWt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalNWt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Length { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Weight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Height { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? CBM { get; set; }
    }
}