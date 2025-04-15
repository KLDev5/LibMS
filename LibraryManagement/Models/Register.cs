using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Register
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName{ get; set; }
        [Required]
        public string Email { get; set; }    
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string VerifyPassword { get; set; }
        
        
        

    }
}