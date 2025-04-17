using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using LibraryManagement.Models;
using LibraryManagement.CustomExceptions;
using PagedList;
using System.IO;
using LibraryManagement.ClsLib;

namespace LibraryManagement.Controllers
{
    public class BookController: Controller
    {
        private LibraryDBContext db = new LibraryDBContext();
        
        #region index 
        public ActionResult Index(int? page,string searchTitle,string searchAuthor,string searchPublisher,string BookStatus)
        {
            try
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);



                ViewBag.statuses = db.BookStatuses.ToList(); 
                ViewBag.statusList=new SelectList(ViewBag.statuses, "BookStatusID", "BookStatusName");//viewbag for status name
                // for status names
                List<Book> PagedList;

                // var PagedList = db.Books.Where(b => b.isDeleted == false)
                //     .ToList().OrderBy(b => b.BookId).ToPagedList(pageNumber, pageSize);
                if (String.IsNullOrEmpty(searchTitle))
                {
                    searchTitle = "";
                }

                if (String.IsNullOrEmpty(searchAuthor))
                {
                    searchAuthor = "";
                }

                if (String.IsNullOrEmpty(searchPublisher))
                {
                    searchPublisher = "";
                }
                if (String.IsNullOrEmpty(BookStatus))
                {
                    BookStatus = "";
                }

                if (searchPublisher != "" || searchAuthor != "" || searchTitle != ""||BookStatus!="")
                {
                    PagedList = clsDBOperations
                        .GetFilteredBooks(searchTitle.Trim(), searchAuthor.Trim(), searchPublisher.Trim(),BookStatus.Trim(),false)
                        .ToList();
                    // if (PagedList == null) throw new BookMasterExceptions.BookNotFoundExceptions();
                    return View(PagedList);
                }



