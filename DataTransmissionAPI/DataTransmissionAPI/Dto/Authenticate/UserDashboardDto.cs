namespace DataTransmissionAPI.Dto
{
    public class UserDashboardDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int DashboardId { get; set; }
        public string DashboardName { get; set; }
        public string FileControl { get; set; }
        public string Description { get; set; }
        public bool PermitAccess { get; set; }
    }
}
