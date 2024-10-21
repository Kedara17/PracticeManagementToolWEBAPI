//IcertrificationServices

using DataServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificationsApi.Services
{
    public interface ICertificationsService
    {
        Task<IEnumerable<CertificationsDTO>> GetAll();
        Task<CertificationsDTO> Get(string id);
        Task<CertificationsDTO> Add(CertificationsDTO certification);
        Task<CertificationsDTO> Update(CertificationsDTO certification);
        Task<bool> Delete(string id);
    }
}
