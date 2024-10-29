using DataServices.Data;
using DataServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataServices.Repositories
{
    public class BestPerformersRepository : IRepository<BestPerformers>
    {
        private readonly DataBaseContext _context;
        private readonly ILogger<BestPerformersRepository> _logger;

        public BestPerformersRepository(DataBaseContext context, ILogger<BestPerformersRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<BestPerformers>> GetAll()
        {
            try
            {
                return await _context.TblBestPerformers
                                     .Include(b => b.Employee)
                                     .Include(b => b.Client)
                                     .Include(b => b.Project)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all best performers");
                throw;
            }
        }

        public async Task<BestPerformers> Get(string id)
        {
            try
            {
                var bestPerformer = await _context.TblBestPerformers
                                                  .Include(b => b.Employee)
                                                  .Include(b => b.Client)
                                                  .Include(b => b.Project)
                                                  .FirstOrDefaultAsync(b => b.Id == id);

                if (bestPerformer == null)
                {
                    _logger.LogWarning($"Best performer with id: {id} was not found.");
                }
                return bestPerformer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching best performer with id: {id}");
                throw;
            }
        }

        public async Task<BestPerformers> Create(BestPerformers bestPerformer)
        {
            if (bestPerformer == null)
                throw new ArgumentNullException(nameof(bestPerformer));

            try
            {
                _context.TblBestPerformers.Add(bestPerformer);
                await _context.SaveChangesAsync();
                return bestPerformer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating best performer");
                throw;
            }
        }

        public async Task<BestPerformers> Update(BestPerformers bestPerformer)
        {
            if (bestPerformer == null)
                throw new ArgumentNullException(nameof(bestPerformer));

            try
            {
                _context.Entry(bestPerformer).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return bestPerformer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating best performer");
                throw;
            }
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                var bestPerformer = await _context.TblBestPerformers.FindAsync(id);
                if (bestPerformer == null)
                {
                    _logger.LogWarning($"Best performer with id: {id} not found for deletion.");
                    return false;
                }

                _context.TblBestPerformers.Remove(bestPerformer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting best performer with id: {id}");
                throw;
            }
        }
    }
}