using DataServices.Models;

namespace ProjectApi.Services
{
    public interface IProjectService
    {
        public Task<IEnumerable<ProjectDTO>> GetAll();
        public Task<ProjectDTO> Get(string id);
        public Task<ProjectDTO> Add(ProjectDTO _object);
        public Task<ProjectDTO> Update(ProjectDTO _object);
        public Task<bool> Delete(string id);
    }
}
