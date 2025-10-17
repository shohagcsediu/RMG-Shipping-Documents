using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Data;

namespace RMG_Shipping_Documents.Service
{
    public class GatepassService
    {
        private readonly ApplicationDbContext _context;

        public GatepassService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all gatepasses
        public List<Gatepass> GetAll()
        {
            return _context.Gatepass.ToList();
        }

        // Get gatepass by Id
        public Gatepass? GetById(int id)
        {
            return _context.Gatepass.FirstOrDefault(g => g.Id == id);
        }

        // Get gatepass by GatePassNo
        public Gatepass? GetByGatePassNo(string gatePassNo)
        {
            return _context.Gatepass.FirstOrDefault(g => g.GatePassNo == gatePassNo);
        }

        // Insert new gatepass
        public void Insert(Gatepass gatepass)
        {
            // Generate a unique gate pass number if not provided
            if (string.IsNullOrEmpty(gatepass.GatePassNo))
            {
                gatepass.GatePassNo = GenerateGatePassNo();
            }

            gatepass.CreatedDate = DateTime.Now;
            _context.Gatepass.Add(gatepass);
            _context.SaveChanges();
        }

        // Update existing gatepass
        public void Update(Gatepass gatepass)
        {
            gatepass.ModifiedDate = DateTime.Now;
            _context.Gatepass.Update(gatepass);
            _context.SaveChanges();
        }

        // Delete gatepass
        public void Delete(int id)
        {
            var item = _context.Gatepass.FirstOrDefault(g => g.Id == id);
            if (item != null)
            {
                _context.Gatepass.Remove(item);
                _context.SaveChanges();
            }
        }

        // Generate a unique gate pass number
        private string GenerateGatePassNo()
        {
            string prefix = "GP";
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("D2");

            // Get the last gate pass number for this month
            var lastGatepass = _context.Gatepass
                .Where(g => g.GatePassNo.StartsWith($"{prefix}-{year}-{month}"))
                .OrderByDescending(g => g.GatePassNo)
                .FirstOrDefault();

            int sequence = 1;

            if (lastGatepass != null)
            {
                // Extract the sequence number from the last gate pass number
                string[] parts = lastGatepass.GatePassNo.Split('-');
                if (parts.Length >= 3 && int.TryParse(parts[2], out int lastSequence))
                {
                    sequence = lastSequence + 1;
                }
            }

            return $"{prefix}-{year}-{month}-{sequence:D3}";
        }

        // Get unique truck numbers from delivery challans
        public List<string> GetUniqueTruckNumbers()
        {
            return _context.DeliveryChallan
                .Where(d => !string.IsNullOrEmpty(d.TruckNo))
                .Select(d => d.TruckNo)
                .Distinct()
                .OrderBy(d => d)
                .ToList();
        }

        // Get the last delivery challan for a specific truck
        public DeliveryChallan? GetLastDeliveryChallanForTruck(string truckNo)
        {
            return _context.DeliveryChallan
                .Where(d => d.TruckNo == truckNo)
                .OrderByDescending(d => d.Date)
                .ThenByDescending(d => d.Id)
                .FirstOrDefault();
        }

        // Get all delivery challans for a specific truck
        public List<DeliveryChallan> GetDeliveryChallansForTruck(string truckNo)
        {
            return _context.DeliveryChallan
                .Where(d => d.TruckNo == truckNo)
                .OrderByDescending(d => d.Date)
                .ThenByDescending(d => d.Id)
                .ToList();
        }


    }
}