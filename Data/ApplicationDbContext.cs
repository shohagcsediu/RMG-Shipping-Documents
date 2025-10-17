using Microsoft.EntityFrameworkCore;
using RMG_Shipping_Documents.Models;

namespace RMG_Shipping_Documents.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PackingList> PackingList { get; set; }
        public DbSet<PackingDetails> PackingDetails { get; set; }

        public DbSet<Template> ExcelTemplates { get; set; }

        public DbSet<DeliveryChallan> DeliveryChallan { get; set; }

        public DbSet<Gatepass> Gatepass { get; set; }
    }
}
