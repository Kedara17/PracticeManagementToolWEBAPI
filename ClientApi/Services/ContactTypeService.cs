using ClientServices.Services;
using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Services
{
    public class ContactTypeService : IContactTypeService
    {
        private readonly IRepository<ContactType> _repository;
        private readonly DataBaseContext _context;

        public ContactTypeService(IRepository<ContactType> repository,DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ContactType>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<ContactType> Get(string id)
        {
            return await _repository.Get(id);
        }

        public async Task<ContactType> Add(ContactType _object)
        {
            // Check if the ContactType name already exists
            var existingContactType = await _context.TblContactType
                .FirstOrDefaultAsync(t => t.TypeName == _object.TypeName);

            if (existingContactType != null)
                throw new ArgumentException("A ContactType with the same name already exists.");

            return await _repository.Create(_object);
        }

        public async Task<ContactType> Update(ContactType _object)
        {
            // Check if the ContactType name already exists
            var existingContactType = await _context.TblContactType
                .FirstOrDefaultAsync(t => t.TypeName == _object.TypeName);

            if (existingContactType != null)
                throw new ArgumentException("A ContactType with the same name already exists.");

            return await _repository.Update(_object);
        }

        public async Task<bool> Delete(string id)
        {
            // Check if the technology exists
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"Technology with ID {id} not found.");
            }

            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}




