using System;
namespace LibraryManagement.CustomExceptions
{
    public class UserMasterExceptions
    {
        public class UserNotFoundException : Exception
        {
            public UserNotFoundException() : base("User not found or May be Deleted") { }

            public UserNotFoundException(string message) : base(message) { }

            public UserNotFoundException(string message, Exception innerException) 
                : base(message, innerException) { }
        }
        
        // public class PasswordsUnmatchingExceptions : Exception
        // {
        //     public PasswordsUnmatchingExceptions() : base("Empty Password or Passwords don't match. Pls Match the Password Field with the Verify Password Field") { }
        //
        //     public PasswordsUnmatchingExceptions(string message) : base(message) { }
        //
        //     public PasswordsUnmatchingExceptions(string message, Exception innerException) 
        //         : base(message, innerException) { }
        // }
    }
}