using DataServices.Models;

namespace BlogsApi.Services
{
    public interface IBlogsService
    {
        Task<IEnumerable<BlogsDTO>> GetAll();
        Task<BlogsDTO> Get(string id);
        Task<BlogsDTO> Add(BlogsDTO _object);
        Task<BlogsDTO> Update(BlogsDTO _object, string userRole);
        Task<bool> Delete(string id);
    }
}
