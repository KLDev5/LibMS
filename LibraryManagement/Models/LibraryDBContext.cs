using System.Data.Entity;
using System.Configuration;
namespace LibraryManagement.Models
{
    public class LibraryDBContext : DbContext
    {
        
        
        public LibraryDBContext() : base("name=DefaultConnection") { }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookStatus> BookStatuses { get; set; } 
        
        public DbSet<User> Users { get; set; }
        
        public DbSet<Role> Roles { get; set; }  
        
        public DbSet<Member> Members { get; set; }  
        
        public DbSet<BorrowStatus> BorrowStatuses { get; set; }  
        
        public DbSet<BorrowRecord> BorrowRecords { get; set; }  
        
         public DbSet<BorrowRecordDetail> BorrowRecordDetails { get; set; }  
         public DbSet<LibraryConfiguration> LibraryConfigurations { get; set; }  

        


        
    }
}

