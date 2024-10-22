using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TrainingApi.Services
{
    public class TrainingTeamService : ITrainingTeamService
    {
        private readonly IRepository<TrainingTeam> _repository;
        private readonly DataBaseContext _context;

        public TrainingTeamService(IRepository<TrainingTeam> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<TrainingTeamDTO>> GetAll()
        {
            var trainingTeams = await _context.TblTrainingTeam
                .Include(e => e.Employee)
                .Include(e => e.Trainings)
                .ToListAsync();

            var trainingTeamDtos = new List<TrainingTeamDTO>();

            foreach (var trainings in trainingTeams)
            {
                trainingTeamDtos.Add(new TrainingTeamDTO
                {
                    Id = trainings.Id,
                    Trainings = trainings.Trainings?.Topic,
                    Employee = trainings.Employee?.Name,
                    IsActive = trainings.IsActive,
                    CreatedBy = trainings.CreatedBy,
                    CreatedDate = trainings.CreatedDate,
                    UpdatedBy = trainings.UpdatedBy,
                    UpdatedDate = trainings.UpdatedDate
                });
            }
            return trainingTeamDtos;
        }

        public async Task<TrainingTeamDTO> Get(string id)
        {
            var trainings = await _context.TblTrainingTeam
                .Include(e => e.Employee)
                .Include(e => e.Trainings)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainings == null)
                return null;

            return new TrainingTeamDTO
            {
                Id = trainings.Id,
                Trainings = trainings.Trainings?.Topic,
                Employee = trainings.Employee?.Name,
                IsActive = trainings.IsActive,
                CreatedBy = trainings.CreatedBy,
                CreatedDate = trainings.CreatedDate,
                UpdatedBy = trainings.UpdatedBy,
                UpdatedDate = trainings.UpdatedDate
            };
        }

        public async Task<TrainingTeamDTO> Add(TrainingTeamDTO _object)
        {
            var employee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.Employee);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var trainings = await _context.TblTrainings
                .FirstOrDefaultAsync(d => d.Topic == _object.Trainings);

            if (trainings == null)
                throw new KeyNotFoundException("Trainings not found");

            var trainingTeam = new TrainingTeam
            {
                TrainingId = trainings.Id,
                EmployeeId = employee.Id,
                IsActive = _object.IsActive,
                CreatedBy = _object.CreatedBy,
                CreatedDate = _object.CreatedDate,
                UpdatedBy = _object.UpdatedBy,
                UpdatedDate = _object.UpdatedDate
            };

            _context.TblTrainingTeam.Add(trainingTeam);
            await _context.SaveChangesAsync();

            _object.Id = trainingTeam.Id;
            return _object;
        }

        public async Task<TrainingTeamDTO> Update(TrainingTeamDTO _object)
        {
            var trainingTeam = await _context.TblTrainingTeam.FindAsync(_object.Id);

            if (trainingTeam == null)
                throw new KeyNotFoundException("TrainingTeam not found");

            var employee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.Employee);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var trainings = await _context.TblTrainings
                .FirstOrDefaultAsync(d => d.Topic == _object.Trainings);

            if (trainings == null)
                throw new KeyNotFoundException("Trainings not found");

            trainingTeam.TrainingId = trainings?.Id;
            trainingTeam.EmployeeId = employee?.Id;
            trainingTeam.IsActive = _object.IsActive;
            trainingTeam.CreatedBy = _object.CreatedBy;
            trainingTeam.CreatedDate = _object.CreatedDate;
            trainingTeam.UpdatedBy = _object.UpdatedBy;
            trainingTeam.UpdatedDate = _object.UpdatedDate;

            _context.Entry(trainings).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            // Check if the Blogs exists
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"Blogs with ID {id} not found.");
            }
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}
