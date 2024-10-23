using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DataBaseContext _context;
        private readonly IRepository<Employee> _repository;

        public EmployeeService(DataBaseContext context, IRepository<Employee> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAll()
        {
            var employees = await _context.TblEmployee
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Roles)
                .Include(e => e.ReportingToEmployee)
                .ToListAsync();

            var empDtos = employees.Select(employee => new EmployeeDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                Designation = employee.Designation?.Name,
                EmployeeID = employee.EmployeeID,
                EmailId = employee.EmailId,
                Department = employee.Department?.Name,
                ReportingTo = employee.ReportingToEmployee?.Name,
                JoiningDate = employee.JoiningDate,
                RelievingDate = employee.RelievingDate,
                Projection = employee.Projection,
                IsActive = employee.IsActive,
                CreatedBy = employee.CreatedBy,
                CreatedDate = employee.CreatedDate,
                UpdatedBy = employee.UpdatedBy,
                UpdatedDate = employee.UpdatedDate,
                Profile = employee.Profile,
                PhoneNo = employee.PhoneNo,
                Role = employee.Roles?.RoleName
            }).ToList();

            return empDtos;
        }

        public async Task<EmployeeDTO> Get(string id)
        {
            var employee = await _context.TblEmployee
                  .Include(e => e.Department)
                  .Include(e => e.Designation)
                  .Include(e => e.Roles)
                  .Include(e => e.ReportingToEmployee)
                  .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return null;

            return new EmployeeDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                Designation = employee.Designation?.Name,
                EmployeeID = employee.EmployeeID,
                EmailId = employee.EmailId,
                Department = employee.Department?.Name,
                ReportingTo = employee.ReportingToEmployee?.Name,
                JoiningDate = employee.JoiningDate,
                RelievingDate = employee.RelievingDate,
                Projection = employee.Projection,
                IsActive = employee.IsActive,
                CreatedBy = employee.CreatedBy,
                CreatedDate = employee.CreatedDate,
                UpdatedBy = employee.UpdatedBy,
                UpdatedDate = employee.UpdatedDate,
                Profile = employee.Profile,
                PhoneNo = employee.PhoneNo,
                Role = employee.Roles?.RoleName
            };
        }

        public async Task<EmployeeDTO> Add(EmployeeDTO empDto)
        {
            // Check if Employee name is unique
            var existingEmployeeName = await _context.TblEmployee
                .FirstOrDefaultAsync(e => e.Name == empDto.Name);

            if (existingEmployeeName != null)
            {
                throw new ArgumentException("Employee Name must be unique. This Employee Name is already in use.");
            }

            // Check if EmployeeID is unique
            var existingEmployeeID = await _context.TblEmployee
                .FirstOrDefaultAsync(e => e.EmployeeID == empDto.EmployeeID);

            if (existingEmployeeID != null)
            {
                throw new ArgumentException("EmployeeID must be unique. This EmployeeID is already in use.");
            }

            // Check if EmailId is unique
            var existingEmailId = await _context.TblEmployee
                .FirstOrDefaultAsync(e => e.EmailId== empDto.EmailId);

            if (existingEmailId != null)
            {
                throw new ArgumentException("EmailId must be unique. This EmailId is already in use.");
            }

            // Check if phone number is unique
            var existingEmployeeWithSamePhoneNo = await _context.TblEmployee
                .FirstOrDefaultAsync(e => e.PhoneNo == empDto.PhoneNo);

            if (existingEmployeeWithSamePhoneNo != null)
            {
                throw new ArgumentException("Phone number must be unique. This phone number is already in use.");
            }

            var employee = new Employee();
            
            var department = await _context.TblDepartment
               .FirstOrDefaultAsync(d => d.Name == empDto.Department);
            if (department == null)
                throw new KeyNotFoundException("Department not found");

            var designation = await _context.TblDesignation
               .FirstOrDefaultAsync(d => d.Name == empDto.Designation);
            if (designation == null)
                throw new KeyNotFoundException("Designation not found");

            var reportingTo = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == empDto.ReportingTo);
            if (reportingTo == null)
                throw new KeyNotFoundException("ReportingTo not found");

            var role = await _context.TblRole
               .FirstOrDefaultAsync(r => r.RoleName == empDto.Role);
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            employee.Name = empDto.Name;
            employee.DesignationId = designation.Id;
            employee.EmployeeID = empDto.EmployeeID;
            employee.EmailId = empDto.EmailId;
            employee.DepartmentId = department.Id;
            employee.ReportingTo = reportingTo.Id;
            employee.JoiningDate = empDto.JoiningDate;
            employee.RelievingDate = empDto.RelievingDate;
            employee.Projection = empDto.Projection;
            employee.IsActive = empDto.IsActive;
            employee.CreatedBy = empDto.CreatedBy;
            employee.CreatedDate = empDto.CreatedDate;
            employee.UpdatedBy = empDto.UpdatedBy;
            employee.UpdatedDate = empDto.UpdatedDate;
            employee.Password = PasswordHasher.HashPassword(empDto.Password);
            employee.Profile = empDto.Profile;
            employee.PhoneNo = empDto.PhoneNo;
            employee.Role = role.Id;

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(empDto.Profile))
            {
                employee.Profile = empDto.Profile;
            }


            _context.TblEmployee.Add(employee);
            await _context.SaveChangesAsync();
            empDto.Id = employee.Id;

            if (empDto.Technology != null && empDto.Technology.Any())
            {
                foreach (var technologyId in empDto.Technology)
                {
                    var employeeTechnology = new EmployeeTechnology
                    {
                        EmployeeID = employee.Id,
                        Technology = technologyId.ToString(),
                    };

                    await _context.TblEmployeeTechnology.AddAsync(employeeTechnology);
                }

                await _context.SaveChangesAsync();
            }
            return empDto;
        }

        public async Task<string> UploadFileAsync(EmployeeProfileDTO employeeProfile)
        {
            string filePath = "";
            try
            {
                // Check if the file is not empty
                if (employeeProfile.Profile.Length > 0)
                {
                    var file = employeeProfile.Profile;
                    filePath = Path.GetFullPath($"C:\\Users\\mshaik5\\Desktop\\UploadProfiles\\{file.FileName}");

                    // Save file to the specified path
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Update employee's profile if ID is provided
                    if (!string.IsNullOrEmpty(employeeProfile.Id))
                    {
                        var employee = await Get(employeeProfile.Id);

                        if (employee != null)
                        {
                            employee.Profile = file.FileName;
                            await Update(employee);
                        }
                    }
                    else
                    {
                        return file.FileName;
                    }
                }
                else
                {
                    throw new Exception("The uploaded file is empty.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while uploading the file: " + ex.Message);
            }

            return filePath;
        }

        public async Task<EmployeeDTO> Update(EmployeeDTO empDto)
        {
            // Check if Employee name is unique
            var existingEmployeeName = await _context.TblEmployee
                .FirstOrDefaultAsync(e => e.Name == empDto.Name);

            if (existingEmployeeName != null)
            {
                throw new ArgumentException("Employee Name must be unique. This Employee Name is already in use.");
            }

            // Check if EmployeeID is unique
            var existingEmployeeID = await _context.TblEmployee
                .FirstOrDefaultAsync(e => e.EmployeeID == empDto.EmployeeID);

            if (existingEmployeeID != null)
            {
                throw new ArgumentException("EmployeeID must be unique. This EmployeeID is already in use.");
            }

            // Check if EmailId is unique
            var existingEmailId = await _context.TblEmployee
                .FirstOrDefaultAsync(e => e.EmailId == empDto.EmailId);

            if (existingEmailId != null)
            {
                throw new ArgumentException("EmailId must be unique. This EmailId is already in use.");
            }

            // Check if phone number is unique
            var existingEmployeeWithSamePhoneNo = await _context.TblEmployee
                .FirstOrDefaultAsync(e => e.PhoneNo == empDto.PhoneNo);

            if (existingEmployeeWithSamePhoneNo != null)
            {
                throw new ArgumentException("Phone number must be unique. This phone number is already in use.");
            }
            var employee = await _context.TblEmployee.FindAsync(empDto.Id);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var department = await _context.TblDepartment
                .FirstOrDefaultAsync(d => d.Name == empDto.Department);
            if (department == null)
                throw new KeyNotFoundException("Department not found");

            var reportingTo = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == empDto.ReportingTo);
            if (reportingTo == null)
                throw new KeyNotFoundException("ReportingTo not found");

            var designation = await _context.TblDesignation
                .FirstOrDefaultAsync(d => d.Name == empDto.Designation);
            if (designation == null)
                throw new KeyNotFoundException("Designation not found");

            var role = await _context.TblRole
               .FirstOrDefaultAsync(r => r.RoleName == empDto.Role);
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            employee.Name = empDto.Name;
            employee.DesignationId = designation.Id;
            employee.EmployeeID = empDto.EmployeeID;
            employee.EmailId = empDto.EmailId;
            employee.DepartmentId = department.Id;
            employee.ReportingTo = reportingTo.Id;
            employee.JoiningDate = empDto.JoiningDate;
            employee.RelievingDate = empDto.RelievingDate;
            employee.Projection = empDto.Projection;
            employee.IsActive = empDto.IsActive;
            employee.CreatedBy = empDto.CreatedBy;
            employee.CreatedDate = empDto.CreatedDate;
            employee.UpdatedBy = empDto.UpdatedBy;
            employee.UpdatedDate = empDto.UpdatedDate;
            employee.Password = PasswordHasher.HashPassword(empDto.Password);
            employee.Profile = empDto.Profile;
            employee.PhoneNo = empDto.PhoneNo;
            employee.Role = role.Id;

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(empDto.Profile))
            {
                employee.Profile = empDto.Profile;
            }


            _context.Entry(employee).State = EntityState.Modified;

            if (empDto.Technology != null && empDto.Technology.Any())
            {
                // Remove old technologies
                var employeeTechnologies = await _context.TblEmployeeTechnology
                    .Where(et => et.EmployeeID == empDto.Id)
                    .ToListAsync();
                _context.TblEmployeeTechnology.RemoveRange(employeeTechnologies);

                // Add updated technologies
                foreach (var technologyId in empDto.Technology)
                {
                    var employeeTechnology = new EmployeeTechnology
                    {
                        EmployeeID = employee.Id,
                        Technology = technologyId.ToString(),
                    };

                    await _context.TblEmployeeTechnology.AddAsync(employeeTechnology);
                }
            }

            await _context.SaveChangesAsync();

            return empDto;
        }

        public async Task<bool> Delete(string id)
        {
            /*            var employee = await _context.TblEmployee.FindAsync(id);
                        if (employee == null) return false;
                        _context.TblEmployee.Remove(employee);
                        await _context.SaveChangesAsync();
                        return true;*/

            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"with ID {id} not found.");
            }
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}
