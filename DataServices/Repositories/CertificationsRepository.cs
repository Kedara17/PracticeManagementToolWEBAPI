using DataServices.Data;
using DataServices.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public class CertificationsRepository : IRepository<Certifications>
    {
        private readonly DataBaseContext _context;

        public CertificationsRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Certifications>> GetAll()
        {
            return await _context.TblCertifications.ToListAsync();
        }

        public async Task<Certifications> Get(string id)
        {
            return await _context.TblCertifications.FindAsync(id);
        }

        public async Task<Certifications> Create(Certifications entity)
        {
            _context.TblCertifications.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Certifications> Update(Certifications entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(string id)
        {
            var certification = await _context.TblCertifications.FindAsync(id);

            if (certification == null)
            {
                return false;
            }

            certification.IsActive = false; // Soft delete
            _context.Entry(certification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
