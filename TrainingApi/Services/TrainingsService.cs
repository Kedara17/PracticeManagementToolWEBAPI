using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TrainingApi.Services
{
    public class TrainingsService : ITrainingsService
    {
        private readonly IRepository<Trainings> _repository;
        private readonly DataBaseContext _context;

        public TrainingsService(IRepository<Trainings> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<TrainingsDTO>> GetAll()
        {
            var trainings1 = await _context.TblTrainings.Include(e => e.Employee).ToListAsync();
            var trainingsDtos = new List<TrainingsDTO>();

            foreach (var trainings in trainings1)
            {
                trainingsDtos.Add(new TrainingsDTO
                {
                    Id = trainings.Id,
                    Topic = trainings.Topic,
                    Employee = trainings.Employee?.Name,
                    StartDate = trainings.StartDate,
                    EndDate = trainings.EndDate,
                    Status = trainings.Status,
                    Comments = trainings.Comments,
                    IsActive = trainings.IsActive,
                    CreatedBy = trainings.CreatedBy,
                    CreatedDate = trainings.CreatedDate,
                    UpdatedBy = trainings.UpdatedBy,
                    UpdatedDate = trainings.UpdatedDate
                });
            }
            return trainingsDtos;
        }

        public async Task<TrainingsDTO> Get(string id)
        {
            var trainings = await _context.TblTrainings
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainings == null)
                return null;

            return new TrainingsDTO
            {
                Id = trainings.Id,
                Topic = trainings.Topic,
                Employee = trainings.Employee?.Name,
                StartDate = trainings.StartDate,
                EndDate = trainings.EndDate,
                Status = trainings.Status,
                Comments = trainings.Comments,
                IsActive = trainings.IsActive,
                CreatedBy = trainings.CreatedBy,
                CreatedDate = trainings.CreatedDate,
                UpdatedBy = trainings.UpdatedBy,
                UpdatedDate = trainings.UpdatedDate
            };
        }

        public async Task<TrainingsDTO> Add(TrainingsDTO _object)
        {
            var employee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.Employee);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var trainings = new Trainings
            {
                Topic = _object.Topic,
                EmployeeId = employee.Id,
                StartDate = _object.StartDate,
                EndDate = _object.EndDate,
                Status = _object.Status,
                Comments = _object.Comments,
                IsActive = _object.IsActive,
                CreatedBy = _object.CreatedBy,
                CreatedDate =_object.CreatedDate,
                UpdatedBy = _object.UpdatedBy,
                UpdatedDate = _object.UpdatedDate
            };

            _context.TblTrainings.Add(trainings);
            await _context.SaveChangesAsync();

            _object.Id = trainings.Id;
            return _object;
        }

        public async Task<TrainingsDTO> Update(TrainingsDTO _object)
        {
            var trainings = await _context.TblTrainings.FindAsync(_object.Id);

            if (trainings == null)
                throw new KeyNotFoundException("Trainings not found");

            var employee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.Employee);

            if (employee == null)
                throw new KeyNotFoundException("Author not found");

            trainings.Topic = _object.Topic;
            trainings.EmployeeId = employee?.Id;
            trainings.StartDate = _object.StartDate;         
            trainings.EndDate = _object.EndDate;
            trainings.Status = _object.Status;
            trainings.Comments = _object.Comments;
            trainings.IsActive = _object.IsActive;
            trainings.CreatedBy = _object.CreatedBy;
            trainings.CreatedDate =_object.CreatedDate;
            trainings.UpdatedBy = _object.UpdatedBy;
            trainings.UpdatedDate = _object.UpdatedDate;

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
