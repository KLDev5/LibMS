using System;

namespace LibraryManagement.CustomExceptions
{
    public class CustomCommonExceptions
    {
        public class BadRequestExceptions : Exception
        {
            public BadRequestExceptions() : base("Bad Request") { }

            public BadRequestExceptions(string message) : base(message) { }

            public BadRequestExceptions(string message, Exception innerException) 
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