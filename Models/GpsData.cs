using System.ComponentModel.DataAnnotations.Schema;

namespace TowSat_Backend.Models {

    [Table("gpsData")]
    public class GpsData {
        public int Id { get; set; }
        public string Name { get; set; }
        public string JsonData { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
    }
}
