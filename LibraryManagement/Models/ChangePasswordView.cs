using System.Data.Entity;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace LibraryManagement.Models
{
    public class ChangePasswordView
    {
        [Key]
        public long UserId { get; set; }
        
        [Required]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        
        [Required]
        public string NewPassword { get; set; }
    }
}