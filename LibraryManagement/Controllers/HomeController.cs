using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryManagement.Models;

namespace LibraryManagement.Controllers
{
    public class HomeController : Controller
    {
        private LibraryDBContext db = new LibraryDBContext();
        private Dashboard dsb = new Dashboard();
        
        
        public ActionResult Index()
        {
            try
            {
                long roleid = Convert.ToInt64(Session["UserRole"].ToString());
                long userid = Convert.ToInt64(Session["UserId"].ToString());
                var  availableBooks = db.Books.Count(b => b.BookStatusID == 1 && b.isDeleted == false);
                long TotalBooksCount=db.Books.Count(b => b.isDeleted == false);
                long TotalMembersCount=db.Members.Count(m=> m.IsDeleted==false);
                long BooksOverdueCount;
                long TotalBooksBorrowed;
                
                
                if (roleid == 3)
                {
                    BooksOverdueCount= db.BorrowRecords
                        .Where(br => br.BorrowStatusId == 2 
                                     && br.OverdueStatus == true
                                     && br.Member.UserId == userid)
                        .Count();
                    
                    TotalBooksBorrowed=db.BorrowRecords
                        .Where(br => br.BorrowStatusId == 2 && br.Member.UserId == userid)
                        .Select(br => br.BookId)
                        .Distinct()
                        .Count();
                }
                else
                {
                    BooksOverdueCount=db.BorrowRecords.Count(b => b.isDeleted==false && b.BorrowStatusId==2 && b.OverdueStatus== true  );

                    TotalBooksBorrowed=db.BorrowRecords
                        .Where(b => b.BorrowStatusId == 2)
                        .Select(b => b.BookId)
                        .Distinct()
                        .Count();
                } 
                
                dsb.BorrowedBooksCount = TotalBooksBorrowed;
                dsb.AvailableBooksCount = availableBooks;
                dsb.TotalBooksCount = TotalBooksCount;
                dsb.BooksOverdueCount = BooksOverdueCount;
                dsb.TotalMembersCount = TotalMembersCount;
                return View(dsb);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();

            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}