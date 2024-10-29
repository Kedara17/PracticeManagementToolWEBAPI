/*SuccessStoriesRepository*/

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
    public class SuccessStoriesRepository : IRepository<SuccessStories>
    {
        private readonly DataBaseContext _context;

        public SuccessStoriesRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SuccessStories>> GetAll()
        {
            return await _context.TblSuccessStories.ToListAsync();
        }

        public async Task<SuccessStories> Get(string id)
        {
            return await _context.TblSuccessStories.FindAsync(id);
        }

        public async Task<SuccessStories> Create(SuccessStories entity)
        {
            _context.TblSuccessStories.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<SuccessStories> Update(SuccessStories entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(string id)
        {
            var successStories = await _context.TblSuccessStories.FindAsync(id);

            if (successStories == null)
            {
                return false;
            }

            successStories.IsActive = false; // Soft delete
            _context.Entry(successStories).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
