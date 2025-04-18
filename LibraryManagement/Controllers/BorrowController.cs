using System.Web.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using LibraryManagement.ClsLib;
using LibraryManagement.Models;
using LibraryManagement.CustomExceptions;
using PagedList;
using BookStatus = LibraryManagement.LibEnums.BookStatus;
using BorrowStatus = LibraryManagement.LibEnums.BorrowStatus;

namespace LibraryManagement.Controllers
{
    public class BorrowController: Controller
    {
        private LibraryDBContext db = new LibraryDBContext();
        private clsLibraryConfigurations libcon=new clsLibraryConfigurations();
        #region Index
        public ActionResult Index(long? id)
        {
            try
            {
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();

                Book book = db.Books.Find(id);
                if (book == null) throw new BookMasterExceptions.BookNotFoundExceptions();
                
                //Check Existing Member
                long userid = Convert.ToInt64(Session["UserId"].ToString());
                Member member=db.Members.FirstOrDefault(m=>m.UserId==userid);
                if (member == null) throw new MemberMasterExceptions.MemberNotFoundException(); 

                BorrowRecord borrowRecord = db.BorrowRecords.FirstOrDefault(b => b.BookId == book.BookId &&  b.MemberId == member.MemberId && b.BorrowStatusId != (int)BorrowStatus.ReturnedOrAvailable);
                if(borrowRecord!=null){ //if there is already a borrow record by the same user then request button will not be shown or will be shown readonly
                    
                    ViewBag.AlreadyRequested = "Requested";
                    
                }
                else
                {
                    ViewBag.AlreadyRequested = "";//not sure whether viewbags will be retained in the client while session is going thats why emptied the viewbag forcefully 
                    
                }

                ViewBag.statuses = db.BookStatuses.ToList();
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();  
            }
        }
        #endregion
        #region search filters
        
        
        #endregion
        
