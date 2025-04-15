using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Role
    {
        [Key]
        public long RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}