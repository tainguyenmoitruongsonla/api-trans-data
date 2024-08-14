using DataTransmissionAPI.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Dto;

namespace DataTransmissionAPI.Service
{
    public class StationService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<AspNetUsers> _userManager;

        // Constructor to initialize the service with required dependencies
        public StationService(DatabaseContext context, IMapper mapper, IHttpContextAccessor httpContext, UserManager<AspNetUsers> userManager)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        // Method to retrieve a list of CT_ThongTin entities based on specified filters
        public async Task<List<StationDto>> GetAllAsync(FormFilterStation filter)
        {
            _context.Database.SetCommandTimeout(120);

            var query = _context.Station!
                .Where(x => x.id > 0)
                .Include(x => x.water_level_data)
                .OrderBy(x => x.id)
                .AsQueryable();

            // Apply filters based on input parameters
            if (!string.IsNullOrEmpty(filter.station_name))
            {
                query = query.Where(x => x.name!.Contains(filter.station_name));
            }

            var stations = await query.ToListAsync();

            // Map the result to DTOs
            var stationDtos = _mapper.Map<List<StationDto>>(stations);

            // Further processing on DTOs
            //foreach (var dto in stationDtos)
            //{
            //    dto.water_level_data = _mapper.Map<List<WaterLevelDataDto>>(await _context.WaterLevelData.Where(x => x.station_id == dto.id).OrderByDescending(e => e.date).Take(10).ToListAsync());
            //}

            // Return the list of DTOs
            return stationDtos;
        }

        // Method to retrieve a single CT_ThongTin entity by Id
        public async Task<StationDto> GetByIdAsync(int Id)
        {
            var query = _context.Station!
                .Where(x => x.id > 0)
                .Include(x => x.water_level_data)
                .OrderBy(x => x.id)
                .AsQueryable();

            var station = await query.SingleOrDefaultAsync(ct => ct.id == Id);

            if (station == null)
            {
                // Handle the case where the record is not found
                return null;
            }

            var stationDto = _mapper.Map<StationDto>(station);

            await PopulateDataAsync(stationDto);

            return stationDto;
        }

        private async Task PopulateDataAsync(StationDto dto)
        {
            dto.water_level_data = _mapper.Map<List<WaterLevelDataDto>>(await _context.WaterLevelData.Where(x => x.station_id == dto.id).OrderByDescending(e => e.date).ToListAsync());
        }

        // Method to save or update a CT_ThongTin entity
        public async Task<int> SaveAsync(StationDto dto)
        {
            int id = 0;
            var currentUser = await _userManager.GetUserAsync(_httpContext.HttpContext!.User);
            Station item = null; // Declare item variable

            // Retrieve an existing item based on Id or if dto.Id is 0
            var existingItem = await _context.Station!.FirstOrDefaultAsync(d => d.id == dto.id);

            if (existingItem == null || dto.id == 0)
            {
                // If the item doesn't exist or dto.Id is 0, create a new item
                item = _mapper.Map<Station>(dto);
                _context.Station!.Add(item);
            }
            else
            {
                // If the item exists, update it with values from the dto
                item = existingItem;
                _mapper.Map(dto, item);
                _context.Station!.Update(item);
            }

            // Save changes to the database
            var res = await _context.SaveChangesAsync();

            // Simplified assignment of id based on the result of SaveChanges
            id = (int)(res > 0 ? item.id : 0);

            // Return the id
            return id;
        }

        // Method to delete a CT_ThongTin entity
        public async Task<bool> DeleteAsync(int Id)
        {
            // Retrieve an existing item based on Id
            var existingItem = await _context.Station!.FirstOrDefaultAsync(d => d.id == Id);

            if (existingItem == null) { return false; } // If the item doesn't exist, return false

            _context.Station!.Remove(existingItem);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return true to indicate successful deletion
            return true;
        }
    }
}
