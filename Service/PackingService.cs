using Microsoft.EntityFrameworkCore;
using RMG_Shipping_Documents.Models;
using RMG_Shipping_Documents.Data;

namespace RMG_Shipping_Documents.Service
{
    public class PackingService
    {
        private readonly ApplicationDbContext _context;

        public PackingService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all packing lists with details
        public async Task<List<PackingList>> GetAllAsync()
        {
            return await _context.PackingList
                .Include(p => p.PackingDetails) // Include child records
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        // Get a single packing list by ID with details
        public async Task<PackingList?> GetByIdAsync(int id)
        {
            return await _context.PackingList
                .Include(p => p.PackingDetails) // Include child records
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Insert a new packing list with details
        public async Task InsertAsync(PackingList packing)
        {
            // Set audit fields
            packing.CreatedDate = DateTime.Now;
            packing.CreatedBy = "System"; // Replace with actual user

            // Add to context
            _context.PackingList.Add(packing);

            // Save the parent first to get the ID
            await _context.SaveChangesAsync();

            // Now set the foreign key for each detail
            if (packing.PackingDetails != null)
            {
                foreach (var detail in packing.PackingDetails)
                {
                    detail.PackingListId = packing.Id;
                    _context.PackingDetails.Add(detail);
                }

                // Save the details
                await _context.SaveChangesAsync();
            }
        }

        // Update an existing packing list

        public async Task UpdateAsync(PackingList packing)
        {
            // Set update audit fields
            packing.UpdatedDate = DateTime.Now;
            packing.UpdatedBy = "System"; // Replace with actual user

            // Get the existing entity with its details
            var existingPacking = await _context.PackingList
                .Include(p => p.PackingDetails)
                .FirstOrDefaultAsync(p => p.Id == packing.Id);

            if (existingPacking != null)
            {
                // Update the parent properties
                _context.Entry(existingPacking).CurrentValues.SetValues(packing);

                // Handle the child details - THIS IS THE FIX
                if (packing.PackingDetails != null)
                {
                    // Remove details that are no longer present
                    foreach (var detail in existingPacking.PackingDetails.ToList())
                    {
                        if (!packing.PackingDetails.Any(d => d.Id == detail.Id))
                        {
                            _context.PackingDetails.Remove(detail);
                        }
                    }

                    // Update or add details
                    foreach (var detail in packing.PackingDetails)
                    {
                        var existingDetail = existingPacking.PackingDetails
                            .FirstOrDefault(d => d.Id == detail.Id);

                        if (existingDetail != null)
                        {
                            // Update existing detail
                            _context.Entry(existingDetail).CurrentValues.SetValues(detail);
                        }
                        else
                        {
                            // Add new detail
                            detail.PackingListId = packing.Id;
                            existingPacking.PackingDetails.Add(detail);
                        }
                    }
                }
                else
                {
                    // If packing.PackingDetails is null, remove all existing details
                    foreach (var detail in existingPacking.PackingDetails.ToList())
                    {
                        _context.PackingDetails.Remove(detail);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        // Delete a packing list (cascade will delete PackingDetails)
        public async Task DeleteAsync(int id)
        {
            var item = await _context.PackingList
                .Include(p => p.PackingDetails)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item != null)
            {
                _context.PackingList.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}