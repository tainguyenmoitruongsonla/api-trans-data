using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataTransmissionAPI.Data
{
    public class Station
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double alarm_level1 { get; set; }
        public double alarm_level2 { get; set; }
        public double alarm_level3 { get; set; }

        public virtual ICollection<WaterLevelData> water_level_data { get; set; }
    }
}
