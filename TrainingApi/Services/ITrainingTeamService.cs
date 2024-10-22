using DataServices.Models;

namespace TrainingApi.Services
{
    public interface ITrainingTeamService
    {
        Task<IEnumerable<TrainingTeamDTO>> GetAll();
        Task<TrainingTeamDTO> Get(string id);
        Task<TrainingTeamDTO> Add(TrainingTeamDTO _object);
        Task<TrainingTeamDTO> Update(TrainingTeamDTO _object);
        Task<bool> Delete(string id);
    }
}
