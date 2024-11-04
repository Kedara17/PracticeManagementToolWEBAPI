using DataServices.Data;
using DataServices.Models;
using DataServices.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net.NetworkInformation;

namespace WebinarsApi.Services
{
    public class WebinarService : IWebinarService
    {
        private readonly IRepository<Webinars> _repository;
        private readonly DataBaseContext _context;

        public WebinarService(IRepository<Webinars> repository, DataBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<WebinarsDTO>> GetAll()
        {
            var webinars = await _context.TblWebinars
                .Include(e => e.Employee)
                .ToListAsync();

            var webinarsDto = new List<WebinarsDTO>();

            foreach (var item in webinars)
            {
                webinarsDto.Add(new WebinarsDTO
                {
                    Id = item.Id,
                    Title = item.Title,
                    Speaker = item.Employee?.Name,
                    Status = item.Status,
                    WebinarDate = item.WebinarDate,
                    NumberOfAudience = item.NumberOfAudience,
                    IsActive = item.IsActive,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate
                });
            }
            return webinarsDto;
        }

        public async Task<WebinarsDTO> Get(string id)
        {
            var webinar = await _context.TblWebinars
               .Include(p => p.Employee)
               .FirstOrDefaultAsync(t => t.Id == id);

            if (webinar == null) return null;


            return new WebinarsDTO
            {
                Id = webinar.Id,
                Title = webinar.Title,
                Speaker = webinar.Employee?.Name,
                Status = webinar.Status,
                WebinarDate = webinar.WebinarDate,
                NumberOfAudience = webinar.NumberOfAudience,
                IsActive = webinar.IsActive,
                CreatedBy = webinar.CreatedBy,
                CreatedDate = webinar.CreatedDate,
                UpdatedBy = webinar.UpdatedBy,
                UpdatedDate = webinar.UpdatedDate

            };
        }

        public async Task<WebinarsDTO> Add(WebinarsDTO _object)
        {
            var webinars = new Webinars();
            // Check if the Webinar already exists
            var existingWebinar = await _context.TblWebinars
                .FirstOrDefaultAsync(t => t.Title == _object.Title);

            if (existingWebinar != null)
                throw new ArgumentException("A Webinar with the same name already exists.");

            /* var employee = await _context.TblEmployee
                .FirstOrDefaultAsync(d => d.Name == _object.Speaker);

             if (employee == null)
                 throw new KeyNotFoundException("Speaker not found");*/

            if (!string.IsNullOrWhiteSpace(_object.Speaker))
            {
                var speakerExists = await _context.TblEmployee
                    .AnyAsync(d => d.Id == _object.Speaker);
                if (!speakerExists)
                    throw new ArgumentException("The specified speaker does not exist.");

                webinars.Speaker = _object.Speaker;
            }
            else
            {
                webinars.Speaker = null;
            }
            //_______________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.Status))
            {
                webinars.Status = _object.Status;
            }
            else
            {
                webinars.Status = null;
            }
            //________________________________________________________
            if (_object.WebinarDate.HasValue)
            {
                webinars.WebinarDate = _object.WebinarDate;
            }
            else
            {
                webinars.WebinarDate = null;
            }
            //_________________________________________________________
            if (_object.NumberOfAudience.HasValue && _object.NumberOfAudience > 0)
            {
                webinars.NumberOfAudience = _object.NumberOfAudience.Value;
            }
            else
            {
                webinars.NumberOfAudience = null;
            }

            webinars.Title = _object.Title;
               /* webinars.Speaker = _object.Speaker;*/
               /* webinars.Status = _object.Status;
                webinars.WebinarDate = _object.WebinarDate;
                webinars.NumberOfAudience = _object.NumberOfAudience;*/
                webinars.IsActive = _object.IsActive;
                webinars.CreatedBy = _object.CreatedBy;
                webinars.CreatedDate = _object.CreatedDate;
                webinars.UpdatedBy = _object.UpdatedBy;
                webinars.UpdatedDate = _object.UpdatedDate;          

            _context.TblWebinars.Add(webinars);
            await _context.SaveChangesAsync();

            _object.Id = webinars.Id;
            return _object;
        }

        public async Task<WebinarsDTO> Update(WebinarsDTO _object)
        {
            // Check if the Webinar already exists
            var existingWebinar = await _context.TblWebinars
                .FirstOrDefaultAsync(t => t.Title == _object.Title);

            if (existingWebinar != null)
                throw new ArgumentException("A Webinar with the same name already exists.");

            var webinars = await _context.TblWebinars.FindAsync(_object.Id);

            if (webinars == null)
                throw new KeyNotFoundException("Webinars not found");

            /*var speaker = await _context.TblEmployee
              .FirstOrDefaultAsync(d => d.Name == _object.Speaker);

            if (speaker == null)
                throw new KeyNotFoundException("Speaker not found");*/


            if (!string.IsNullOrWhiteSpace(_object.Speaker))
            {
                var speakerExists = await _context.TblEmployee
                    .AnyAsync(d => d.Id == _object.Speaker);
                if (!speakerExists)
                    throw new ArgumentException("The specified speaker does not exist.");

                webinars.Speaker = _object.Speaker;
            }
            else
            {
                webinars.Speaker = null;
            }
            //_______________________________________________________
            if (!string.IsNullOrWhiteSpace(_object.Status))
            {
                webinars.Status = _object.Status;
            }
            else
            {
                webinars.Status = null;
            }
            //________________________________________________________
            if (_object.WebinarDate.HasValue)
            {
                webinars.WebinarDate = _object.WebinarDate;
            }
            else
            {
                webinars.WebinarDate = null;
            }
            //_________________________________________________________
            if (_object.NumberOfAudience.HasValue && _object.NumberOfAudience > 0)
            {
                webinars.NumberOfAudience = _object.NumberOfAudience.Value;
            }
            else
            {
                webinars.NumberOfAudience = null;
            }
           
            webinars.Title = _object.Title;
            /* webinars.Speaker = _object.Speaker;*/
            /* webinars.Status = _object.Status;
             webinars.WebinarDate = _object.WebinarDate;
             webinars.NumberOfAudience = _object.NumberOfAudience;*/
            webinars.IsActive = _object.IsActive;
            webinars.CreatedBy = _object.CreatedBy;
            webinars.CreatedDate = _object.CreatedDate;
            webinars.UpdatedBy = _object.UpdatedBy;
            webinars.UpdatedDate = _object.UpdatedDate;

            _context.Entry(webinars).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            // Check if the technology exists
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"with ID {id} not found.");
            }
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }

        public async Task Activate(string id)
        {
            var webinar = await _context.TblWebinars.FindAsync(id);

            if (webinar == null)
                throw new KeyNotFoundException("Webinar not found");

            webinar.IsActive = true;
            _context.Entry(webinar).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}