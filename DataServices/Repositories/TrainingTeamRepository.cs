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
    public class TrainingTeamRepository : IRepository<TrainingTeam>
    {
        private readonly DataBaseContext _context;

        public TrainingTeamRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrainingTeam>> GetAll()
        {
            return await _context.TblTrainingTeam.ToListAsync();
        }

        public async Task<TrainingTeam> Get(string id)
        {
            return await _context.TblTrainingTeam.FindAsync(id);
        }

        public async Task<TrainingTeam> Create(TrainingTeam trainingTeam)
        {
            _context.TblTrainingTeam.Add(trainingTeam);
            await _context.SaveChangesAsync();
            return trainingTeam;
        }

        public async Task<TrainingTeam> Update(TrainingTeam trainingTeam)
        {
            _context.Entry(trainingTeam).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return trainingTeam;
        }

        public async Task<bool> Delete(string id)
        {
            var trainingTeam = await _context.TblTrainingTeam.FindAsync(id);
            if (trainingTeam == null)
            {
                return false;
            }

            _context.TblTrainingTeam.Remove(trainingTeam);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
