using System;
namespace LibraryManagement.CustomExceptions
{
    public class DatabaseExceptions
    {
        public class DatabaseOperationException : Exception
        {
            public DatabaseOperationException(string message, Exception innerException)
                : base(message, innerException) { }
            
            public DatabaseOperationException() : base("No Data is Found") { }

            public DatabaseOperationException(string message) : base(message) { }
            
        }
    }
}