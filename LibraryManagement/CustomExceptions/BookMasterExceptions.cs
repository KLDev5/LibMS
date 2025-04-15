using System;
namespace LibraryManagement.CustomExceptions
{
    public class BookMasterExceptions
    {
        public class BookNotFoundExceptions : Exception
        {
            public BookNotFoundExceptions() : base("Book not found") { }

            public BookNotFoundExceptions(string message) : base(message) { }

            public BookNotFoundExceptions(string message, Exception innerException) 
                : base(message, innerException) { }
        }
    }
}