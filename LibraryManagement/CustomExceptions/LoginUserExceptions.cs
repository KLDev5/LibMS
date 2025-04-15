using System;

namespace LibraryManagement.CustomExceptions
{
    public class LoginUserExceptions
    {
        
        public class UserEmailAlreadyExistsException : Exception
        {
            public UserEmailAlreadyExistsException() : base("Email Already Used") { }

            public UserEmailAlreadyExistsException(string message) : base(message) { }

            public UserEmailAlreadyExistsException(string message, Exception innerException) 
                : base(message, innerException) { }
        }
        
        public class PasswordsUnmatchingExceptions : Exception
        {
            public PasswordsUnmatchingExceptions() : base("Empty Password or Passwords don't match. Pls Match the Password Field with the Verify Password Field") { }

            public PasswordsUnmatchingExceptions(string message) : base(message) { }

            public PasswordsUnmatchingExceptions(string message, Exception innerException) 
                : base(message, innerException) { }
        }
        
        public class InvalidFieldsException : Exception
        {
            public InvalidFieldsException() : base("Some Fields are Invalid. Try Again") { }

            public InvalidFieldsException(string message) : base(message) { }

            public InvalidFieldsException(string message, Exception innerException) 
                : base(message, innerException) { }
        }

        public class InvalidEmailException : Exception
        {
            public InvalidEmailException() : base("Invalid Email. Please Sign in with Proper Email or Sign Up") { }   
            public InvalidEmailException(string message) : base(message) { }
            public InvalidEmailException(string message, Exception innerException)   
                : base(message, innerException) { }
        }
        
        public class InvalidPasswordsException: Exception
        {
            public  InvalidPasswordsException() : base("Invalid Passwords") { }
            public InvalidPasswordsException(string message) : base(message) { }
            public  InvalidPasswordsException(string message, Exception innerException)
                : base(message, innerException) { }
            
        }
        
    }
}