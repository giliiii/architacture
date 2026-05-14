using BsdFinalProject.Data;
using BsdFinalProject.Models;
using Chocolate.Data;
using Microsoft.EntityFrameworkCore;
using BsdFinalProject.DTOs;
using BsdFinalProject.IRepositories;

namespace BsdFinalProject.Repositories
{
    public class DonorRepository : IDonorRepository
    {
        private readonly SaleContextFactory _saleContextFactory;
        private Lazy<SaleContext> _lazyContext;

        public DonorRepository(SaleContextFactory saleContextFactory)
        {
            _saleContextFactory = saleContextFactory;
            _lazyContext = new Lazy<SaleContext>(() => _saleContextFactory.CreateContext());
        }

        private SaleContext _context => _lazyContext.Value;

        public async Task<Donor> CreateNewDonor(Donor donor)
        {
            _context.Donor.Add(donor);
            await _context.SaveChangesAsync();
            return donor;
        }
        public async Task<Donor?> GetDonorById(int id)
        {
            return await _context.Donor.FindAsync(id);
        }
        public async Task<IEnumerable<Donor>> GetAllDonors()
        {
            return await _context.Donor.ToListAsync();
        }

        public async Task<Donor?> DeleteDonor(int id)
        {
            var donor = await _context.Donor.FindAsync(id);
            if (donor == null) return null;
            _context.Donor.Remove(donor);
            await _context.SaveChangesAsync();
            return donor;
        }
        public async Task<Donor?> UpdateDonor(Donor donor)
        {
            var existingDonor = await _context.Donor.FindAsync(donor.Id);
            if (existingDonor == null) return null;
            existingDonor.Name = donor.Name;
            existingDonor.EMail = donor.EMail;
            await _context.SaveChangesAsync();
            return existingDonor;
        }

        public async Task<IEnumerable<Gift?>> GetDonorGiftList(int id)
        {
            var donor = await _context.Donor
                .Include(d => d.GiftsList)
                .FirstOrDefaultAsync(d => d.Id == id);
            return donor?.GiftsList ?? Enumerable.Empty<Gift>();
        }
        public async Task<Donor?> GetDonorByEmail(string email)
        {
            return await _context.Donor
                .FirstOrDefaultAsync(d => d.EMail == email);
        }
    }
}