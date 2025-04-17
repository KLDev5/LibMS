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
using Microsoft.EntityFrameworkCore;


namespace LibraryManagement.ClsLib
{
    public class clsDBOperations
    {
        private static string dbcon = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private static LibraryDBContext db = new LibraryDBContext();

        public clsDBOperations()
        {
            
        }
        #region BookOperations
        #region GetBooksdefault

        public static IQueryable<Book> GetBooksDefault()
        {
            try
            {
                var booklist = db.Books.Where(b => b.isDeleted == false)
                    .ToList().OrderBy(b => b.BookId);

                IQueryable<Book> Books = booklist.AsQueryable();
                return Books;
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }
        
        public static IQueryable<Book> GetBooksDefault(bool? All, bool? OnlyAvailable)
        {
            try
            {
                IQueryable<Book> Books;
                //if only available is true
                if (OnlyAvailable??false)
                {
                    Books = db.Books.Where(b => b.isDeleted == false && b.BookStatusID == 1)
                        .ToList().OrderBy(b => b.BookId).AsQueryable();
                    return Books;
                }
                
                //default
                Books = db.Books
                        .ToList().OrderBy(b => b.BookId).AsQueryable();


                return Books;
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }

        

        #endregion
        
        #region Fetch Filtered Table
        public static  IQueryable<Book>  GetFilteredBooks(string Title,string Author,string Publisher,string BookStatus,bool IsDeleted)
        {
            try
            {
                //decoding Title
                string encoded = Title;
                Title = HttpUtility.UrlDecode(encoded);

                List<Book> booklist = new List<Book>();
                IQueryable<Book> Books;

                using (var connection = new SqlConnection(dbcon))
                {
                    using (var command = new SqlCommand("sp_FilteredViewBooks", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Title",
                            string.IsNullOrEmpty(Title) ? (object)DBNull.Value : Title);
                        command.Parameters.AddWithValue("@Author",
                            string.IsNullOrEmpty(Author) ? (object)DBNull.Value : Author);
                        command.Parameters.AddWithValue("@Publisher",
                            string.IsNullOrEmpty(Publisher) ? (object)DBNull.Value : Publisher);
                        command.Parameters.AddWithValue("@BookStatusID",
                            string.IsNullOrEmpty(BookStatus) ? (object)DBNull.Value : Convert.ToInt32(BookStatus));
                        command.Parameters.AddWithValue("@IsDeleted",
                            IsDeleted?1:(object)DBNull.Value);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // booklist.Add(new Book());
                                // throw new DatabaseExceptions.DatabaseOperationException();


                                while (reader.Read())
                                {
                                    var Book = new Book
                                    {
                                        BookId = (long)reader["BookId"],
                                        Title = reader["Title"].ToString(),
                                        Author = reader["Author"].ToString(),
                                        Publisher = reader["Publisher"].ToString(),
                                        ISBN = reader["ISBN"].ToString(),
                                        PublishedDate = (DateTime)reader["PublishedDate"],
                                        isDeleted = (bool)reader["isdeleted"],
                                        BookStatusID = (long)reader["BookStatusID"]
                                        // Map other properties as needed
                                    };
                                    booklist.Add(Book);
                                }
                            }
                        }
                    }
                }

                Books = booklist.AsQueryable();
                if (Books == null) throw new Exception("Sql Query didn't return Books");
                return Books;
            }
            catch (System.NullReferenceException ex)
            {
                throw ex;
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        #endregion
        #endregion 
        
        #region useroperations

        public static IQueryable<User> GetUsersDefault(long role)
        {
            IQueryable<User> Users;
            IOrderedQueryable<User> userlist;
            try
            {
                if (role == 1)
                {
                    userlist = db.Users
                        .Where(u => u.IsDeleted == false) // Filter out deleted users
                        .OrderBy(u => u.UserId);
                }
                else if (role == 2)
                {
                    userlist = db.Users
                        .Where(u => u.IsDeleted == false && u.RoleId == 3) // Filter out deleted users
                        .OrderBy(u => u.UserId);
                }
                else
                {
                   
                    userlist = db.Users
                        .Where(u => u.IsDeleted == false && u.RoleId == 3) // Filter out deleted users
                        .OrderBy(u => u.UserId); //this needs to be changed.. as visibility of this link is not there in login.layout we have not changed the code
                }

                Users = userlist.AsQueryable();
                return Users;
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static IQueryable<User> GetUsersFiltered(long role,string searchUserId,string searchFullName,string searchEmail,string SearchRoleID,bool isDeleted)
        {
            try
            {
                IQueryable<User> Users;
                List<User> userlist = new List<User>();
                long? FilterRole;
                if (role != 1)
                {
                    FilterRole = 3;

                }
                else
                {
                    FilterRole = string.IsNullOrEmpty(SearchRoleID) || !long.TryParse(SearchRoleID,out var parseRoleID)
                        ?(long?) null
                        : parseRoleID; 
                        
                }

                using (var connection = new SqlConnection(dbcon))
                {
                    using (var command = new SqlCommand("sp_FilteredViewUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId",
                            string.IsNullOrEmpty(searchUserId) || !long.TryParse(searchUserId, out _)
                                ? (object)DBNull.Value
                                : Convert.ToInt64(searchUserId));
                        command.Parameters.AddWithValue("@FullName",
                            string.IsNullOrEmpty(searchFullName) ? (object)DBNull.Value : searchFullName);
                        command.Parameters.AddWithValue("@Email",
                            string.IsNullOrEmpty(searchEmail) ? (object)DBNull.Value : searchEmail);
                        command.Parameters.AddWithValue("@RoleId",
                            FilterRole == null
                                ? (object)DBNull.Value
                                : FilterRole);
                        command.Parameters.AddWithValue("@IsDeleted",
                            isDeleted?1:(object)DBNull.Value);

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // booklist.Add(new Book());
                                // throw new DatabaseExceptions.DatabaseOperationException();


                                while (reader.Read())
                                {
                                    var User = new User
                                    {
                                        UserId = (long)reader["UserId"],
                                        FullName = reader["FullName"].ToString(),
                                        Email = reader["Email"].ToString(),
                                        PasswordHash = reader["PasswordHash"].ToString(),
                                        RoleId = Convert.ToInt64(reader["RoleId"].ToString()),
                                        IsDeleted = bool.Parse(reader["IsDeleted"].ToString())
                                        // Map other properties as needed
                                    };
                                    userlist.Add(User);
                                }
                            }
                        }
                    }
                }

                Users = userlist.AsQueryable();
                if (Users == null) throw new Exception("No Data Returned");
                return Users;
            }
            catch (System.NullReferenceException ex)
            {
                throw ex;
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        #endregion
        
        #region BorrowOperations
       
        public static IQueryable<ApprovedBorrowedRecordsViewModel> GetApprovedRequestsDefault(long role, long userid)
        {
            try
            {
                IQueryable<ApprovedBorrowedRecordsViewModel> query;
                if (role == 3)
                {
                    query = from R in db.BorrowRecords
                        join D in db.BorrowRecordDetails on R.BorrowId equals D.BorrowId
                        join BorrowMember in db.Members on D.BorrowerMemberId equals BorrowMember.MemberId
                        join ApproverMember in db.Members on D.ApproverMemberId equals ApproverMember.MemberId
                        join BorrowUser in db.Users on BorrowMember.UserId equals BorrowUser.UserId
                        join ApproveUser in db.Users on ApproverMember.UserId equals ApproveUser.UserId
                        join B in db.Books on R.BookId equals B.BookId
                        join BS in db.BookStatuses on B.BookStatusID equals BS.BookStatusID
                        where R.BorrowStatusId == 2 && R.isDeleted == false &&
                              BorrowUser.UserId ==
                              userid //request data only of individual member when the user is only member
                        orderby R.BorrowId descending
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
                }
                else
                {
                    query = from R in db.BorrowRecords
                        join D in db.BorrowRecordDetails on R.BorrowId equals D.BorrowId
                        join BorrowMember in db.Members on D.BorrowerMemberId equals BorrowMember.MemberId
                        join ApproverMember in db.Members on D.ApproverMemberId equals ApproverMember.MemberId
                        join BorrowUser in db.Users on BorrowMember.UserId equals BorrowUser.UserId
                        join ApproveUser in db.Users on ApproverMember.UserId equals ApproveUser.UserId
                        join B in db.Books on R.BookId equals B.BookId
                        join BS in db.BookStatuses on B.BookStatusID equals BS.BookStatusID
                        where R.BorrowStatusId == 2 && R.isDeleted == false 
                            //request data only of individual member when the user is only member
                        orderby R.BorrowId descending
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
                    
                }

                return query;
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IQueryable<ApprovedBorrowedRecordsViewModel> GetApprovedRequestsFiltered(   
            long role,
            long userId,
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
                
                DateTime parsedDate;
                List<ApprovedBorrowedRecordsViewModel> ApprovedList = new List<ApprovedBorrowedRecordsViewModel>();
                IQueryable<ApprovedBorrowedRecordsViewModel> ApprovedBorrowedRecords;

                using (var connection = new SqlConnection(dbcon))
                {
                    using (var command = new SqlCommand("sp_FilteredViewApprovedRequest", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BorrowerMembershipCode",
                            string.IsNullOrWhiteSpace(borrowerMembershipCode) ? (object)DBNull.Value : borrowerMembershipCode.Trim());

                        command.Parameters.AddWithValue("@BorrowerFullName",
                            string.IsNullOrWhiteSpace(borrowerFullName) ? (object)DBNull.Value : borrowerFullName.Trim());

                        command.Parameters.AddWithValue("@ApproverMembershipCode",
                            string.IsNullOrWhiteSpace(approverMembershipCode) ? (object)DBNull.Value : approverMembershipCode.Trim());

                        command.Parameters.AddWithValue("@ApproverFullName",
                            string.IsNullOrWhiteSpace(approverFullName) ? (object)DBNull.Value : approverFullName.Trim());

                        command.Parameters.AddWithValue("@BookTitle",
                            string.IsNullOrWhiteSpace(bookTitle) ? (object)DBNull.Value : bookTitle.Trim());

                        
                        if (!string.IsNullOrWhiteSpace(borrowId) && long.TryParse(borrowId, out long parsedBorrowId))
                            command.Parameters.AddWithValue("@BorrowId", parsedBorrowId);
                        else
                            command.Parameters.AddWithValue("@BorrowId", DBNull.Value);

                       
                        if (!string.IsNullOrWhiteSpace(borrowDate) && DateTime.TryParse(borrowDate, out DateTime parsedBorrowDate))
                            command.Parameters.AddWithValue("@BorrowDate", parsedBorrowDate.Date);
                        else
                            command.Parameters.AddWithValue("@BorrowDate", DBNull.Value);

                        if (!string.IsNullOrWhiteSpace(scheduledReturnDate) && DateTime.TryParse(scheduledReturnDate, out DateTime parsedScheduledReturnDate))
                            command.Parameters.AddWithValue("@ScheduledReturnDate", parsedScheduledReturnDate.Date);
                        else
                            command.Parameters.AddWithValue("@ScheduledReturnDate", DBNull.Value);

                        // OverdueStatus (nullable bool)
                        if (!string.IsNullOrWhiteSpace(overdueStatus))
                        {
                            if (Convert.ToInt32(overdueStatus) == 1 || Convert.ToInt32(overdueStatus) == 0)
                            {
                                command.Parameters.AddWithValue("@OverdueStatus",Convert.ToInt32(overdueStatus) );
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@OverdueStatus", DBNull.Value);
                            }
                        }
                            
                        else
                            command.Parameters.AddWithValue("@OverdueStatus", DBNull.Value);
                        
                        if(role==3)
                        {
                            command.Parameters.AddWithValue("@UserId",userId );
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@UserId",(object)DBNull.Value );

                        }
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // booklist.Add(new Book());
                                // throw new DatabaseExceptions.DatabaseOperationException();


                                while (reader.Read())
                                {
                                    var approvedRequest = new ApprovedBorrowedRecordsViewModel
                                    {
                                        BorrowId = (long)reader["BorrowId"],
                                        BorrowerMembershipCode = reader["BorrowerMembershipCode"].ToString(),
                                        BorrowerName = reader["BorrowerName"].ToString(),
                                        ApproverMembershipCode = reader["ApproverMembershipCode"].ToString(),
                                        ApproverName = reader["ApproverName"].ToString(),
                                        Book = reader["Book"].ToString(),
                                        BorrowDate = (DateTime)reader["BorrowDate"],
                                        ScheduledReturnDate = (DateTime)reader["ScheduledReturnDate"],
                                        ActualReturnDate = reader["ActualReturnDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ActualReturnDate"],
                                        OverdueStatus = reader["OverdueStatus"] != DBNull.Value && (bool)reader["OverdueStatus"],
                                        OverdueAmount = reader["OverdueAmount"] == DBNull.Value ? (double?)null : (double)reader["OverdueAmount"],
                                        BookStatus = reader["BookStatus"].ToString()
                                    };
                                    ApprovedList.Add(approvedRequest);
                                }
                            }
                        }
                    }
                }
                
                ApprovedBorrowedRecords= ApprovedList.AsQueryable();
                return ApprovedBorrowedRecords;
                
                
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public static IQueryable<PendingRequestViewModel>  GetPendingBorrowRequestsDefault(long role, long userid)
        {
            try
            {
                IQueryable<PendingRequestViewModel> query;
                if (role == 3)
                {
                    query = from r in db.BorrowRecords
                        join d in db.BorrowRecordDetails on r.BorrowId equals d.BorrowId
                        join m in db.Members on r.MemberId equals m.MemberId
                        join b in db.Books on r.BookId equals b.BookId
                        join u in db.Users on m.UserId equals u.UserId
                        where r.BorrowStatusId == 1 && r.isDeleted == false &&
                              u.UserId == userid &&
                              b.BookStatusID==1 
                        //request data only of individual member when the user is only member and book is not borrowed(it should eb available)
                        orderby r.BorrowId
                        select new PendingRequestViewModel
                        {
                            BorrowId = r.BorrowId,
                            MembershipCode = m.MembershipCode,
                            MemberName = u.FullName,
                            BookTitle = b.Title,
                            RequestDate = d.BorrowRecordDetailsDate
                        };
                    
                }
                else
                {
                    query = from r in db.BorrowRecords
                        join d in db.BorrowRecordDetails on r.BorrowId equals d.BorrowId
                        join m in db.Members on r.MemberId equals m.MemberId
                        join b in db.Books on r.BookId equals b.BookId
                        join u in db.Users on m.UserId equals u.UserId
                        where r.BorrowStatusId == 1 && r.isDeleted == false
                                                    &&
                                                    b.BookStatusID==1 
                        orderby r.BorrowId
                        select new PendingRequestViewModel
                        {
                            BorrowId = r.BorrowId,
                            MembershipCode = m.MembershipCode,
                            MemberName = u.FullName,
                            BookTitle = b.Title,
                            RequestDate = d.BorrowRecordDetailsDate
                        };
                }

                return query;
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IQueryable<PendingRequestViewModel> GetPendingBorrowRequestsFiltered(long role, long userid, string searchFullName,string searchMembershipCode,string searchBookName, string SearchRequestDate )
        {
            
            try
            {
                DateTime parsedDate;
                List<PendingRequestViewModel> PendingRequestList = new List<PendingRequestViewModel>();
                IQueryable<PendingRequestViewModel> PendingRequests;

                using (var connection = new SqlConnection(dbcon))
                {
                    using (var command = new SqlCommand("sp_FilteredViewPendingBorrowRequest", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FullName",
                            string.IsNullOrEmpty(searchFullName) ? (object)DBNull.Value : searchFullName.Trim());
                        command.Parameters.AddWithValue("@MembershipCode",
                            string.IsNullOrEmpty(searchMembershipCode) ? (object)DBNull.Value : searchMembershipCode.Trim());
                        command.Parameters.AddWithValue("@BookName",
                            string.IsNullOrEmpty(searchBookName) ? (object)DBNull.Value : searchBookName.Trim());
                        command.Parameters.AddWithValue("@RequestDate",
                            !string.IsNullOrEmpty(SearchRequestDate) && DateTime.TryParse(SearchRequestDate, out parsedDate)? parsedDate.Date : (object)DBNull.Value);
                        
                        if(role==3)
                        {
                            command.Parameters.AddWithValue("@UserId",userid );
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@UserId",(object)DBNull.Value );

                        }
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // booklist.Add(new Book());
                                // throw new DatabaseExceptions.DatabaseOperationException();


                                while (reader.Read())
                                {
                                    var PendingRequest = new PendingRequestViewModel
                                    {
                                        BorrowId = (long)reader["BorrowId"],
                                        MembershipCode = reader["MembershipCode"].ToString(),
                                        MemberName = reader["MemberName"].ToString(),
                                        BookTitle = reader["BookTitle"].ToString(),
                                        RequestDate = (DateTime)reader["RequestDate"],
                                        
                                        // Map other properties as needed
                                    };
                                    PendingRequestList.Add(PendingRequest);
                                }
                            }
                        }
                    }
                }
                
                PendingRequests= PendingRequestList.AsQueryable();
                return PendingRequests;

            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IQueryable<BorrowRecordHistoryViewModel> GetBorrowRecordHistoryDefault(long role,long user)
        {
            IQueryable<BorrowRecordHistoryViewModel> query;

            try
            {
                if (role == 3)
                {
                    query = from d in db.BorrowRecordDetails
                        join bs in db.BorrowStatuses on d.BorrowStatusId equals bs.BorrowStatusId
                        join r in db.BorrowRecords on d.BorrowId equals r.BorrowId into borrowRecords
                        from r in borrowRecords.DefaultIfEmpty() // Left Join
                        join b in db.Books on r.BookId equals b.BookId

                        join aMembers in db.Members on d.ApproverMemberId equals aMembers.MemberId into approvers
                        from aMembers in approvers.DefaultIfEmpty() // Left Join

                        join bMembers in db.Members on d.BorrowerMemberId equals bMembers.MemberId into borrowers
                        from bMembers in borrowers.DefaultIfEmpty() // Left Join

                        join approvingUser in db.Users on aMembers.UserId equals approvingUser.UserId into
                            approvingUsers
                        from approvingUser in approvingUsers.DefaultIfEmpty() // Left Join

                        join borrowingUser in db.Users on bMembers.UserId equals borrowingUser.UserId into
                            borrowingUsers

                        from borrowingUser in borrowingUsers.DefaultIfEmpty() // Left Join
                        where borrowingUser.UserId == user
                        select new BorrowRecordHistoryViewModel
                        {
                            BorrowRecordHistoryId = d.BorrowRecordDetailsId, // Renamed as per request
                            BorrowEvent = bs.BorrowStatusName,
                            Book = b.Title,
                            Approver = approvingUser.FullName,
                            Borrower = borrowingUser.FullName,
                            EventDate = d.BorrowRecordDetailsDate,
                            ScheduledReturnDate = r.ScheduledReturnDate,
                            ActualReturnDate = r.ActualReturnDate,
                            OverdueAmount = r.OverdueAmount,
                            OverdueStatus = r.OverdueStatus
                        };
                }
                else
                {
                     query = from d in db.BorrowRecordDetails
                        join bs in db.BorrowStatuses on d.BorrowStatusId equals bs.BorrowStatusId
                        join r in db.BorrowRecords on d.BorrowId equals r.BorrowId into borrowRecords
                        from r in borrowRecords.DefaultIfEmpty() // Left Join
                        join b in db.Books on r.BookId equals b.BookId

                        join aMembers in db.Members on d.ApproverMemberId equals aMembers.MemberId into approvers
                        from aMembers in approvers.DefaultIfEmpty() // Left Join

                        join bMembers in db.Members on d.BorrowerMemberId equals bMembers.MemberId into borrowers
                        from bMembers in borrowers.DefaultIfEmpty() // Left Join

                        join approvingUser in db.Users on aMembers.UserId equals approvingUser.UserId into
                            approvingUsers
                        from approvingUser in approvingUsers.DefaultIfEmpty() // Left Join

                        join borrowingUser in db.Users on bMembers.UserId equals borrowingUser.UserId into
                            borrowingUsers

                        from borrowingUser in borrowingUsers.DefaultIfEmpty() // Left Join
                        
                        select new BorrowRecordHistoryViewModel
                        {
                            
                            
                            BorrowRecordHistoryId = d.BorrowRecordDetailsId, // Renamed as per request
                            BorrowEvent = bs.BorrowStatusName,
                            Book = b.Title,
                            Approver = approvingUser.FullName,
                            Borrower = borrowingUser.FullName,
                            EventDate = d.BorrowRecordDetailsDate,
                            ScheduledReturnDate = r.ScheduledReturnDate,
                            ActualReturnDate = r.ActualReturnDate,
                            OverdueAmount = r.OverdueAmount,
                            OverdueStatus = r.OverdueStatus
                        };
                
                }

                return query;
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public static IQueryable<BorrowRecordHistoryViewModel> GetBorrowRecordHistoryFiltered(
            long role,
            long user,
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
                DateTime parsedDate;
                float parsedFloat;
                bool parsedBool;

                List<BorrowRecordHistoryViewModel> BorrowHistoryList = new List<BorrowRecordHistoryViewModel>();
                IQueryable<BorrowRecordHistoryViewModel> BorrowRecords;

                using (var connection = new SqlConnection(dbcon))
                {
                    using (var command = new SqlCommand("sp_FilteredViewBorrowTransactionHistory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@BorrowRecordHistoryId",
                            string.IsNullOrEmpty(BorrowRecordHistoryId)
                                ? (object)DBNull.Value
                                : Convert.ToInt64(BorrowRecordHistoryId.Trim()));

                        command.Parameters.AddWithValue("@BorrowEvent",
                            string.IsNullOrEmpty(BorrowEvent) ? (object)DBNull.Value : BorrowEvent.Trim());

                        command.Parameters.AddWithValue("@BorrowingUserName",
                            string.IsNullOrEmpty(BorrowingUserName) ? (object)DBNull.Value : BorrowingUserName.Trim());

                        command.Parameters.AddWithValue("@ApprovingUserName",
                            string.IsNullOrEmpty(ApprovingUserName) ? (object)DBNull.Value : ApprovingUserName.Trim());

                        command.Parameters.AddWithValue("@BorrowDate",
                            !string.IsNullOrEmpty(BorrowDate) && DateTime.TryParse(BorrowDate, out parsedDate)
                                ? parsedDate.Date
                                : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@ScheduledReturnDate",
                            !string.IsNullOrEmpty(ScheduledReturnDate) && DateTime.TryParse(ScheduledReturnDate, out parsedDate)
                                ? parsedDate.Date
                                : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@ActualReturnDate",
                            !string.IsNullOrEmpty(ActualReturnDate) && DateTime.TryParse(ActualReturnDate, out parsedDate)
                                ? parsedDate.Date
                                : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@OverdueAmount",
                            !string.IsNullOrEmpty(OverdueAmount) && float.TryParse(OverdueAmount, out parsedFloat)
                                ? parsedFloat
                                : (object)DBNull.Value);

                        command.Parameters.AddWithValue("@OverdueStatus",
                            !string.IsNullOrEmpty(OverdueStatus) && OverdueStatus.Trim()=="1");
                        if(role==3)
                        {
                            command.Parameters.AddWithValue("@UserId",user );
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@UserId",(object)DBNull.Value );

                        }
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BorrowHistoryList.Add(new BorrowRecordHistoryViewModel
                                {
                                    BorrowRecordHistoryId = reader.GetInt64(reader.GetOrdinal("BorrowRecordHistoryId")),
                                    BorrowEvent = reader["BorrowEvent"]?.ToString(),
                                    Book = reader["Book"]?.ToString(),
                                    Approver = reader["Approver"]?.ToString(),
                                    Borrower = reader["Borrower"]?.ToString(),
                                    EventDate = reader.GetDateTime(reader.GetOrdinal("EventDate")),
                                    ScheduledReturnDate = reader["ScheduledReturnDate"] as DateTime?,
                                    ActualReturnDate = reader["ActualReturnDate"] as DateTime?,
                                    OverdueAmount = reader["OverdueAmount"] as float?,
                                    OverdueStatus = reader["OverdueStatus"].ToString().Trim()=="1"
                                });
                            }
                        }

                    }
                }
                return BorrowHistoryList.AsQueryable();

            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            
        }


        #endregion
        
        #region members filtered view

        public static IQueryable<MemberView> GetMembersDefault(long role,long user)
        {
            try
            {
                if (role == 3)
                {


                 return  db.Members
                        .Where(m => m.User.RoleId == 3 && m.User.UserId == user) // Filter members with RoleId = 3
                        .OrderBy(m => m.MemberId) // Ensure ordering before pagination
                        .Select(m => new MemberView
                        {
                            MemberId = m.MemberId,
                            MembershipCode = m.MembershipCode,
                            FullName = m.User.FullName,
                            PhoneNumber = m.PhoneNumber,
                            Address = m.Address,
                            DateOfBirth = m.DateOfBirth,
                            JoinDate = m.JoinDate,
                            Email = m.User.Email,
                            RoleId = m.User.RoleId,
                            OverdueCount = m.OverdueCount,
                            OverdueLimit = m.OverdueLimit,
                            TotalBooksBorrowed = m.TotalBooksBorrowed
                        });
                }
                else
                {
                 return   db.Members
                        .Where(m => m.User.RoleId == 3 && m.User.IsDeleted == false) // Filter members with RoleId = 3
                        .OrderBy(m => m.MemberId) // Ensure ordering before pagination
                        .Select(m => new MemberView
                        {
                            MemberId = m.MemberId,
                            MembershipCode = m.MembershipCode,
                            FullName = m.User.FullName,
                            PhoneNumber = m.PhoneNumber,
                            Address = m.Address,
                            DateOfBirth = m.DateOfBirth,
                            JoinDate = m.JoinDate,
                            Email = m.User.Email,
                            RoleId = m.User.RoleId,
                            OverdueCount = m.OverdueCount,
                            OverdueLimit = m.OverdueLimit,
                            TotalBooksBorrowed = m.TotalBooksBorrowed
                        });



                }
                
            }
            catch (DatabaseExceptions.DatabaseOperationException ex)
            {
                throw ex.InnerException ?? ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            
        }

    public static IQueryable<MemberView> GetMembersFiltered(
    long role,
    long user,
    string memberId,
    string membershipCode,
    string fullName,
    string phoneNumber,
    string address,
    string dateOfBirth,
    string joinDate,
    string email,
    string roleId)
    {
        try
        {
             List<MemberView> membersList = new List<MemberView>();

            using (var connection = new SqlConnection(dbcon))
            {
                using (var command = new SqlCommand("sp_FilteredViewMembers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // UserId (based on role)
                    if (role == 3)
                        command.Parameters.AddWithValue("@UserId", user);
                    else
                        command.Parameters.AddWithValue("@UserId", DBNull.Value);

                    // MemberId
                    if (long.TryParse(memberId?.Trim(), out long parsedMemberId))
                        command.Parameters.AddWithValue("@MemberId", parsedMemberId);
                    else
                        command.Parameters.AddWithValue("@MemberId", DBNull.Value);

                    // Strings
                    command.Parameters.AddWithValue("@MembershipCode", string.IsNullOrWhiteSpace(membershipCode) ? (object)DBNull.Value : membershipCode.Trim());
                    command.Parameters.AddWithValue("@FullName", string.IsNullOrWhiteSpace(fullName) ? (object)DBNull.Value : fullName.Trim());
                    command.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(phoneNumber) ? (object)DBNull.Value : phoneNumber.Trim());
                    command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address.Trim());

                    // DateOfBirth
                    if (DateTime.TryParse(dateOfBirth?.Trim(), out DateTime dobParsed))
                        command.Parameters.AddWithValue("@DateOfBirth", dobParsed.Date);
                    else
                        command.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);

                    // JoinDate
                    if (DateTime.TryParse(joinDate?.Trim(), out DateTime joinParsed))
                        command.Parameters.AddWithValue("@JoinDate", joinParsed.Date);
                    else
                        command.Parameters.AddWithValue("@JoinDate", DBNull.Value);

                    // Email
                    command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email.Trim());

                    // RoleId
                    if (long.TryParse(roleId?.Trim(), out long parsedRoleId))
                        command.Parameters.AddWithValue("@RoleId", parsedRoleId);
                    else
                        command.Parameters.AddWithValue("@RoleId", DBNull.Value);

                    // Execute
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            membersList.Add(new MemberView
                            {
                                MemberId = reader.GetInt64(reader.GetOrdinal("MemberId")),
                                MembershipCode = reader["MembershipCode"]?.ToString(),
                                FullName = reader["FullName"]?.ToString(),
                                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                                Address = reader["Address"]?.ToString(),
                                DateOfBirth = reader["DateOfBirth"] as DateTime?,
                                JoinDate = reader["JoinDate"] as DateTime?,
                                Email = reader["Email"]?.ToString(),
                                RoleId = reader.GetInt64(reader.GetOrdinal("RoleId"))
                            });
                        }
                    }
                }
            }

            return membersList.AsQueryable();
            
            
            
        }
        catch (DatabaseExceptions.DatabaseOperationException ex)
        {
            throw ex.InnerException ?? ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }  
       

   
    }


        #endregion

    }
}