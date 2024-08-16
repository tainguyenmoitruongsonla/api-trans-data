using DataTransmissionAPI.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Dto;

namespace DataTransmissionAPI.Service
{
    public class StoragePreDataService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<AspNetUsers> _userManager;

        // Constructor to initialize the service with required dependencies
        public StoragePreDataService(DatabaseContext context, IMapper mapper, IHttpContextAccessor httpContext, UserManager<AspNetUsers> userManager)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        // Method to retrieve a list of CT_ThongTin entities based on specified filters
        public async Task<List<StoragePreDataDto>> GetAllAsync()
        {

            var query = _context.StoragePreData!
                .OrderBy(x => x.Id)
                .AsQueryable();

            var stations = await query.ToListAsync();

            // Map the result to DTOs
            var stationDtos = _mapper.Map<List<StoragePreDataDto>>(stations);

            // Return the list of DTOs
            return stationDtos;
        }

        // Method to retrieve a single CT_ThongTin entity by Id
        public async Task<StoragePreDataDto> GetByIdAsync(int Id)
        {
            var query = _context.StoragePreData!
                .Where(x => x.Id > 0)
                .OrderBy(x => x.Id)
                .AsQueryable();

            var PreData = await query.SingleOrDefaultAsync(ct => ct.Id == Id);

            if (PreData == null)
            {
                // Handle the case where the record is not found
                return null;
            }

            var PreDataDto = _mapper.Map<StoragePreDataDto>(PreData);

            return PreDataDto;
        }

        // Method to save or update a CT_ThongTin entity
        public async Task<int> SaveAsync(StoragePreDataDto dto)
        {
            int id = 0;
            var currentUser = await _userManager.GetUserAsync(_httpContext.HttpContext!.User);
            StoragePreData item = null; // Declare item variable

            // Retrieve an existing item based on Id or if dto.Id is 0
            var existingItem = await _context.StoragePreData!.FirstOrDefaultAsync(d => d.Id == dto.Id);

            if (existingItem == null || dto.Id == 0)
            {
                // If the item doesn't exist or dto.Id is 0, create a new item
                item = _mapper.Map<StoragePreData>(dto);
                _context.StoragePreData!.Add(item);
            }
            else
            {
                // If the item exists, update it with values from the dto
                item = existingItem;
                _mapper.Map(dto, item);
                _context.StoragePreData!.Update(item);
            }

            // Save changes to the database
            var res = await _context.SaveChangesAsync();

            // Simplified assignment of id based on the result of SaveChanges
            id = (int)(res > 0 ? item.Id : 0);

            // Return the id
            return id;
        }

        // Method to delete a CT_ThongTin entity
        public async Task<bool> DeleteAsync(int Id)
        {
            // Retrieve an existing item based on Id
            var existingItem = await _context.StoragePreData!.FirstOrDefaultAsync(d => d.Id == Id);

            if (existingItem == null) { return false; } // If the item doesn't exist, return false

            _context.StoragePreData!.Remove(existingItem);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return true to indicate successful deletion
            return true;
        }
    }
}
