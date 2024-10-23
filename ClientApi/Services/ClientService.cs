using ClientServices.Services;
using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Net;

namespace ClientApi.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _repository;
        private readonly DataBaseContext _context;

        public ClientService(IRepository<Client> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<ClientDTO>> GetAll()
        {
            var clients = await _context.TblClient.Include(e => e.Employee).ToListAsync();
            var clientDtos = new List<ClientDTO>();

            foreach (var client in clients)
            {
                clientDtos.Add(new ClientDTO
                {
                    Id = client.Id,
                    Name = client.Name,
                    LineofBusiness = client.LineofBusiness,
                    SalesEmployee = client.Employee?.Name,
                    Country = client.Country,
                    City = client.City,
                    State = client.State,
                    Address = client.Address,
                    IsActive = client.IsActive,
                    CreatedBy = client.CreatedBy,
                    CreatedDate = client.CreatedDate,
                    UpdatedBy = client.UpdatedBy,
                    UpdatedDate = client.UpdatedDate
                });
            }
            return clientDtos;
        }

        public async Task<ClientDTO> Get(string id)
        {
            var client = await _context.TblClient
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (client == null)
                return null;

            return new ClientDTO
            {
                Id = client.Id,
                Name = client.Name,
                LineofBusiness = client.LineofBusiness,
                SalesEmployee = client.Employee?.Name,
                Country = client.Country,
                City = client.City,
                State = client.State,
                Address = client.Address,
                IsActive = client.IsActive,
                CreatedBy = client.CreatedBy,
                CreatedDate = client.CreatedDate,
                UpdatedBy = client.UpdatedBy,
                UpdatedDate = client.UpdatedDate
            };
        }

        public async Task<ClientDTO> Add(ClientDTO _object)
        {
            // Check if the Client name already exists
            var existingClient = await _context.TblClient
                .FirstOrDefaultAsync(t => t.Name == _object.Name);

            if (existingClient != null)
                throw new ArgumentException("A client with the same name already exists.");

            var salesEmployee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.SalesEmployee);

            if (salesEmployee == null)
                throw new KeyNotFoundException("SalesEmployee not found");

            var client = new Client
            {
                Name = _object.Name,
                LineofBusiness = _object.LineofBusiness,
                SalesEmployee = salesEmployee.Id,
                Country = _object.Country,
                City = _object.City,
                State = _object.State,
                Address = _object.Address,
                IsActive = _object.IsActive,
                CreatedBy= _object.CreatedBy,
                CreatedDate = _object.CreatedDate,
                UpdatedBy = _object.UpdatedBy,
                UpdatedDate = _object.UpdatedDate
            };

            _context.TblClient.Add(client);
            await _context.SaveChangesAsync();

            _object.Id = client.Id;
            return _object;
        }

        public async Task<ClientDTO> Update(ClientDTO _object)
        {
            // Check if the Client name already exists
            var existingClient = await _context.TblClient
                .FirstOrDefaultAsync(t => t.Name == _object.Name);

            if (existingClient != null)
                throw new ArgumentException("A client with the same name already exists.");

            var client = await _context.TblClient.FindAsync(_object.Id);

            if (client == null)
                throw new KeyNotFoundException("Client not found");

            var salesEmployee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.SalesEmployee);

            if (salesEmployee == null)
                throw new KeyNotFoundException("SalesEmployee not found");

            client.Name = _object.Name;
            client.LineofBusiness = _object.LineofBusiness;
            client.SalesEmployee = salesEmployee.Id;
            client.Country = _object.Country;
            client.City = _object.City;
            client.State = _object.State;
            client.Address = _object.Address;
            client.IsActive = _object.IsActive;
            client.CreatedBy = _object.CreatedBy;
            client.CreatedDate = _object.CreatedDate;
            client.UpdatedBy = _object.UpdatedBy;
            client.UpdatedDate = _object.UpdatedDate;

            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _object;

        }

        public async Task<bool> Delete(string id)
        {
            // Check if the client exists
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"Client with ID {id} not found.");
            }

            // Call repository to delete the technology
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}



