using DataServices.Models;
using DataServices.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechnologyApi.Services
{
    public interface ITechnologyService
    {
        Task<IEnumerable<Technology>> GetAll();
        Task<Technology> Get(string id);
        Task<TechnologyViewModel> Add(TechnologyViewModel technology);
        Task<bool> Update(string id,TechnologyViewModel technology);
        Task<bool> Delete(string id);
    }
}