using DataTransmissionAPI.Data;
using Microsoft.ML.Data;

namespace DataTransmissionAPI.Dto
{
    public class FloodDataDto
    {
        [LoadColumn(0)]
        public float station_id { get; set; }

        [LoadColumn(1)]
        public DateTime date { get; set; }
        public float DayOfYear => date.DayOfYear;
        public float Month => date.Month;
        public float Year => date.Year;

        [LoadColumn(2)]
        public float water_level { get; set; }

        [LoadColumn(3)]
        public float amount_rain { get; set; }

        [ColumnName("Label")]
        public float NextDayWaterLevel { get; set; }
    }

    public class WaterLevelPrediction
    {
        [ColumnName("Score")]
        public float PredictedWaterLevel { get; set; }
    }

    public class WaterLevelPredictionOut
    {
        public List<DateTime> dates { get; set; }
        public List<float?> water_level { get; set; }
        public List<float?> water_level_predict { get; set; }
        public List<WaterLevelData> data { get; set; }
    }
}