                //default
                PagedList = clsDBOperations.GetBooksDefault().ToList();
                // if (PagedList == null) throw new BookMasterExceptions.BookNotFoundExceptions();
                return View(PagedList); //only those with not deleted flag will run
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index", new {
                    page = (int?)null,
                    searchTitle = "",
                    searchAuthor = "",
                    searchPublisher = "",
                    BookStatus = ""
                }); 
            }
            catch (BookMasterExceptions.BookNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index", new {
                    page = (int?)null,
                    searchTitle = "",
                    searchAuthor = "",
                    searchPublisher = "",
                    BookStatus = ""
                });
            }
            catch (System.NullReferenceException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index", new {
                    page = (int?)null,
                    searchTitle = "",
                    searchAuthor = "",
                    searchPublisher = "",
                    BookStatus = ""
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                
                return RedirectToAction("Index", new {
                    page = (int?)null,
                    searchTitle = "",
                    searchAuthor = "",
                    searchPublisher = "",
                    BookStatus = ""
                });
            }
        }
        #endregion

        #region details
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();

                Book book = db.Books.Find(id);
                if (book == null) throw new BookMasterExceptions.BookNotFoundExceptions();

                ViewBag.statuses = db.BookStatuses.ToList();
                return View(book);
            }
            
            catch (CustomCommonExceptions.BadRequestExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();  
            }
            catch (BookMasterExceptions.BookNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();  
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;   
                return View();  
            }
        }
        #endregion
       
        #region Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookId,Title,Author,Publisher,ISBN,PublishedDate,BookStatusID")]Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    book.isDeleted = false;
                    db.Books.Add(book);
                    db.SaveChanges();
                    

                }
                TempData["SuccessMessage"] = "Book Saved Successfully";

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");
                return View(book);
            }
            
        }
        #endregion
        
        #region Edit
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();

                Book book = db.Books.Find(id);
                if (book == null) throw new BookMasterExceptions.BookNotFoundExceptions(); 

                ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return View(book);
            }
            catch (BookMasterExceptions.BookNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();  
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookId,Title,Author,Publisher,ISBN,PublishedDate,BookStatusID")] Book book,HttpPostedFileBase imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(imageFile.FileName);
                        string fileextension = Path.GetExtension(imageFile.FileName);
                        if (fileextension == ".jpg" || fileextension == ".jpeg" || fileextension == ".png")
                        {
                            string relativepath=ConfigurationManager.AppSettings["BookImageUploadPath"] + "Book_"+Guid.NewGuid() + fileextension;
                            string SavePath = Server.MapPath(relativepath);
                            imageFile.SaveAs(SavePath);
                            book.BookImage = relativepath;

                        }
                        else
                        {
                            throw new FileFormatException("Image file format is not supported.");
                        }
                        

                    }
                    else
                    {
                        throw new FileNotFoundException();
                    }
                    db.Entry(book).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }

                TempData["SuccessMessage"] = "Book Data Updated Successfully";
                return RedirectToAction("Index");
            }
            catch (FileFormatException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View(book);
            }
            catch (FileNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View(book);
            }

            
        }
        
        #endregion
        
        #region Delete
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();

                Book book = db.Books.Find(id);
                if (book == null) throw new BookMasterExceptions.BookNotFoundExceptions();
                
                return View(book);
            }
            catch (CustomCommonExceptions.BadRequestExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();  
            }
            catch (BookMasterExceptions.BookNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return RedirectToAction("Index");   
            }

            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Book book = db.Books.Find(id);
                book.isDeleted = true;

                // db.Books.Remove(book);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Book Deleted Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index");
            }
        }
        #endregion
        
        #region Delete all
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAll()
        {
            try
            {
                // Book book = db.Books.Find(id);
                // book.isDeleted = true;
                //
                // // db.Books.Remove(book);
                // db.SaveChanges();
                //
                // return RedirectToAction("Index");
                using (db)
                {
                    if (db.Database.ExecuteSqlCommand("exec sp_SoftDeleteAllBooks")<1)
                    {
                        throw new Exception("Sql Did not Work");
                    }
                }
                TempData["SuccessMessage"] = "All Books Deleted Successfully";
                return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index");
            }
        }
        #endregion
        
        #region GetAll

        public ActionResult GetAll(int? page,string searchTitle,string searchAuthor,string searchPublisher,string BookStatus,string IsDeleted)
        {
            try
            {
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                bool isDeletedflag = false;
                IPagedList<Book> PagedList;
                ViewBag.statuses = db.BookStatuses.ToList(); // for status names
                ViewBag.statusList=new SelectList(ViewBag.statuses, "BookStatusID", "BookStatusName");//viewbag for status name

                // var PagedList=db.Books.ToList().OrderBy(b => b.BookId).ToPagedList(pageNumber, pageSize);
                if (searchTitle != "" || searchAuthor != "" || searchPublisher != "" || BookStatus != "" ||
                    IsDeleted != "")
                {
                    if (!string.IsNullOrEmpty(IsDeleted)&&IsDeleted == "1")
                    {
                        isDeletedflag = true;

                    }
                    PagedList=clsDBOperations.GetFilteredBooks(searchTitle,searchAuthor,searchPublisher,BookStatus,isDeletedflag).ToPagedList(pageNumber, pageSize);
                    return View(PagedList);
                    
                }
                
                
                
                
                PagedList=clsDBOperations.GetBooksDefault(true,false).ToPagedList(pageNumber, pageSize);

                return View(PagedList); //only those with not deleted flag will run
            }
            
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();
            }
        }
        #endregion
        
        #region Undelete

        [HttpPost, ActionName("Undelete")]
        [ValidateAntiForgeryToken]
        public ActionResult Undelete(int id)
        {
            try
            {
                Book book = db.Books.Find(id);
                book.isDeleted = false;

                // db.Books.Remove(book);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return RedirectToAction("Index");
            }
        }
        #endregion
        
        
        #region bookCatalogue
        
        public ActionResult BookCatalogue(int? page,string searchTitle,string searchAuthor,string searchPublisher)
        {
            try
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                bool isDeletedflag = false;
                List<Book> PagedList;

                BookCatalogueView bc = new BookCatalogueView();
                ViewBag.statuses = db.BookStatuses.ToList(); // for status names
                // var PagedList = db.Books.Where(b => b.isDeleted == false && b.BookStatusID==1)
                //     .ToList().OrderBy(b => b.BookId).ToPagedList(pageNumber, pageSize);
                if (searchTitle != "" || searchAuthor != "" || searchPublisher != "")
                {
                   
                    PagedList=clsDBOperations.GetFilteredBooks(searchTitle,searchAuthor,searchPublisher,"1",false).ToList();
                    return View(PagedList);
                }
                 PagedList =clsDBOperations.GetBooksDefault(true,true).ToList();
                return View(PagedList); //only those with not deleted flag will run
            }
            
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();
            }
        }
        #endregion
        
        #region extract bulk book data from csv
        public ActionResult ExtractBookBulk()
        {
            try
            {
                ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExtractBookBulk(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength < 1)
                {
                    throw new Exception("Invalid File");
                }
                var books = new List<Book>();
                    using (var reader = new StreamReader(file.InputStream))
                    {
                        bool isHeader = true;
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (isHeader) // Skip the header row
                            {
                                isHeader = false;
                                continue;
                            }

                            var values = line.Split(',');

                            var book = new Book()
                            {
                                Title = values[0].Trim(),
                                Author = values[1].Trim(),
                                Publisher = values[2].Trim(),
                                ISBN = values[3].Trim(),
                                PublishedDate = DateTime.TryParseExact(
                                    values[4].Trim(), 
                                    new[] { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" }, // Accepted formats
                                    CultureInfo.InvariantCulture, 
                                    DateTimeStyles.None, 
                                    out var publishedDate
                                ) ? (DateTime)publishedDate : throw new Exception($"Invalid date format for PublishedDate: {values[4]}"),
                                isDeleted = false,
                                BookStatusID = 1
                                
                            };

                            books.Add(book);
                        }
                    }
                    db.Books.AddRange(books);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Book Data Extract Successfully";  
                    return RedirectToAction("Index");
            }
            
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return RedirectToAction("Index");
            }
        }
        #endregion
        
        
        
    }
}