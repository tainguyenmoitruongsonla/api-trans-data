namespace DataTransmissionAPI.Dto
{
    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public List<DashboardDto> Dashboards { get; set; }
    }
}
