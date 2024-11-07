using DataServices.Models;

namespace InterviewApi.Services
{
    public interface IInterviewService
    {
        public Task<IEnumerable<InterviewsDTO>> GetAll();
        public Task<InterviewsDTO> Get(string id);
        public Task<InterviewsDTO> Add(InterviewsDTO interview);
        public Task<InterviewsDTO> Update(InterviewsDTO interview);
        public Task<bool> Delete(string id);
    }
}
