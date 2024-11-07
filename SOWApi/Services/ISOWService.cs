using DataServices.Models;

namespace SOWApi.Services
{
    public interface ISOWService
    {
        Task<IEnumerable<SOWDTO>> GetAll();
        Task<SOWDTO> Get(string id);
        Task<SOWDTO> Add(SOWDTO sow);
        Task<SOWDTO> Update(SOWDTO sow);
        Task<bool> Delete(string id);
    }
}
