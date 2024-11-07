using DataServices.Models;

namespace ClientServices.Services
{
    public interface IClientService
    {
        public Task<IEnumerable<ClientDTO>> GetAll();
        public Task<ClientDTO> Get(string id);
        public Task<ClientDTO> Add(ClientDTO client);
        public Task<ClientDTO> Update(ClientDTO client);
        public Task<bool> Delete(string id);

    }

}
