using System.ComponentModel.DataAnnotations;

namespace LibrarySystemAPI.Models
{
    public class User_Data
    {
        [Key]
        [Required]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }
        public string UserAuth { get; set; }
        public string UserEmail { get; set; }
        public string? UserLoc { get; set; }
       
    }
}
