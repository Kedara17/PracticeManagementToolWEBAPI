using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DataBaseContext _context;
        private readonly IRepository<Employee> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(DataBaseContext context, IRepository<Employee> repository, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
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
            var employeeName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;

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
            //----------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Designation))
            {
                var designation = await _context.TblDesignation
                    .FirstOrDefaultAsync(d => d.Name == empDto.Designation);
                if (designation == null)
                {
                    throw new ArgumentException($"Invalid designation. Please enter a valid designation.");
                }
                employee.DesignationId = designation.Id;
            }
            else
            {
                employee.DepartmentId = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Department))
            {
                var department = await _context.TblDepartment
                    .FirstOrDefaultAsync(d => d.Name == empDto.Department);
                if (department == null)
                {
                    throw new ArgumentException($"Invalid department name. Please enter a valid department name.");
                }

                employee.DepartmentId = department.Id;
            }
            else
            {
                employee.DepartmentId = null;
            }
            //------------------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.ReportingTo))
            {
                var reportingTo = await _context.TblEmployee
                    .FirstOrDefaultAsync(d => d.Name == empDto.ReportingTo);
                if (reportingTo == null)
                {
                    throw new ArgumentException($"Invalid ReportingTo name. Please enter a valid ReportingTo name.");
                }
                employee.ReportingTo = reportingTo.Id;
            }
            else
            {
                employee.ReportingTo = null;
            }
            //---------------------------------------------------------
            if (empDto.JoiningDate.HasValue)
            {
                employee.JoiningDate = empDto.JoiningDate;
            }
            else
            {
                employee.JoiningDate = null;
            }
            //---------------------------------------------------------
            if (empDto.RelievingDate.HasValue)
            {
                employee.RelievingDate = empDto.RelievingDate;
            }
            else
            {
                employee.RelievingDate = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Projection))
            {
                employee.Projection = empDto.Projection;
            }
            else
            {
                employee.Projection = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Profile))
            {
                employee.Profile = empDto.Profile;
            }
            else
            {
                employee.Profile = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.PhoneNo))
            {
                employee.PhoneNo= empDto.PhoneNo;
            }
            else
            {
                employee.PhoneNo = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Role))
            {
                var role = await _context.TblRole
                    .FirstOrDefaultAsync(d => d.RoleName == empDto.Role);
                if (role == null)
                {
                    throw new ArgumentException($"Invalid Role. Please enter a valid Role.");
                }
                employee.Role = role.Id;
            }
            else
            {
                employee.ReportingTo = null;
            }
            //---------------------------------------------------------
            employee.Name = empDto.Name;
            employee.EmployeeID = empDto.EmployeeID;
            employee.EmailId = empDto.EmailId;
            employee.IsActive = true;
            employee.CreatedBy = employeeName;
            employee.CreatedDate = DateTime.Now;
            employee.Password = PasswordHasher.HashPassword(empDto.Password);
            _context.TblEmployee.Add(employee);
            await _context.SaveChangesAsync();

            empDto.Id = employee.Id;

            // If Technology is null or contains only empty strings, treat it as null
            if (empDto.Technology == null || empDto.Technology.All(string.IsNullOrWhiteSpace))
            {
                empDto.Technology = null;
                Console.WriteLine("empDto.Technology is set to null.");
            }
            else
            {
                foreach (var technologyId in empDto.Technology)
                {
                    // Ensure you're not processing empty or whitespace strings
                    if (!string.IsNullOrWhiteSpace(technologyId))
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
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;
            
            var employee = await _context.TblEmployee.FindAsync(empDto.Id);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            //----------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Designation))
            {
                var designation = await _context.TblDesignation
                    .FirstOrDefaultAsync(d => d.Name == empDto.Designation);
                if (designation == null)
                {
                    throw new ArgumentException($"Invalid designation. Please enter a valid designation.");
                }
                employee.DesignationId = designation.Id;
            }
            else
            {
                employee.DepartmentId = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Department))
            {
                var department = await _context.TblDepartment
                    .FirstOrDefaultAsync(d => d.Name == empDto.Department);
                if (department == null)
                {
                    throw new ArgumentException($"Invalid department name. Please enter a valid department name.");
                }

                employee.DepartmentId = department.Id;
            }
            else
            {
                employee.DepartmentId = null;
            }
            //------------------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.ReportingTo))
            {
                var reportingTo = await _context.TblEmployee
                    .FirstOrDefaultAsync(d => d.Name == empDto.ReportingTo);
                if (reportingTo == null)
                {
                    throw new ArgumentException($"Invalid ReportingTo name. Please enter a valid ReportingTo name.");
                }
                employee.ReportingTo = reportingTo.Id;
            }
            else
            {
                employee.ReportingTo = null;
            }
            //---------------------------------------------------------
            if (empDto.JoiningDate.HasValue)
            {
                employee.JoiningDate = empDto.JoiningDate;
            }
            else
            {
                employee.JoiningDate = null;
            }
            //---------------------------------------------------------
            if (empDto.RelievingDate.HasValue)
            {
                employee.RelievingDate = empDto.RelievingDate;
            }
            else
            {
                employee.RelievingDate = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Projection))
            {
                employee.Projection = empDto.Projection;
            }
            else
            {
                employee.Projection = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Profile))
            {
                employee.Profile = empDto.Profile;
            }
            else
            {
                employee.Profile = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.PhoneNo))
            {
                employee.PhoneNo = empDto.PhoneNo;
            }
            else
            {
                employee.PhoneNo = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Role))
            {
                var role = await _context.TblRole
                    .FirstOrDefaultAsync(d => d.RoleName == empDto.Role);
                if (role == null)
                {
                    throw new ArgumentException($"Invalid Role. Please enter a valid Role.");
                }
                employee.Role = role.Id;
            }
            else
            {
                employee.ReportingTo = null;
            }
            //---------------------------------------------------------
            employee.Name = empDto.Name;
            employee.EmployeeID = empDto.EmployeeID;
            employee.EmailId = empDto.EmailId;
            employee.IsActive = true;
            employee.UpdatedBy = userName;
            employee.UpdatedDate = DateTime.Now;
            employee.Password = PasswordHasher.HashPassword(empDto.Password);

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(empDto.Profile))
            {
                employee.Profile = empDto.Profile;
            }


           // _context.Entry(employee).State = EntityState.Modified;

            if (empDto.Technology == null || empDto.Technology.All(string.IsNullOrWhiteSpace))
            {
                empDto.Technology = null;
                Console.WriteLine("empDto.Technology is set to null.");
            }
            else
            {
                foreach (var technologyId in empDto.Technology)
                {
                    // Ensure you're not processing empty or whitespace strings
                    if (!string.IsNullOrWhiteSpace(technologyId))
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
            }
            _context.Entry(employee).State = EntityState.Modified;
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
