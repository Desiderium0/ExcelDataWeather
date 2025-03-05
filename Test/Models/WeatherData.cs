using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class WeatherData
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? Time { get; set; }

        [Column("T")]
        public double? Temperature { get; set; }

        [Column("RelativeHumidity")]
        public int? RelativeHumidity { get; set; }

        [Column("Td")]
        public double? DewPoint { get; set; }

        [Column("AtmosphericPressure")]
        public double? AtmosphericPressure { get; set; }

        [Column("WindDirection")]
        [StringLength(50)]
        public string? WindDirection { get; set; }

        [Column("WindSpeed")]
        public double? WindSpeed { get; set; }

        [Column("Cloudiness")]
        public int? Cloudiness { get; set; }

        [Column("h")]
        public int? CloudHeight { get; set; }

        [Column("VV")]
        public double? Visibility { get; set; }

        [Column("WeatherPhenomena")]
        [StringLength(255)]
        public string? WeatherPhenomena { get; set; }
    }
}
