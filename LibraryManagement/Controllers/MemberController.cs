using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryManagement.Models;
using LibraryManagement.ClsLib;
using LibraryManagement.CustomExceptions;
using PagedList;
using PagedList.Mvc;

namespace LibraryManagement.Controllers
{
    public class MemberController: Controller
    {
        private LibraryDBContext db = new LibraryDBContext();
        #region Index
        
        public ActionResult Index(
            int? page,
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
                //pagination
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                IQueryable<MemberView> MemberQuery;
                IPagedList<MemberView> pagedMembers;
                List<MemberView> Members;
                ViewBag.Roles = db.Roles.ToList(); // for status names
                ViewBag.RolesList = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");

                long userid = Convert.ToInt64(Session["UserId"].ToString().Trim());
                long roleid = Convert.ToInt64(Session["UserRole"].ToString().Trim());
                // if (Session["UserRole"].ToString().Trim() == "3") //only members can view only their data
                // {
                //    
                //     
                //     pagedMembers = db.Members
                //         .Where(m => m.User.RoleId == 3 && m.User.UserId == userid) // Filter members with RoleId = 3
                //         .OrderBy(m => m.MemberId) // Ensure ordering before pagination
                //         .Select(m => new MemberView
                //         {
                //             MemberId = m.MemberId,
                //             MembershipCode = m.MembershipCode,
                //             FullName = m.User.FullName,
                //             PhoneNumber = m.PhoneNumber,
                //             Address = m.Address,
                //             DateOfBirth = m.DateOfBirth,
                //             JoinDate = m.JoinDate,
                //             Email = m.User.Email,
                //             RoleId = m.User.RoleId,
                //             OverdueCount = m.OverdueCount,
                //             OverdueLimit=m.OverdueLimit,
                //             TotalBooksBorrowed = m.TotalBooksBorrowed
                //         })
                //         .ToPagedList(pageNumber, pageSize);
                // }
                // else
                // {
                //      pagedMembers = db.Members
                //         .Where(m => m.User.RoleId == 3 && m.User.IsDeleted == false) // Filter members with RoleId = 3
                //         .OrderBy(m => m.MemberId) // Ensure ordering before pagination
                //         .Select(m => new MemberView
                //         {
                //             MemberId = m.MemberId,
                //             MembershipCode = m.MembershipCode,
                //             FullName = m.User.FullName,
                //             PhoneNumber = m.PhoneNumber,
                //             Address = m.Address,
                //             DateOfBirth = m.DateOfBirth,
                //             JoinDate = m.JoinDate,
                //             Email = m.User.Email,
                //             RoleId = m.User.RoleId,
                //             OverdueCount = m.OverdueCount,
                //             OverdueLimit=m.OverdueLimit,
                //             TotalBooksBorrowed = m.TotalBooksBorrowed
                //         })
                //         .ToPagedList(pageNumber, pageSize);
                //     
                // }
                if (!string.IsNullOrWhiteSpace(memberId) ||
                    !string.IsNullOrWhiteSpace(membershipCode) ||
                    !string.IsNullOrWhiteSpace(fullName) ||
                    !string.IsNullOrWhiteSpace(phoneNumber) ||
                    !string.IsNullOrWhiteSpace(address) ||
                    !string.IsNullOrWhiteSpace(dateOfBirth) ||
                    !string.IsNullOrWhiteSpace(joinDate) ||
                    !string.IsNullOrWhiteSpace(email) ||
                    !string.IsNullOrWhiteSpace(roleId))
                {

                    MemberQuery = clsDBOperations.GetMembersFiltered(roleid, userid, memberId, membershipCode, fullName,
                        phoneNumber, address, dateOfBirth, joinDate, email, roleId);
                    
                    pagedMembers=MemberQuery.ToPagedList(pageNumber, pageSize);
                    ViewBag.PagedMembers = pagedMembers;
                     Members= MemberQuery.ToList();
                    return View(Members);
                }
                
                
                
                
                
                 MemberQuery = clsDBOperations.GetMembersDefault(roleid, userid);
                pagedMembers=MemberQuery.ToPagedList(pageNumber, pageSize);
                ViewBag.PagedMembers = pagedMembers;
                 Members= MemberQuery.ToList();
                return View(Members); //only those with not deleted flag will run
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();
            }
        }
        #endregion
        
