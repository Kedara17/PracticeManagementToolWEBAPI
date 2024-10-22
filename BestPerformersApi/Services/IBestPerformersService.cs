using DataServices.Models;

namespace BestPerformersAPI.Services
{
    public interface IBestPerformersService
    {
        Task<IEnumerable<BestPerformersDTO>> GetAll();
        Task<BestPerformersDTO> Get(string id);
        Task<BestPerformersDTO> Add(BestPerformersDTO _object);
        Task<BestPerformersDTO> Update(BestPerformersDTO _object, string userRole);
        Task<bool> Delete(string id);
    }
}
