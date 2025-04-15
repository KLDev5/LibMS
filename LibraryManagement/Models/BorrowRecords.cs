using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;

namespace LibraryManagement.Models
{
    public class BorrowRecord
    {   
        [Key]
        [Required]
        public long BorrowId { get; set; }
        
        
        [Required]
        [ForeignKey(nameof(Member))]
        public long MemberId { get; set; }    
        
        [Required]
        [ForeignKey(nameof(Book))]
        public long BookId { get; set; }  
        
        public DateTime?  BorrowDate { get; set; }
        
        public DateTime?  ScheduledReturnDate { get; set; } 
        
        public DateTime?  ActualReturnDate { get; set; }
        
        [Required]
        [ForeignKey(nameof(BorrowStatus))]
        public long BorrowStatusId { get; set; }
        
        public bool OverdueStatus  { get; set; }
        
        public double?  OverdueAmount { get; set; }
        
        public virtual BorrowStatus BorrowStatus { get; set; }
        public virtual Book Book { get; set; }  
        public virtual Member Member { get; set; }  
        
        public bool isDeleted { get; set; } 
        
        
    }

    public class BorrowStatus
    {
        [Key]
        [Required]
        public long BorrowStatusId { get; set; }
        
        [Required]
        public string BorrowStatusName { get; set; }    
    }

    public class BorrowRecordDetail
    {
        [Key]
        [Required]
        public long BorrowRecordDetailsId { get; set; }
        
        [Required]
        [ForeignKey(nameof(BorrowRecord))]
        public long BorrowId { get; set; }
        public virtual  BorrowRecord BorrowRecord { get; set; } 
        
        
        [ForeignKey(nameof(BorrowStatus))]
        public long? BorrowStatusId { get; set; }
        public virtual  BorrowStatus BorrowStatus { get; set; } 
    
        
        [ForeignKey(nameof(BorrowerMember))]
        [Display(Name = "Borrow Issued By")]
        public long? BorrowerMemberId { get; set; }
        public virtual Member BorrowerMember { get; set; } 
        
        [ForeignKey(nameof(ApproverMember))]
        [Display(Name = "Borrow Approved By")]
        public long? ApproverMemberId { get; set; }
        public virtual Member ApproverMember { get; set; }  
        
        
        public DateTime?  BorrowRecordDetailsDate { get; set; } 
        
        public bool isDeleted { get; set; } 

        
        
        
        
    }
    
    public class PendingRequestViewModel
    {
        public long BorrowId { get; set; }
        public string MembershipCode { get; set; }
        public string MemberName { get; set; }
        public string BookTitle { get; set; }
        public DateTime? RequestDate { get; set; }
    }
    
    public class ApprovedBorrowedRecordsViewModel
    {
        public long BorrowId { get; set; }
        public string BorrowerMembershipCode { get; set; }
        public string BorrowerName { get; set; }
        public string ApproverMembershipCode { get; set; }
        public string ApproverName { get; set; }
        public string Book { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? ScheduledReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public bool OverdueStatus { get; set; }
        public double? OverdueAmount { get; set; }
        public string BookStatus { get; set; }
    }

    
    public class BorrowRecordHistoryViewModel
    {
        public long? BorrowRecordHistoryId { get; set; }
        public string BorrowEvent { get; set; }
        public string Book { get; set; }
        public string Approver { get; set; }
        public string Borrower { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? ScheduledReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public double? OverdueAmount { get; set; }
        public bool OverdueStatus { get; set; }
    }

    
    
}