        #region details
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();

                Member member = db.Members.Find(id);
                if (member == null) throw new MemberMasterExceptions.MemberNotFoundException();

                var User = db.Users.Find(member.UserId);
                if (User == null) throw new UserMasterExceptions.UserNotFoundException();
                
                ViewBag.User = User; 
                ViewBag.Roles = db.Roles.ToList(); 
                
                return View(member);
            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return View();  
            }
            catch (MemberMasterExceptions.MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View();  
            }
            catch (CustomCommonExceptions.BadRequestExceptions ex)
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
        
        #region Edit
        //get
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();

                
                
                Member newMember = db.Members.Find(id);
                if(newMember == null) throw new MemberMasterExceptions.MemberNotFoundException();
                
                User user = db.Users.Find(newMember.UserId);
                if (user == null) throw new UserMasterExceptions.UserNotFoundException();

                // ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                ViewBag.User = user; 
                
                return View(newMember);
            }
            catch (CustomCommonExceptions.BadRequestExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return RedirectToAction("Index");
            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return RedirectToAction("Index");
            }
            catch (MemberMasterExceptions.MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return RedirectToAction("Index");
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
        public ActionResult Edit([Bind(Include = "MemberId,UserId,MembershipCode,PhoneNumber,Address,DateOfBirth,ExpiryDate,TotalBooksBorrowed,OutstandingFines,OverdueCount,OverdueLimit,TotalBooksBorrowed")]Member member,HttpPostedFileBase imageFile)
        {
            try
            {


                //if User Password is Hidden in this page
                if (string.IsNullOrEmpty(member.MembershipCode))
                {
                    // Remove validation error for PasswordHash
                    ModelState.Remove("MembershipCode");
                }

                if (ModelState.IsValid)
                {
                    var selectedMember = db.Members.Find(member.MemberId);
                    if (selectedMember == null) throw new MemberMasterExceptions.MemberNotFoundException();
                    selectedMember.PhoneNumber = member.PhoneNumber;
                    selectedMember.Address = member.Address;
                    selectedMember.DateOfBirth = member.DateOfBirth;
                    selectedMember.ExpiryDate = member.ExpiryDate;
                    selectedMember.TotalBooksBorrowed = member.TotalBooksBorrowed;
                    selectedMember.OutstandingFines = member.OutstandingFines;
                    selectedMember.OverdueCount = member.OverdueCount;
                    selectedMember.OverdueLimit = member.OverdueLimit;
                    selectedMember.TotalBooksBorrowed = member.TotalBooksBorrowed;
                    
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(imageFile.FileName);
                        string fileextension = Path.GetExtension(imageFile.FileName);
                        if (fileextension == ".jpg" || fileextension == ".jpeg" || fileextension == ".png")
                        {
                            string relativepath=ConfigurationManager.AppSettings["UserImageUploadPath"] + "Member_"+Guid.NewGuid() + fileextension;
                            string SavePath = Server.MapPath(relativepath);
                            imageFile.SaveAs(SavePath);
                            selectedMember.MemberImage = relativepath;

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

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Member Edited successfully";


                }

                return RedirectToAction("Index");
            }
            catch (MemberMasterExceptions.MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                // ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                User user = db.Users.Find(member.UserId);
                if (user == null) throw new UserMasterExceptions.UserNotFoundException();
                ViewBag.User = user;
                return View(member);
            }
            catch (FileFormatException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View(member);
            }
            catch (FileNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View(member);
            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return View(member);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
  
                // ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                User user = db.Users.Find(member.UserId);
                if (user == null) throw new UserMasterExceptions.UserNotFoundException();
                ViewBag.User = user; 
                return View(member);
            }

            
        }


        #endregion
    }
}