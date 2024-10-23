//ISuccessStoriesRepository


namespace SuccessStoriesApi.Services
{
    public interface ISuccessStoriesService
    {
        Task<IEnumerable<SuccessStoriesDTO>> GetAll();
        Task<SuccessStoriesDTO> Get(string id);
        Task<SuccessStoriesDTO> Add(SuccessStoriesDTO successStories);
        Task<SuccessStoriesDTO> Update(SuccessStoriesDTO successStories);
        Task<bool> Delete(string id);
    }
}
