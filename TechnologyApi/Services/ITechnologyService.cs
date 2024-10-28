using DataServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechnologyApi.Services
{
    public interface ITechnologyService
    {
        Task<IEnumerable<TechnologyDTO>> GetAll();
        Task<TechnologyDTO> Get(string id);
        Task<TechnologyDTO> Add(TechnologyDTO technology);
        Task<TechnologyDTO> Update(TechnologyDTO technology);
        Task<bool> Delete(string id);
    }
}