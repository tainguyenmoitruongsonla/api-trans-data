using DataTransmissionAPI.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Dto;

namespace DataTransmissionAPI.Service
{
    public class WaterLevelDataService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<AspNetUsers> _userManager;

        // Constructor to initialize the service with required dependencies
        public WaterLevelDataService(DatabaseContext context, IMapper mapper, IHttpContextAccessor httpContext, UserManager<AspNetUsers> userManager)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        // Method to retrieve a single CT_ThongTin entity by Id
        public async Task<List<WaterLevelDataDto>> GetByStationAsync(int station_id, DateTime s_date, DateTime e_date)
        {
            var query = _context.WaterLevelData!
                .Include(x => x.station)
                .Where(x => x.station_id == station_id && x.date >= s_date && x.date <= e_date.AddDays(1))
                .OrderByDescending(x => x.date)
                .AsQueryable();

            var wlData = await query.ToListAsync();

            if (wlData == null)
            {
                // Handle the case where the record is not found
                return null;
            }

            var wlDataDto = _mapper.Map<List<WaterLevelDataDto>>(wlData);

            return wlDataDto;
        }

        // Method to save or update a CT_ThongTin entity
        public async Task<bool> SaveAsync(WaterLevelDataDto dto)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContext.HttpContext!.User);
            WaterLevelData item = null; // Declare item variable

            // Retrieve an existing item based on Id or if dto.Id is 0
            var existingItem = await _context.WaterLevelData!.FirstOrDefaultAsync(d => d.id == dto.id);

            if (existingItem == null || dto.id == 0)
            {
                // If the item doesn't exist or dto.Id is 0, create a new item
                item = _mapper.Map<WaterLevelData>(dto);
                _context.WaterLevelData!.Add(item);
            }
            else
            {
                // If the item exists, update it with values from the dto
                item = existingItem;
                _mapper.Map(dto, item);
                _context.WaterLevelData!.Update(item);
            }

            // Save changes to the database
            var res = await _context.SaveChangesAsync();

            return res > 0 ? true : false;
        }

        // Method to delete a CT_ThongTin entity
        public async Task<bool> DeleteAsync(int Id)
        {
            // Retrieve an existing item based on Id
            var existingItem = await _context.WaterLevelData!.FirstOrDefaultAsync(d => d.id == Id);

            if (existingItem == null) { return false; } // If the item doesn't exist, return false

            _context.WaterLevelData!.Remove(existingItem);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return true to indicate successful deletion
            return true;
        }
    }
}
