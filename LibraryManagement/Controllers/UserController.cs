using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Linq;
using System.Web.Mvc;
using LibraryManagement.Models;
using LibraryManagement.ClsLib;
using LibraryManagement.CustomExceptions;
using PagedList;
using PagedList.Mvc;
//this page will be controlled by admin users only
namespace LibraryManagement.Controllers
{
    public class UserController: Controller
    {
        private LibraryDBContext db = new LibraryDBContext();

        #region Index
        public ActionResult Index(int? page,string searchUserId,string searchFullName,string searchEmail,string SearchRoleID)
        {
            try
            {
                //pagination
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                List<User> pagedUsers;
                
                long role = Convert.ToInt64(Session["UserRole"].ToString());
                if (role == 3) return RedirectToAction("Index", "Home"); //only members not allowed to view this page
                
                ViewBag.Roles = db.Roles.ToList();
                ViewBag.RolesList=new SelectList(ViewBag.Roles, "RoleId", "RoleName");//viewbag for status name


                if (!string.IsNullOrEmpty(searchUserId) || !string.IsNullOrEmpty(searchFullName) ||
                    !string.IsNullOrEmpty(searchEmail) || !string.IsNullOrEmpty(SearchRoleID))
                {
                    pagedUsers= clsDBOperations.GetUsersFiltered(role,searchUserId.Trim(),searchFullName.Trim(),searchEmail.Trim(),SearchRoleID.Trim(),false).ToList();
                    return View(pagedUsers);
                }
                
                
                
                
                
                
                
                pagedUsers = clsDBOperations.GetUsersDefault(role).ToList();
                return View(pagedUsers); //only those with not deleted flag will run
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index", new {
                    page = (int?)null,
                    searchUserId = "",
                    searchFullName = "",
                    searchEmail = "",
                    SearchRoleID = ""
                });
            }
        }
        #endregion
        
        #region create 
        //get
        public ActionResult Create()
        {
            try
            {
                ViewBag.Roles = new SelectList(db.Roles, "RoleID", "RoleName");
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
        public ActionResult Create([Bind(Include = "UserId,FullName,Email,PasswordHash,RoleId")]User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region User Part
                    var ExistingUser = db.Users.Where(c => c.Email.Trim().ToUpper() == user.Email.Trim().ToUpper()).SingleOrDefault();
                    if (ExistingUser != null)
                    {
                        throw new LoginUserExceptions.InvalidEmailException();
                    }
                    user.IsDeleted = false;
                    user.PasswordHash = ClsPasswordEncryption.HashPassword(user.PasswordHash);
                    db.Users.Add(user);
                    
                    #endregion
                    
                    #region Corresponding Member Part
                    Member NewMember = new Member();    
                    NewMember.UserId=user.UserId;
                    NewMember.MembershipCode=ClsMembership.GenerateMembershipCode(user.FullName);
                    NewMember.IsDeleted = false;
                    NewMember.JoinDate= DateTime.Now;
                    NewMember.ExpiryDate= DateTime.Now.AddMonths(6);//6 months Membership Retaining
                    db.Members.Add(NewMember);
                    #endregion
                    
                    
                    
                    db.SaveChanges();
                    

                }
                TempData["SuccessMessage"] = "User Saved successfully";
                return RedirectToAction("Index");
            }
            catch(LoginUserExceptions.InvalidEmailException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                ViewBag.Roles = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                return View();
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;   
                ViewBag.Roles = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
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

                User user = db.Users.Find(id);
                if (user == null) throw new UserMasterExceptions.UserNotFoundException();

                ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");

                return View(user);
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FullName,Email,RoleId")]User user)
        {
            try
            {
                //if User Password is Hidden in this page
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    // Remove validation error for PasswordHash
                    ModelState.Remove("PasswordHash");
                }
                
                if (ModelState.IsValid)
                {
                    
                    
                    
                    var ExistingUser = db.Users.Where(c => c.Email.Trim().ToUpper() == user.Email.Trim().ToUpper()).SingleOrDefault();
                    if (ExistingUser != null)
                    {
                        if (ExistingUser.UserId != user.UserId)
                        {
                            throw new LoginUserExceptions.UserEmailAlreadyExistsException();
                        }
                        
                    }
                    var selecteduser=db.Users.Find(user.UserId);    
                    if (selecteduser == null) throw new UserMasterExceptions.UserNotFoundException();  
                    selecteduser.FullName = user.FullName;
                    selecteduser.Email = user.Email;    
                    // selecteduser.PasswordHash = ClsPasswordEncryption.HashPassword(user.PasswordHash);
                    selecteduser.RoleId = user.RoleId;
                    
                    // db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    
                    db.SaveChanges();

                }
                TempData["SuccessMessage"] = "User Updated Successfully";
                return RedirectToAction("Index");
            }
            catch (LoginUserExceptions.UserEmailAlreadyExistsException ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");

                return View(user);
            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");

                return View(user);
            }

            
        }


        #endregion
        
        #region Delete
        //get
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();

                User user = db.Users.Find(id);
                if (user == null) throw new UserMasterExceptions.UserNotFoundException();
                
                ViewBag.Roles = db.Roles.ToList();
                return View(user);
            }
            catch (CustomCommonExceptions.BadRequestExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index");   
            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return RedirectToAction("Index");   
            }
            catch (Exception ex)
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
                //an user is a member by default. Members can be dissolved through only usermaster page
                User user = db.Users.Find(id);
                if(user==null) throw new UserMasterExceptions.UserNotFoundException();
                user.IsDeleted = true;
                Member member = db.Members.Where(m=>m.UserId==user.UserId).FirstOrDefault();
                if (member == null) throw new MemberMasterExceptions.MemberNotFoundException("User Exists But The Corresponding Member Does not Exists");

                member.IsDeleted = true;

                // db.Books.Remove(book);
                db.SaveChanges();
                TempData["SuccessMessage"] = "User Deleted successfully";
                return RedirectToAction("Index");
            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index");
            }
            catch (MemberMasterExceptions.MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
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
               
                using (db)
                {
                    if (db.Database.ExecuteSqlCommand("exec sp_SoftDeleteAllUsers")<1)
                    {
                        throw new Exception("Sql Did not Work");
                    }
                }
                TempData["SuccessMessage"] = "All Users Deleted Successfully";
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

        public ActionResult GetAll(int?page,string searchUserId,string searchFullName,string searchEmail,string SearchRoleID,string isDeleted)
        {
            try
            {
                bool isDeletedFlag = false;
                if (!string.IsNullOrEmpty(isDeleted) && isDeleted.Trim().ToLower() == "1")
                {
                    isDeletedFlag = true;

                }
                //pagination
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                ViewBag.Roles = db.Roles.ToList();
                ViewBag.RolesList=new SelectList(ViewBag.Roles, "RoleId", "RoleName");// for status names
                
                long role = Convert.ToInt64(Session["UserRole"].ToString());
                if (role != 1) return RedirectToAction("Index", "Home");
                IPagedList<User> pagedUsers;
                if (!string.IsNullOrEmpty(searchUserId) || !string.IsNullOrEmpty(searchFullName) ||
                    !string.IsNullOrEmpty(searchEmail) || !string.IsNullOrEmpty(SearchRoleID) || isDeletedFlag)
                {
                    pagedUsers= clsDBOperations.GetUsersFiltered(role,searchUserId.Trim(),searchFullName.Trim(),searchEmail.Trim(),SearchRoleID.Trim(),isDeletedFlag).ToPagedList(pageNumber, pageSize);
                    return View(pagedUsers);
                }
 
                pagedUsers = clsDBOperations.GetUsersDefault(role).ToPagedList(pageNumber, pageSize);
                return View(pagedUsers);
            }
            
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("GetAll", new {
                    page = (int?)null,
                    searchUserId = "",
                    searchFullName = "",
                    searchEmail = "",
                    SearchRoleID = "",
                    isDeleted = ""
                });
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
                //an user is a member by default. Members can be dissolved through only usermaster page

                User user = db.Users.Find(id);
                if (user == null) throw new UserMasterExceptions.UserNotFoundException();
                var ExistingUser = db.Users.Where(c => c.Email.Trim().ToUpper() == user.Email.Trim().ToUpper()&& c.IsDeleted==false).SingleOrDefault();
                if (ExistingUser != null)
                {
                    throw new LoginUserExceptions.UserEmailAlreadyExistsException();
                }
                user.IsDeleted = false;
                
                Member member = db.Members.Where(m=>m.UserId==user.UserId).FirstOrDefault();
                if (member == null) throw new MemberMasterExceptions.MemberNotFoundException("User Exists But The Corresponding Member Does not Exists");
                member.IsDeleted = false;

                // db.Books.Remove(book);
                db.SaveChanges();
                TempData["SuccessMessage"] = "The User has been Restored Successfully";
                return RedirectToAction("Index");
            }
            catch (MemberMasterExceptions.MemberNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index");
            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index");
            }
            catch (LoginUserExceptions.UserEmailAlreadyExistsException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                return RedirectToAction("Index");
            }
        }
        #endregion
        
        #region ChangePassword
        //get
        public ActionResult ChangePassword(int? id)
        {
            try
            {
                Console.WriteLine("Entered in the code");
                if (id == null) throw new CustomCommonExceptions.BadRequestExceptions();
                Console.WriteLine("Entered ID");
                User user = db.Users.Find(id);
                if (user == null) throw new UserMasterExceptions.UserNotFoundException();

                ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                ChangePasswordView CPView = new ChangePasswordView();
                CPView.UserId=user.UserId;
                
                return View(CPView);
            }
            catch (CustomCommonExceptions.BadRequestExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");
                if (Session["UserRole"].ToString().Trim() != "3")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index","Home");
                }


            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                if (Session["UserRole"].ToString().Trim() != "3")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index","Home");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                // ViewBag.bookStatus = new SelectList(db.BookStatuses, "BookStatusID", "BookStatusName");

                if (Session["UserRole"].ToString().Trim() != "3")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index","Home");
                }
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "UserID,OldPassword,NewPassword")]ChangePasswordView CPView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    var selecteduser=db.Users.Find(CPView.UserId);    
                    if (selecteduser == null || selecteduser.IsDeleted == true) throw new UserMasterExceptions.UserNotFoundException();
                    if (ClsPasswordEncryption.VerifyPassword(CPView.OldPassword, selecteduser.PasswordHash))
                    {
                        selecteduser.PasswordHash = ClsPasswordEncryption.HashPassword(CPView.NewPassword);
                        
                    }
                    else
                    {
                        throw new LoginUserExceptions.PasswordsUnmatchingExceptions();
                    }
                  
                    
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Password Changed Successfully";
                }

                if (Session["UserRole"].ToString().Trim() != "3")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index","Home");
                }
            }
            catch (LoginUserExceptions.UserEmailAlreadyExistsException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");

                return View(CPView);
            }
            catch (UserMasterExceptions.UserNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");

                return View(CPView);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                ViewBag.RolesVB = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");

                return View(CPView);
            }

            
        }


        #endregion
        
    }
}