        #region Request
        
        
        [HttpPost, ActionName("Request")]
        [ValidateAntiForgeryToken]
        public ActionResult Request(long id)
        {
            try
            {
                if (id < 1) throw new CustomCommonExceptions.BadRequestExceptions();
                long userid = Convert.ToInt64(Session["UserId"].ToString());
                Member member = db.Members.FirstOrDefault(m => m.UserId == userid);
                if (member == null) throw new MemberMasterExceptions.MemberNotFoundException();
        
                #region Populate BorrowRecord
        
                BorrowRecord borrowRecord = new BorrowRecord();
                borrowRecord.BookId = id;
                borrowRecord.MemberId = member.MemberId;
                borrowRecord.BorrowStatusId = (int)BorrowStatus.Requested;
                db.BorrowRecords.Add(borrowRecord);
                db.SaveChanges();
        
        
        
                #endregion
        
                #region BorrowRecordDetails
        
                BorrowRecordDetail borrowRecordDetail = new BorrowRecordDetail();
                borrowRecordDetail.BorrowId = borrowRecord.BorrowId;
                borrowRecordDetail.BorrowStatusId = borrowRecord.BorrowStatusId;
                borrowRecordDetail.BorrowerMemberId = borrowRecord.MemberId;
                borrowRecordDetail.BorrowRecordDetailsDate = DateTime.Now;
                db.BorrowRecordDetails.Add(borrowRecordDetail);
                db.SaveChanges();
        
                #endregion
        
        
        
                // db.Books.Remove(book);
                db.SaveChanges();
        
        
        
                TempData["SuccessMessage"] = "Request has been sent successfully";
                ViewBag.statuses = db.BookStatuses.ToList();
                return RedirectToAction("Index", new { id = id });
            }
            catch (CustomCommonExceptions.BadRequestExceptions ex)
            {
                ViewBag.statuses = db.BookStatuses.ToList();
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index", new { id = id });
                
            }
            catch (MemberMasterExceptions.MemberNotFoundException ex)
            {
                ViewBag.statuses = db.BookStatuses.ToList();
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { id = id });
                
            }
            catch (Exception ex)
            {
                ViewBag.statuses = db.BookStatuses.ToList();
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { id = id });
                
            }
        }
        #endregion
        
        #region Pending Request
        
        public ActionResult PendingRequests(int? page,string searchFullName,string searchMembershipCode,string searchBookName, string SearchRequestDate)
        {
            try
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                List<PendingRequestViewModel> PagedList;
                ViewBag.statuses = db.BookStatuses.ToList(); // for status names
                //show only those records which have borrowstatus=1 or requested
                IQueryable<PendingRequestViewModel> query;
                long userid = Convert.ToInt64(Session["UserId"].ToString());
                long roleid = Convert.ToInt64(Session["UserRole"].ToString());

                if (!string.IsNullOrEmpty(searchFullName)  || !string.IsNullOrEmpty(searchMembershipCode) || !string.IsNullOrEmpty(searchBookName) ||
                   !string.IsNullOrEmpty(SearchRequestDate))
                {
                    PagedList=clsDBOperations.GetPendingBorrowRequestsFiltered(roleid,userid,searchFullName,searchMembershipCode,searchBookName,SearchRequestDate).ToList();
                    return View(PagedList); 
                }
                
                
                 PagedList = clsDBOperations.GetPendingBorrowRequestsDefault(roleid,userid).ToList();
                return View(PagedList); //only those with not deleted flag will run
            }
            catch (BorrowRecordsExceptions.BorrowRecordNotFoundExceptions ex)
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
        
        #region ApproveRequest

        [HttpPost, ActionName("ApproveRequest")]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveRequest(long? id)
        {
            try
            {
                long userid = Convert.ToInt64(Session["UserId"].ToString());
                Member member = db.Members.FirstOrDefault(m => m.UserId == userid);

                if (id == null) throw new BorrowRecordsExceptions.BorrowRecordNotFoundExceptions();
                
                // LibraryConfiguration lconfig = db.LibraryConfigurations.Find(3);
                // if (lconfig == null) throw new BorrowRecordsExceptions.BorrowConfigurationsNotFoundExceptions();
                LibraryConfiguration lconfig = libcon.getBorrowDurationConfiguration();
                if(!lconfig.ConfigValue.Trim().IsInt()) throw new BorrowRecordsExceptions.BorrowConfigurationsValueNotSetProperly();
                    
                int DefaultBookDuration = string.IsNullOrEmpty(lconfig.ConfigValue.Trim())? Convert.ToInt32(lconfig.ConfigValue.Trim()): 14;
                
                
                #region borrowrecord update
                BorrowRecord borrowRecord = db.BorrowRecords.Find(id);
                if (borrowRecord == null) throw new BorrowRecordsExceptions.BorrowRecordNotFoundExceptions();
                
                
                borrowRecord.BorrowStatusId = (int)BorrowStatus.ApprovedOrBorrowed;// approved 
                borrowRecord.BorrowDate=DateTime.Now;
                borrowRecord.ScheduledReturnDate = (borrowRecord.BorrowDate ?? DateTime.Now).AddDays(DefaultBookDuration);
                
                #endregion
                
                #region borrowrecord details
                
                if (member == null) throw new MemberMasterExceptions.MemberNotFoundException();
                
                BorrowRecordDetail borrowRecordDetail = new BorrowRecordDetail();
                borrowRecordDetail.BorrowId = borrowRecord.BorrowId;
                borrowRecordDetail.BorrowStatusId = borrowRecord.BorrowStatusId;
                borrowRecordDetail.BorrowerMemberId = borrowRecord.MemberId;
                borrowRecordDetail.BorrowRecordDetailsDate = DateTime.Now;
                borrowRecordDetail.ApproverMemberId = member.MemberId;
                db.BorrowRecordDetails.Add(borrowRecordDetail);
                #endregion
                
                #region change book status
                Book book = db.Books.Find(borrowRecord.BookId);
                if (book == null) throw new BookMasterExceptions.BookNotFoundExceptions();
                book.BookStatusID = (int)BookStatus.Borrowed; // book status changed to borrowed
                #endregion
                
                #region Member book borrow count update

                Member BorrowerMember = db.Members.Find(borrowRecord.MemberId);
                BorrowerMember.TotalBooksBorrowed= BorrowerMember.TotalBooksBorrowed != null && BorrowerMember.TotalBooksBorrowed >= 1 ? BorrowerMember.TotalBooksBorrowed + 1 : 1;
                
                
                #endregion 
                
                db.SaveChanges();
                // db.Books.Remove(book);
               
                TempData["SuccessMessage"] = "Request has been sent successfully";
                return RedirectToAction("PendingRequests");
            }
            catch (BorrowRecordsExceptions.BorrowRecordNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();
                
            }
            catch (MemberMasterExceptions.MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();
                
            }
            
            catch (BookMasterExceptions.BookNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();
                
            }
            catch (BorrowRecordsExceptions.BorrowConfigurationsNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();
                
            }
            catch (BorrowRecordsExceptions.BorrowConfigurationsValueNotSetProperly ex)
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
        
        
        
        #region Approved Request or Borrowed
        
        public ActionResult ApprovedBorrowedRecords(
            int? page, 
            string borrowerMembershipCode,
            string borrowId,
            string borrowerFullName,
            string approverMembershipCode,
            string approverFullName,
            string bookTitle,
            string borrowDate,
            string scheduledReturnDate,
            string overdueStatus)
        {
            try
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);

                ViewBag.statuses = db.BookStatuses.ToList(); // for status names
                //show only those records which have borrowstatus=1 or requested
                IQueryable<ApprovedBorrowedRecordsViewModel> query;
                long userid = Convert.ToInt64(Session["UserId"].ToString());
                long roleid = Convert.ToInt64(Session["UserRole"].ToString());

                List<ApprovedBorrowedRecordsViewModel> PagedList;

                if (!string.IsNullOrWhiteSpace(borrowerMembershipCode) ||
                    !string.IsNullOrWhiteSpace(borrowId) ||
                    !string.IsNullOrWhiteSpace(borrowerFullName) ||
                    !string.IsNullOrWhiteSpace(approverMembershipCode) ||
                    !string.IsNullOrWhiteSpace(approverFullName) ||
                    !string.IsNullOrWhiteSpace(bookTitle) ||
                    !string.IsNullOrWhiteSpace(borrowDate) ||
                    !string.IsNullOrWhiteSpace(scheduledReturnDate) ||
                    !string.IsNullOrWhiteSpace(overdueStatus))
                {
                    query = clsDBOperations.GetApprovedRequestsFiltered(
                        roleid,
                        userid,
                        borrowerMembershipCode,
                        borrowId,
                        borrowerFullName,
                        approverMembershipCode,
                        approverFullName,
                        bookTitle,
                        borrowDate,
                        scheduledReturnDate,
                        overdueStatus);
                    PagedList = query.ToList();
                    return View(PagedList); 
                    
                }
                
                query=clsDBOperations.GetApprovedRequestsDefault(roleid, userid);

                 PagedList = query.ToList();
                 return View(PagedList); 
            }
            catch (BorrowRecordsExceptions.BorrowRecordNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                
                return RedirectToAction("ApprovedBorrowedRecords", new {
                    page = (int?)null,
                    borrowerMembershipCode = "",
                    borrowId = "",
                    borrowerFullName = "",
                    approverMembershipCode = "",
                    approverFullName = "",
                    bookTitle = "",
                    borrowDate = "",
                    scheduledReturnDate = "",
                    overdueStatus = ""
               
                });
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("ApprovedBorrowedRecords", new {
                    page = (int?)null,
                    borrowerMembershipCode = "",
                    borrowId = "",
                    borrowerFullName = "",
                    approverMembershipCode = "",
                    approverFullName = "",
                    bookTitle = "",
                    borrowDate = "",
                    scheduledReturnDate = "",
                    overdueStatus = ""
               
                });
            }
        }
        
        
        
        #endregion
        
        
        #region Return
        public ActionResult Return(long? id)
        {
            try
            {
                LibraryConfiguration lconfig = libcon.getDefaultOverdueDateConfiguration();
                
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();

                BorrowRecord borrowRecord = db.BorrowRecords.Find(id);  
                if (borrowRecord == null) throw new BorrowRecordsExceptions.BorrowRecordNotFoundExceptions(); 
                
                
                var query= from R in db.BorrowRecords
                    join D in db.BorrowRecordDetails on R.BorrowId equals D.BorrowId
                    join BorrowMember in db.Members on D.BorrowerMemberId equals BorrowMember.MemberId
                    join ApproverMember in db.Members on D.ApproverMemberId equals ApproverMember.MemberId
                    join BorrowUser in db.Users on BorrowMember.UserId equals BorrowUser.UserId
                    join ApproveUser in db.Users on ApproverMember.UserId equals ApproveUser.UserId
                    join B in db.Books on R.BookId equals B.BookId
                    join BS in db.BookStatuses on B.BookStatusID equals BS.BookStatusID
                    where R.BorrowId == id && R.isDeleted == false 
                    select new ApprovedBorrowedRecordsViewModel
                    {
                        BorrowId = R.BorrowId,
                        BorrowerMembershipCode = BorrowMember.MembershipCode,
                        BorrowerName = BorrowUser.FullName,
                        ApproverMembershipCode = ApproverMember.MembershipCode,
                        ApproverName = ApproveUser.FullName,
                        Book = B.Title,
                        BorrowDate = R.BorrowDate,
                        ScheduledReturnDate = R.ScheduledReturnDate,
                        ActualReturnDate = R.ActualReturnDate,
                        OverdueStatus = R.OverdueStatus,
                        OverdueAmount = R.OverdueAmount,
                        BookStatus = BS.BookStatusName
                    };
                
                
                ApprovedBorrowedRecordsViewModel RecordDetails = query.ToList()[0];
                int overduedays=0;

                if (RecordDetails.ScheduledReturnDate.HasValue &&
                    RecordDetails.ScheduledReturnDate.Value.Date < DateTime.Now.Date)
                { 
                    overduedays = (DateTime.Now.Date - RecordDetails.ScheduledReturnDate.Value.Date).Days;
                    
                    RecordDetails.OverdueStatus = true;
                    borrowRecord.OverdueStatus = true;

                    
                }

                double OverdueAmountPerDay;
                
                if(!Double.TryParse(lconfig.ConfigValue.Trim(), out OverdueAmountPerDay)) throw new BorrowRecordsExceptions.BorrowConfigurationsValueNotSetProperly();
               
                //Record Details for the view while borrowRecord will be the one to get updated
                 RecordDetails.OverdueAmount = OverdueAmountPerDay * overduedays;
                 borrowRecord.OverdueAmount = RecordDetails.OverdueAmount;
                 
                 RecordDetails.ActualReturnDate = DateTime.Now.Date;
                 borrowRecord.ActualReturnDate = RecordDetails.ActualReturnDate;
                 borrowRecord.BorrowStatusId = (int)BorrowStatus.ReturnedOrAvailable;
                 ViewBag.RecordDetails = RecordDetails;

                return View(borrowRecord);
            }
            catch (BorrowRecordsExceptions.BorrowRecordNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return RedirectToAction("ApprovedBorrowedRecords");
            }
            catch (CustomCommonExceptions.BadRequestExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message;   
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return RedirectToAction("ApprovedBorrowedRecords");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return RedirectToAction("ApprovedBorrowedRecords");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Return(BorrowRecord borrowRecord)
        {
            try
            {
                long userid = Convert.ToInt64(Session["UserId"].ToString());
                Member member = db.Members.FirstOrDefault(m => m.UserId == userid);

                #region borrowrecords update

                var dbborrow = db.BorrowRecords.Find(borrowRecord.BorrowId);
                if (dbborrow == null) throw new BorrowRecordsExceptions.BorrowRecordNotFoundExceptions();
                
                dbborrow.ActualReturnDate = borrowRecord.ActualReturnDate;
                dbborrow.BorrowStatusId = borrowRecord.BorrowStatusId;
                dbborrow.OverdueStatus = borrowRecord.OverdueStatus;
                dbborrow.OverdueAmount = borrowRecord.OverdueAmount;

                #endregion

                #region BorrowRecordDetails

                var dbBorrowrecorddetail = db.BorrowRecordDetails
                    .Where(bd => bd.BorrowId == borrowRecord.BorrowId && bd.BorrowStatusId == (int)BorrowStatus.ApprovedOrBorrowed).SingleOrDefault();
                
                if (dbBorrowrecorddetail == null)
                    throw new BorrowRecordsExceptions.BorrowRecordDetailsNotFoundExceptions();
                
                BorrowRecordDetail borrowRecordDetail = new BorrowRecordDetail();
                borrowRecordDetail.BorrowId = borrowRecord.BorrowId;
                borrowRecordDetail.BorrowStatusId = borrowRecord.BorrowStatusId;
                borrowRecordDetail.BorrowerMemberId = dbBorrowrecorddetail.BorrowerMemberId;
                borrowRecordDetail.ApproverMemberId = dbBorrowrecorddetail.ApproverMemberId;
                borrowRecordDetail.BorrowRecordDetailsDate = DateTime.Now;
                
                db.BorrowRecordDetails.Add(borrowRecordDetail);

                #endregion
                
                #region book status change

                var dbbook = db.Books.Find(dbborrow.BookId);
                if (dbbook == null) throw new BookMasterExceptions.BookNotFoundExceptions();
                dbbook.BookStatusID = (int)BookStatus.Available; //chnged to available
                
                #endregion
                
                #region Member book borrow count update
                Member BorrowerMember = db.Members.Find(dbborrow.MemberId);

                BorrowerMember.TotalBooksBorrowed= BorrowerMember.TotalBooksBorrowed != null && BorrowerMember.TotalBooksBorrowed > 0 ? BorrowerMember.TotalBooksBorrowed - 1 : 0;
                
                
                #endregion 

                db.SaveChanges();
                TempData["SuccessMessage"] = "Request has been sent successfully";
                return RedirectToAction("ApprovedBorrowedRecords");
            }
            catch (BookMasterExceptions.BookNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Return",borrowRecord.BorrowId);
            }
            catch (BorrowRecordsExceptions.BorrowRecordNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Return",borrowRecord.BorrowId);
            }
            catch (BorrowRecordsExceptions.BorrowRecordDetailsNotFoundExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message;   
                return RedirectToAction("Return",borrowRecord.BorrowId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return RedirectToAction("Return",borrowRecord.BorrowId);
            }
        
            
        }
        
        #endregion
        
        #region Show BorrowRecordHistory
        public ActionResult BorrowRecordHistory(
            int? page,
            string BorrowRecordHistoryId,
            string BorrowEvent,
            string BorrowingUserName,
            string ApprovingUserName,
            string BorrowDate,
            string ScheduledReturnDate,
            string ActualReturnDate,
            string OverdueAmount,
            string OverdueStatus)
        {
            try
            {
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                IPagedList<BorrowRecordHistoryViewModel> PagedList;
                List<BorrowRecordHistoryViewModel> model;
             
                IQueryable<BorrowRecordHistoryViewModel> query;
                long userid = Convert.ToInt64(Session["UserId"].ToString());
                long roleid = Convert.ToInt64(Session["UserRole"].ToString());
                if (!string.IsNullOrWhiteSpace(BorrowRecordHistoryId) ||
                    !string.IsNullOrWhiteSpace(BorrowEvent) ||
                    !string.IsNullOrWhiteSpace(BorrowingUserName) ||
                    !string.IsNullOrWhiteSpace(ApprovingUserName) ||
                    !string.IsNullOrWhiteSpace(BorrowDate) ||
                    !string.IsNullOrWhiteSpace(ScheduledReturnDate) ||
                    !string.IsNullOrWhiteSpace(ActualReturnDate) ||
                    !string.IsNullOrWhiteSpace(OverdueAmount) ||
                    !string.IsNullOrWhiteSpace(OverdueStatus))
                {
                    
                    query= clsDBOperations.GetBorrowRecordHistoryFiltered(roleid,userid,BorrowRecordHistoryId,BorrowEvent,BorrowingUserName,ApprovingUserName,BorrowDate,ScheduledReturnDate,ActualReturnDate,OverdueAmount,OverdueStatus).OrderBy(d => d.BorrowRecordHistoryId);;
                    PagedList = query.ToPagedList(pageNumber, pageSize);
                   model = query.ToList();
                    return View(model);
                }
                
                
                
                query = clsDBOperations.GetBorrowRecordHistoryDefault(roleid,userid).OrderBy(d => d.BorrowRecordHistoryId);
                 PagedList = query.ToPagedList(pageNumber, pageSize);
                  model = query.ToList();
                return View(model); //only those with not deleted flag will run
            }
            catch (BorrowRecordsExceptions.BorrowRecordNotFoundExceptions ex)
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



    }
}