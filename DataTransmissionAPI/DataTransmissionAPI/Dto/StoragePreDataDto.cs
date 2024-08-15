namespace DataTransmissionAPI.Dto
{
    public class StoragePreDataDto
    {
        public int Id { get; set; }
        public string ConstructionCode { get; set; }
        public string StationCode { get; set; }
        public string ParameterName { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime Time { get; set; }
        public bool DeviceStatus { get; set; }

        public ConstructionDto? construction { get; set; }
    }
}
