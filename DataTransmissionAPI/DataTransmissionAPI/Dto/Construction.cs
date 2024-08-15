namespace DataTransmissionAPI.Dto
{
    public class ConstructionDto
    {
        public int Id { get; set; }
        public int TypeOfConstructionId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int CommuneId { get; set; }
        public int RiverId { get; set; }
        public int BasinId { get; set; }
        public string ConstructionName { get; set; }
        public string Name { get; set; }
        public string ConstructionCode { get; set; }
        public string ConstructionLocation { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public float MaximumDischargeFlowPre { get; set; }
        public float CapacityPre { get; set; }
        public float DownstreamWLPre { get; set; }
        public float UpstreamWLPre { get; set; }
        public float MinimumFlowPre { get; set; }
        public float MaximumFlowPre { get; set; }
        public float DukienluuluonghaduPre { get; set; }
        public float DukienmucnuochoPre { get; set; }
        public float LuuluonghaduPre { get; set; }
        public float LuuluongdenPre { get; set; }
        public float MuathuongluuPre { get; set; }
        public DateTime TimePre { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedUser { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string ModifiedUser { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDisconnect { get; set; }
        public bool IsError { get; set; }
        public bool Change { get; set; }
    }

}
