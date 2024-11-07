using DataServices.Models;

namespace WebinarsApi.Services
{
    public interface IWebinarService
    {
        public Task<IEnumerable<WebinarsDTO>> GetAll();
        public Task<WebinarsDTO> Get(string id);
        public Task<WebinarsDTO> Add(WebinarsDTO _object);
        public Task<WebinarsDTO> Update(WebinarsDTO _object);
        public Task<bool> Delete(string id);
    }
}
