using DataServices.Models;

namespace EmployeeApi.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAll();
        Task<EmployeeDTO> Get(string id);
        Task<EmployeeDTO> Add(EmployeeDTO employee);
        Task<string> UploadFileAsync(EmployeeProfileDTO employee);
        Task<EmployeeDTO> Update(EmployeeDTO employee);
        Task<bool> Delete(string id);

    }
}
