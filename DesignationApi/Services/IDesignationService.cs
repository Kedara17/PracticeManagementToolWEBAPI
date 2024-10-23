using DataServices.Models;

namespace DesignationApi.Services
{
    public interface IDesignationService
    {
        public Task<IEnumerable<DesignationDTO>> GetAll();
        public Task<DesignationDTO> Get(string id);
        public Task<DesignationDTO> Add(DesignationDTO _object);
        public Task<DesignationDTO> Update(DesignationDTO _object);
        public Task<bool> Delete(string id);
        public Task<DesignationDTO> GetByName(string name);
    }
}
