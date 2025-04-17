using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    public class Dashboard //to show the borrower which books to choose
    {

        public long AvailableBooksCount { get; set; }

        public long BorrowedBooksCount { get; set; }
        
        public long TotalBooksCount { get; set; }
        
         public long TotalMembersCount { get; set; }
         
         public long BooksOverdueCount { get; set; }
    }
}