using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace LibraryManagement.Models
{
    public class Login
    {
        [Required]
        public string Email { get; set; }    
        
        [Required]
        public string Password { get; set; }
    }
}