using DataServices.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAll();
        Task<EmployeeDTO> Get(string id);
        Task<EmployeeDTO> Add(EmployeeDTO employee);
        Task<string> UploadFileAsync(EmployeeProfileDTO employee);
        Task<FileContentResult> DownloadFileAsync(string filename);
        Task<EmployeeDTO> Update(EmployeeDTO employee);
        Task<bool> Delete(string id);
        public Task Activate(string id);

    }
}