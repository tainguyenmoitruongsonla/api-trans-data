using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataTransmissionAPI.Data
{
    public class WaterLevelData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int station_id { get; set; }
        public DateTime date { get; set; }
        public double water_level { get; set; }
        public double? amount_rain { get; set; }

        [ForeignKey("station_id")]
        public virtual Station station { get; set; }
    }
}
