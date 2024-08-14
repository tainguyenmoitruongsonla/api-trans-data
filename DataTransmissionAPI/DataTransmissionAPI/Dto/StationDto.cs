namespace DataTransmissionAPI.Dto
{
    public class StationDto
    {
        public int? id { get; set; }
        public string name { get; set; }
        public double? x { get; set; }
        public double? y { get; set; }
        public double alarm_level1 { get; set; }
        public double alarm_level2 { get; set; }
        public double alarm_level3 { get; set; }
        public List<WaterLevelDataDto> water_level_data { get; set; }
    }

    public class FormFilterStation
    {
        public string station_name { get; set; }
    }
}
