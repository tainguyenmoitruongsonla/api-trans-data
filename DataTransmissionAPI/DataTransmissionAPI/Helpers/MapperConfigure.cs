using AutoMapper;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;

namespace DataTransmissionAPI.Helpers
{
    public class MapperConfigure : Profile
    {
        public MapperConfigure()
        {

            //-------------Authenticatiion--------------------

            //Users
            CreateMap<AspNetUsers, UserDto>().ReverseMap();

            //Users Info
            CreateMap<AspNetUsers, UserInfoDto>()
                .ForMember(dest => dest.Dashboards, opt =>
                {
                    opt.MapFrom((src, dest) => dest.Dashboards);
                }).ReverseMap();

            //Roles
            CreateMap<AspNetRoles, RoleDto>()
                .ForMember(dest => dest.Dashboards, opt =>
                {
                    opt.MapFrom((src, dest) => dest.Dashboards);
                }).ReverseMap();

            //Dashboards
            CreateMap<Dashboards, DashboardDto>()
                .ForMember(dest => dest.Functions, opt =>
                {
                    opt.MapFrom((src, dest) => dest.Functions);
                }).ReverseMap();

            //Permissions
            CreateMap<Permissions, PermissionDto>().ReverseMap();

            //Dashboard for Roles and Users
            CreateMap<UserDashboards, UserDashboardDto>().ReverseMap();
            CreateMap<RoleDashboards, RoleDashboardDto>().ReverseMap();

            //functions
            CreateMap<Functions, FunctionDto>().ReverseMap();

            //Construction
            CreateMap<Construction, ConstructionDto>().ReverseMap();

            //StoragePreData
            CreateMap<StoragePreData, StoragePreDataDto>()
                .ForMember(dest => dest.construction, opt =>
                {
                    opt.MapFrom((src, dest) => dest.construction);
                }).ReverseMap();

            //-------------Other mapper--------------------
            CreateMap<Station, StationDto>()
                .ForMember(dest => dest.water_level_data, opt =>
                {
                    opt.MapFrom((src, dest) => dest.water_level_data);
                }).ReverseMap();

            CreateMap<WaterLevelData, WaterLevelDataDto>()
                .ForMember(dest => dest.station_name, opt => opt.MapFrom((src, dest) => src.station!.id == dest.station_id ? src.station.name : null))
                .ReverseMap();

        }
    }
}
