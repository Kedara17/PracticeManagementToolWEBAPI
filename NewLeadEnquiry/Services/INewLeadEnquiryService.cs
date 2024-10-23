using DataServices.Models;

namespace NewLeadApi.Services
{
    public interface INewLeadEnquiryService
    {
        public Task<IEnumerable<NewLeadEnquiryDTO>> GetAll();
        public Task<NewLeadEnquiryDTO> Get(string id);
        public Task<NewLeadEnquiryDTO> Add(NewLeadEnquiryDTO dto);
        Task<string> UploadFileAsync(NewLeadEnquiryProfileDTO newLeadEnquiryProfile);
        public Task<NewLeadEnquiryDTO> Update(NewLeadEnquiryDTO dto);
        public Task<bool> Delete(string id);
    }
}
