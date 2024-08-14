namespace DataTransmissionAPI.Dto
{
    public class DashboardDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public bool PermitAccess { get; set; } = false;
        public virtual List<FunctionDto> Functions { get; set; }
    }
}
