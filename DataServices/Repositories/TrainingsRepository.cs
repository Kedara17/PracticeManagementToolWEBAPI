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
    public class TrainingsRepository : IRepository<Trainings>
    {
        private readonly DataBaseContext _context;

        public TrainingsRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trainings>> GetAll()
        {
            return await _context.TblTrainings.ToListAsync();
        }

        public async Task<Trainings> Get(string id)
        {
            return await _context.TblTrainings.FindAsync(id);
        }

        public async Task<Trainings> Create(Trainings trainings)
        {
            _context.TblTrainings.Add(trainings);
            await _context.SaveChangesAsync();
            return trainings;
        }

        public async Task<Trainings> Update(Trainings trainings)
        {
            _context.Entry(trainings).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return trainings;
        }

        public async Task<bool> Delete(string id)
        {
            var trainings = await _context.TblTrainings.FindAsync(id);
            if (trainings == null)
            {
                return false;
            }

            _context.TblTrainings.Remove(trainings);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
