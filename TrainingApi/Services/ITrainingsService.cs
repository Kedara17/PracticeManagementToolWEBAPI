using DataServices.Models;

namespace TrainingApi.Services
{
    public interface ITrainingsService
    {
        Task<IEnumerable<TrainingsDTO>> GetAll();
        Task<TrainingsDTO> Get(string id);
        Task<TrainingsDTO> Add(TrainingsDTO _object);
        Task<TrainingsDTO> Update(TrainingsDTO _object);
        Task<bool> Delete(string id);
    }
}



