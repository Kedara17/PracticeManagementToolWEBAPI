using ClientServices.Services;
using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Services
{
    public class ClientContactService : IClientContactService
    {
        private readonly IRepository<ClientContact> _repository;
        private readonly DataBaseContext _context;

        public ClientContactService(IRepository<ClientContact> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ClientContactDTO>> GetAll()
        {
            var clientContacts = await _context.TblClientContact
                .Include(c => c.Client)
                .Include(c => c.ContactType)
                .ToListAsync();
            var ccDTO = new List<ClientContactDTO>();

            foreach (var contact in clientContacts)
            {
                ccDTO.Add(new ClientContactDTO
                {
                    Id = contact.Id,
                    Client = contact.Client?.Name,
                    ContactValue = contact.ContactValue,
                    ContactType = contact.ContactType?.TypeName,
                    IsActive = contact.IsActive,
                    CreatedBy = contact.CreatedBy,
                    CreatedDate = contact.CreatedDate,
                    UpdatedBy = contact.UpdatedBy,
                    UpdatedDate = contact.UpdatedDate
                });
            }

            return ccDTO;
        }

        public async Task<ClientContactDTO> Get(string id)
        {
            var clientContact = await _context.TblClientContact
                .Include(e => e.Client)
                .Include(e => e.ContactType)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (clientContact == null)
                return null;

            return new ClientContactDTO
            {
                Id = clientContact.Id,
                Client = clientContact.Client?.Name,
                ContactValue = clientContact.ContactValue,
                ContactType = clientContact.ContactType?.TypeName,
                IsActive = clientContact.IsActive,
                CreatedBy = clientContact.CreatedBy,
                CreatedDate = clientContact.CreatedDate,
                UpdatedBy = clientContact.UpdatedBy,
                UpdatedDate = clientContact.UpdatedDate
            };

        }

        public async Task<ClientContactDTO> Add(ClientContactDTO clientContactDTO)
        {
            // Check if the ContactValue name already exists
            var existingContactValue = await _context.TblClientContact
                .FirstOrDefaultAsync(t => t.ContactValue == clientContactDTO.ContactValue);

            if (existingContactValue != null)
                throw new ArgumentException("A ContactValue with the same name already exists.");

            var client = await _context.TblClient
                .FirstOrDefaultAsync(e => e.Name == clientContactDTO.Client);

            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var contactType = await _context.TblContactType
               .FirstOrDefaultAsync(e => e.TypeName == clientContactDTO.ContactType);

            if (contactType == null)
                throw new KeyNotFoundException("ContactType not found");

            var clientContact = new ClientContact
            {
                ClientId = client.Id,
                ContactValue = clientContactDTO.ContactValue,
                ContactTypeId = contactType.Id,
                IsActive = clientContactDTO.IsActive,
                CreatedBy = clientContactDTO.CreatedBy,
                CreatedDate = clientContactDTO.CreatedDate,
                UpdatedBy = clientContactDTO.UpdatedBy,
                UpdatedDate = clientContactDTO.UpdatedDate
            };

            _context.TblClientContact.Add(clientContact);
            await _context.SaveChangesAsync();

            clientContactDTO.Id = clientContact.Id;
            return clientContactDTO;

        }

        public async Task<ClientContactDTO> Update(ClientContactDTO clientContactDTO)
        {
            var clientContact = await _context.TblClientContact.FindAsync(clientContactDTO.Id);
            // Check if the ContactValue name already exists
            var existingContactValue = await _context.TblClientContact
                .FirstOrDefaultAsync(t => t.ContactValue == clientContactDTO.ContactValue);

            if (existingContactValue != null)
                throw new ArgumentException("A ContactValue with the same name already exists.");

            if (clientContact == null)
                throw new KeyNotFoundException("ClientContact not found");

            var client = await _context.TblClient
                .FirstOrDefaultAsync(d => d.Name == clientContactDTO.Client);

            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var contactType = await _context.TblContactType
               .FirstOrDefaultAsync(d => d.TypeName == clientContactDTO.ContactType);

            if (contactType == null)
                throw new KeyNotFoundException("ContactType not found");

            clientContact.ClientId = client.Id;
            clientContact.ContactValue = clientContactDTO.ContactValue;
            clientContact.ContactTypeId = contactType.Id;
            clientContact.IsActive = clientContactDTO.IsActive;
            clientContact.CreatedBy = clientContactDTO.CreatedBy;
            clientContact.CreatedDate = clientContactDTO.CreatedDate;
            clientContact.UpdatedBy = clientContactDTO.UpdatedBy;
            clientContact.UpdatedDate = clientContactDTO.UpdatedDate;

            _context.Entry(clientContact).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return clientContactDTO;
        }

        public async Task<bool> Delete(string id)
        {
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"Client with ID {id} not found.");
            }
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}



