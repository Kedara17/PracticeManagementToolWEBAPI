using DataServices.Data;
using DataServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public class NewLeadEnquiryRepository : IRepository<NewLeadEnquiry>
    {
        private readonly DataBaseContext _context;

        public NewLeadEnquiryRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewLeadEnquiry>> GetAll()
        {
            return await _context.TblNewLeadEnquiry.ToListAsync();
        }

        public async Task<NewLeadEnquiry> Get(string id)
        {
            return await _context.TblNewLeadEnquiry.FindAsync(id);
        }

        public async Task<NewLeadEnquiry> Create(NewLeadEnquiry newLeadEnquiry)
        {
            _context.TblNewLeadEnquiry.Add(newLeadEnquiry);
            await _context.SaveChangesAsync();
            return newLeadEnquiry;
        }

        public async Task<NewLeadEnquiry> Update(NewLeadEnquiry newLeadEnquiry)
        {
            _context.Entry(newLeadEnquiry).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return newLeadEnquiry;
        }

        public async Task<bool> Delete(string id)
        {
            var newLeadEnquiry = await _context.TblNewLeadEnquiry.FindAsync(id);
            if (newLeadEnquiry == null)
            {
                return false;
            }

            _context.TblNewLeadEnquiry.Remove(newLeadEnquiry);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
