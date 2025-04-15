using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class BookStatus
    {
        [Key]
        public long BookStatusID { get; set; }
        
        public string BookStatusName { get; set; }
        
    }
}