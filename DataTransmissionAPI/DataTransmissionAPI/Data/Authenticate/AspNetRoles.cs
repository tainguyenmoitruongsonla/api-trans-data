using Microsoft.AspNetCore.Identity;

namespace DataTransmissionAPI.Data
{
    public class AspNetRoles : IdentityRole
    {
        public Nullable<bool> IsDefault { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string ModifiedUser { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public virtual Permissions Permissions { get; set; }

    }
}
