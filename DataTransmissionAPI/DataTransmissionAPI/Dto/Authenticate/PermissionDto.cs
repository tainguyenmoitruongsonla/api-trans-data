namespace DataTransmissionAPI.Dto
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int DashboardId { get; set; }
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionCode { get; set; }
    }
}
