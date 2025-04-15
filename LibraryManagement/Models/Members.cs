using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Member
    {
        [Key]
        [Required]
        public long MemberId { get; set; }
        
        
        [ForeignKey(nameof(User))]
        [Required]
        public long UserId { get; set; }
        
        [Required]
        public string MembershipCode { get; set; }  
        
        
        public string PhoneNumber { get; set; }
        
        
        public string Address { get; set; }
        
        
        public DateTime? DateOfBirth { get; set; }
        
        
        public DateTime? JoinDate { get; set; }
        
        
        public DateTime? ExpiryDate { get; set; }
        
        
        public int? TotalBooksBorrowed { get; set; }
        
        public double? OutstandingFines { get; set; }
        
        public int? OverdueLimit { get; set; }
        
        public int? OverdueCount { get; set; }
        
        
            
        [Required]
        public bool IsDeleted { get; set; } 
        public virtual User User { get; set; }  
        
        // [Required]
        // [StringLength(128)]
        // [Display(Name = "Status")]
        // [ForeignKey(nameof(BookStatus))]
        
    }
    
    public class MemberView
    {
        public long MemberId { get; set; }
        public string MembershipCode { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Email { get; set; }
        public long RoleId { get; set; }
        
        public int? OverdueCount { get; set; }
        public int? OverdueLimit { get; set; }
        
        public int? TotalBooksBorrowed { get; set; }



        
    }
}