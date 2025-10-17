using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Data;

namespace RMG_Shipping_Documents.Service
{
    public class DeliveryChallanService
    {
        private readonly ApplicationDbContext _context;

        public DeliveryChallanService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all delivery challans
        public List<DeliveryChallan> GetAll()
        {
            return _context.DeliveryChallan.ToList();
        }

        // Get delivery challan by Id
        public DeliveryChallan? GetById(int id)
        {
            return _context.DeliveryChallan.FirstOrDefault(c => c.Id == id);
        }

        // Get delivery challan by ChallanNo
        public DeliveryChallan? GetByChallanNo(string challanNo)
        {
            return _context.DeliveryChallan.FirstOrDefault(c => c.ChallanNo == challanNo);
        }

        // Insert new delivery challan
        public void Insert(DeliveryChallan challan)
        {
            // Generate a unique challan number if not provided
            if (string.IsNullOrEmpty(challan.ChallanNo))
            {
                challan.ChallanNo = GenerateChallanNo();
            }

            challan.CreatedDate = DateTime.Now;
            _context.DeliveryChallan.Add(challan);
            _context.SaveChanges();
        }

        // Update existing delivery challan
        public void Update(DeliveryChallan challan)
        {
            challan.ModifiedDate = DateTime.Now;
            _context.DeliveryChallan.Update(challan);
            _context.SaveChanges();
        }

        // Delete delivery challan
        public void Delete(int id)
        {
            var item = _context.DeliveryChallan.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                _context.DeliveryChallan.Remove(item);
                _context.SaveChanges();
            }
        }

        // Generate a unique challan number
        private string GenerateChallanNo()
        {
            string prefix = "CH";
            string year = DateTime.Now.Year.ToString();

            // Get the last challan number for this year
            var lastChallan = _context.DeliveryChallan
                .Where(c => c.ChallanNo.StartsWith($"{prefix}-{year}"))
                .OrderByDescending(c => c.ChallanNo)
                .FirstOrDefault();

            int sequence = 1;

            if (lastChallan != null)
            {
                // Extract the sequence number from the last challan number
                string[] parts = lastChallan.ChallanNo.Split('-');
                if (parts.Length >= 3 && int.TryParse(parts[2], out int lastSequence))
                {
                    sequence = lastSequence + 1;
                }
            }

            return $"{prefix}-{year}-{sequence:D3}";
        }

        // Update delivery status
        public void UpdateStatus(int id, string status)
        {
            var challan = GetById(id);
            if (challan != null)
            {
                challan.Status = status;
                challan.ModifiedDate = DateTime.Now;

                // Update InDate or OutDate based on status
                if (status == "In Progress" && !challan.InDate.HasValue)
                {
                    challan.InDate = DateTime.Now;
                }
                else if (status == "Completed" && !challan.OutDate.HasValue)
                {
                    challan.OutDate = DateTime.Now;
                }

                _context.SaveChanges();
            }
        }
    }
}