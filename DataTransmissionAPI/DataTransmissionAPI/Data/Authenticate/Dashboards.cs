using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransmissionAPI.Data
{
    public class Dashboards
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Nullable<bool> PermitAccess { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime? ModifiedTime { get; set; }
        public string ModifiedUser { get; set; } = string.Empty;
        public Nullable<bool> IsDeleted { get; set; }

        public virtual Permissions Permissions { get; set; }
    }
}
