using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SOWApi.Services
{
    public class SOWProposedTeamService : ISOWProposedTeamService
    {
        private readonly IRepository<SOWProposedTeam> _repository;
        private readonly DataBaseContext _context;

        public SOWProposedTeamService(IRepository<SOWProposedTeam> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<SOWProposedTeamDTO>> GetAll()
        {
            var sowProposedTeams = await _context.TblSOWProposedTeam
               .Include(c => c.SOWRequirement)
               .Include(t => t.Employee)
               .ToListAsync();

            var sowproposedTeamDto = new List<SOWProposedTeamDTO>();
            foreach (var item in sowProposedTeams)
            {
                sowproposedTeamDto.Add(new SOWProposedTeamDTO
                {
                    Id = item.Id,
                    SOWRequirement = item.SOWRequirement?.TeamSize.ToString(),
                    Employee = item.Employee?.Name,
                    IsActive = item.IsActive,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate
                });
            }
            return sowproposedTeamDto;
        }

        public async Task<SOWProposedTeamDTO> Get(string id)
        {
            var sowProposedTeams = await _context.TblSOWProposedTeam
               .Include(c => c.SOWRequirement)
               .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (sowProposedTeams == null) return null;

            return new SOWProposedTeamDTO
            {
                Id = sowProposedTeams.Id,
                SOWRequirement = sowProposedTeams.SOWRequirement?.TeamSize.ToString(),
                Employee = sowProposedTeams.Employee?.Name,
                IsActive = sowProposedTeams.IsActive,
                CreatedBy = sowProposedTeams.CreatedBy,
                CreatedDate = sowProposedTeams.CreatedDate,
                UpdatedBy = sowProposedTeams.UpdatedBy,
                UpdatedDate = sowProposedTeams.UpdatedDate
            };
        }

        public async Task<SOWProposedTeamDTO> Add(SOWProposedTeamDTO _object)
        {
            var sowProposedTeam = new SOWProposedTeam();
            /* var sowRequirement = await _context.TblSOWRequirement
                .FirstOrDefaultAsync(d => d.TeamSize.ToString() == _object.SOWRequirement);

             if (sowRequirement == null)
                 throw new KeyNotFoundException("SowRequirement not found");

             var employee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.Employee);

             if (employee == null)
                 throw new KeyNotFoundException("Employee not found");*/

           
            //_________________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.SOWRequirement))
            {
                var sowRequirementExists = await _context.TblSOWRequirement
                    .AnyAsync(d => d.Id == _object.SOWRequirement);
                if (!sowRequirementExists)
                    throw new ArgumentException("The specified project does not exist.");

                sowProposedTeam.SOWRequirementId = _object.SOWRequirement;
            }
            else
            {
                sowProposedTeam.SOWRequirementId = null;
            }
            //_________________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.Employee))
            {
                var employeeExists = await _context.TblEmployee
                    .AnyAsync(d => d.Id == _object.Employee);
                if (!employeeExists)
                    throw new ArgumentException("The specified employee does not exist.");

                sowProposedTeam.EmployeeId = _object.Employee;
            }
            else
            {
                sowProposedTeam.EmployeeId = null;
            }


            /*sowProposedTeam.SOWRequirementId = _object.SOWRequirement;
            sowProposedTeam.EmployeeId = _object.Employee;*/
            sowProposedTeam.IsActive = _object.IsActive;
            sowProposedTeam.CreatedBy = _object.CreatedBy;
            sowProposedTeam.CreatedDate = _object.CreatedDate;
            sowProposedTeam.UpdatedBy = _object.UpdatedBy;
            sowProposedTeam.UpdatedDate = _object.UpdatedDate;
           

            _context.TblSOWProposedTeam.Add(sowProposedTeam);
            await _context.SaveChangesAsync();

            _object.Id = sowProposedTeam.Id;
            return _object;
        }

        public async Task<SOWProposedTeamDTO> Update(SOWProposedTeamDTO _object)
        {
            var sowProposedTeam = await _context.TblSOWProposedTeam.FindAsync(_object.Id);

            if (sowProposedTeam == null)
                throw new KeyNotFoundException("sowProposedTeam not found");

            /* var sowRequirement = await _context.TblSOWRequirement
                .FirstOrDefaultAsync(d => d.TeamSize.ToString() == _object.SOWRequirement);

             if (sowRequirement == null)
                 throw new KeyNotFoundException("SowRequirement not found");

             var employee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.Employee);

             if (employee == null)
                 throw new KeyNotFoundException("Employee not found");
 */
            //_________________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.SOWRequirement))
            {
                var sowRequirementExists = await _context.TblSOWRequirement
                    .AnyAsync(d => d.Id == _object.SOWRequirement);
                if (!sowRequirementExists)
                    throw new ArgumentException("The specified project does not exist.");

                sowProposedTeam.SOWRequirementId = _object.SOWRequirement;
            }
            else
            {
                sowProposedTeam.SOWRequirementId = null;
            }
            //_________________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.Employee))
            {
                var employeeExists = await _context.TblEmployee
                    .AnyAsync(d => d.Id == _object.Employee);
                if (!employeeExists)
                    throw new ArgumentException("The specified employee does not exist.");

                sowProposedTeam.EmployeeId = _object.Employee;
            }
            else
            {
                sowProposedTeam.EmployeeId = null;
            }


            /*sowProposedTeam.SOWRequirementId = _object.SOWRequirement;
            sowProposedTeam.EmployeeId = _object.Employee;*/
            sowProposedTeam.IsActive = _object.IsActive;
            sowProposedTeam.CreatedBy = _object.CreatedBy;
            sowProposedTeam.CreatedDate = _object.CreatedDate;
            sowProposedTeam.UpdatedBy = _object.UpdatedBy;
            sowProposedTeam.UpdatedDate = _object.UpdatedDate;


            _context.Entry(sowProposedTeam).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            var existingsowproposedteam = await _repository.Get(id);
            if (existingsowproposedteam == null)
            {
                throw new ArgumentException($"Technology with ID {id} not found.");
            }
            existingsowproposedteam.IsActive = false; // Soft delete
            await _repository.Update(existingsowproposedteam); // Save changes
            return true;
        }
    }
}
