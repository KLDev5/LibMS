using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LibraryManagement.Models
{
    public class Book
    {
        [Key]
        public long BookId { get; set; }
        [Required]
        [StringLength(128)]
        public string Title { get; set; }  
        [Required]
        [StringLength(128)]
        public string Author { get; set; }  
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }
        [Required]
        public bool isDeleted { get; set; }
        [Required]
        
        [Display(Name = "Status")]
        [ForeignKey(nameof(BookStatus))]
        public long BookStatusID { get; set; }
        
        public virtual BookStatus BookStatus { get; set; }
    }
    
    
    public class BookCatalogueView //to show the borrower which books to choose
    {
        
        public long BookCatalogueId { get; set; }   
        
        public long BookId { get; set; }    
        
        public string Title { get; set; }   
        
    
        public string Author { get; set; }  
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        
        public long BookStatusID
        { get; set; }
        
    
    }
}