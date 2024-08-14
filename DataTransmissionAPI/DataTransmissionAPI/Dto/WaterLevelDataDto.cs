namespace DataTransmissionAPI.Dto
{
    public class WaterLevelDataDto
    {
        public int id { get; set; }
        public int station_id { get; set; }
        public string station_name { get; set; }
        public DateTime date { get; set; }
        public double water_level { get; set; }
        public double? amount_rain { get; set; }
    }
}
