using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryManagement.Models
{
    public class User
    {
        
            [Key]
            public long UserId { get; set; }

            [Required]
            [StringLength(256)]
            public string FullName { get; set; }

            [Required]
            [StringLength(512)]
            public string Email { get; set; }

            [Required]
            [StringLength(256)]
            [Display(Name = "Password")]
            public string PasswordHash { get; set; } // Store hashed password

            [Required]
            [ForeignKey(nameof(Role))]
            public long RoleId { get; set; }
            
            [Required]
            public bool IsDeleted { get; set; } 
            
            public virtual Role Role { get; set; }
        
    }
}