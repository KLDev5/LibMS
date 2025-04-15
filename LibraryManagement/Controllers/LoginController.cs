using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryManagement.Models;
using LibraryManagement.CustomExceptions;
using LibraryManagement.ClsLib;


namespace LibraryManagement.Controllers
{
    
    public class LoginController:Controller
    {
     LibraryDBContext db = new LibraryDBContext();  
        
        #region  Login

        
        //get
        [AllowAnonymous]
        public ActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                return RedirectToAction("Login");
            }
        }
        
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Email,Password")]Login login )
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = db.Users.Where(c => c.Email.Trim().ToUpper() == login.Email.Trim().ToUpper()&& c.IsDeleted==false).FirstOrDefault();
                    if (user == null)
                    {
                        throw new LoginUserExceptions.InvalidEmailException();
                    }
                    else
                    {

                        if (ClsPasswordEncryption.VerifyPassword(login.Password, user.PasswordHash))
                        {
                            Session["UserId"] = user.UserId;
                            Session["UserName"] = user.FullName;
                            Session["UserRole"] = user.RoleId;
                            
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            throw new LoginUserExceptions.InvalidPasswordsException();
                        }
                    }


                }
                else
                {
                    throw new LoginUserExceptions.InvalidFieldsException();
                }

            }
            catch (LoginUserExceptions.InvalidFieldsException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                ViewBag.Roles = new SelectList(db.Roles, "RoleID", "RoleName");
                return View();
            }
            catch (LoginUserExceptions.InvalidEmailException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                ViewBag.Roles = new SelectList(db.Roles, "RoleID", "RoleName");
                return View();
            }
            catch (LoginUserExceptions.InvalidPasswordsException ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                ViewBag.Roles = new SelectList(db.Roles, "RoleID", "RoleName");
                return View();
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;  
                ViewBag.Roles = new SelectList(db.Roles, "RoleID", "RoleName");
                return View();
            }
            
        }
       

        #endregion
        
        
        #region  Register
        
        //get
        [AllowAnonymous]
        public ActionResult Register()
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
        
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "FullName,Email,Password,VerifyPassword")]Register Register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ExistingUser = db.Users.Where(c => c.Email.Trim().ToUpper() == Register.Email.Trim().ToUpper()&& c.IsDeleted==false).SingleOrDefault();
                    if (ExistingUser != null)
                    {
                        throw new LoginUserExceptions.UserEmailAlreadyExistsException();
                    }

                    
                    if (Register.Password==null || Register.Password != Register.VerifyPassword)
                    {
                        throw new LoginUserExceptions.PasswordsUnmatchingExceptions();
                    }
                    
                    //Default Role Member upon Registering
                    var DefaultRole=db.Roles.Where(c=>c.RoleName.Trim().ToLower()=="member").SingleOrDefault();
                    if (DefaultRole == null)
                    {
                        Role newRole = new Role();
                        newRole.RoleName = "Member";
                        db.Roles.Add(newRole);
                        db.SaveChanges();
                        var NewlyAddedMemberRole=db.Roles.Where(c=>c.RoleName.Trim().ToLower()=="member").SingleOrDefault();
                        DefaultRole = NewlyAddedMemberRole;
                    }
                    
                    User user = new User();  
                    
                    #region User Add part
                    user.FullName= Register.FullName;
                    user.Email=Register.Email;
                    user.IsDeleted = false;
                    user.RoleId = DefaultRole.RoleId;
                    user.PasswordHash = ClsPasswordEncryption.HashPassword(Register.Password);
                    db.Users.Add(user);
                    #endregion
                    
                    #region Corresponding Member Part
                    Member NewMember = new Member();    
                    NewMember.UserId=user.UserId;
                    NewMember.MembershipCode=ClsMembership.GenerateMembershipCode(user.FullName);
                    NewMember.IsDeleted = false;
                    NewMember.JoinDate = DateTime.Now;
                    NewMember.ExpiryDate = DateTime.Now.AddMonths(6);
                    db.Members.Add(NewMember);
                    #endregion
                    
                    
                    db.SaveChanges();
                    TempData["SuccessMessage"]="Register Success. Please Login Now"; 
                    return RedirectToAction("Login");

                }
                else
                {
                    throw new LoginUserExceptions.InvalidFieldsException();
                }
                 
                
            }
            catch(LoginUserExceptions.UserEmailAlreadyExistsException ex)
            {
                TempData["ErrorMessage"] = ex.Message; 
                ViewBag.Roles = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                return View();
            }
            catch(LoginUserExceptions.PasswordsUnmatchingExceptions ex)
            {
                TempData["ErrorMessage"] = ex.Message;   
                ViewBag.Roles = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
                return View();
            }
            catch(LoginUserExceptions.InvalidFieldsException ex)
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
        
        #region Logout
        public ActionResult Logout()
        {
            Session.Clear(); // Remove all session data
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }
        #endregion
    }
}