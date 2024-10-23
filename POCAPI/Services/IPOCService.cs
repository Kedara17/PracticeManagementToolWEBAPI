using DataServices.Models;

namespace POCAPI.Services
{
    public interface IPOCService
    {
        Task<IEnumerable<POCDTO>> GetAll();
        Task<POCDTO> Get(string id);
        Task<POCDTO> Add(POCDTO _object);
        Task<string> UploadFileAsync(POCDocumentDTO poc);
        Task<POCDTO> Update(POCDTO _object);
        Task<bool> Delete(string id);
    }
}
