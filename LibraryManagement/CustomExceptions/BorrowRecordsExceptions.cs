using System;
namespace LibraryManagement.CustomExceptions
{
    public class BorrowRecordsExceptions
    {
        public class BorrowRecordNotFoundExceptions : Exception
        {
            public BorrowRecordNotFoundExceptions() : base("Borrow Record Not Found") { }

            public BorrowRecordNotFoundExceptions(string message) : base(message) { }

            public BorrowRecordNotFoundExceptions(string message, Exception innerException) 
                : base(message, innerException) { }
        }
        
        public class BorrowRecordDetailsNotFoundExceptions : Exception
        {
            public BorrowRecordDetailsNotFoundExceptions() : base("Borrow Record Not Found") { }

            public BorrowRecordDetailsNotFoundExceptions(string message) : base(message) { }

            public BorrowRecordDetailsNotFoundExceptions(string message, Exception innerException) 
                : base(message, innerException) { }
        }
        
        public class BorrowConfigurationsNotFoundExceptions : Exception
        {
            public BorrowConfigurationsNotFoundExceptions() : base("Borrow Record Not Found") { }

            public BorrowConfigurationsNotFoundExceptions(string message) : base(message) { }

            public BorrowConfigurationsNotFoundExceptions(string message, Exception innerException) 
                : base(message, innerException) { }
        }
        
        public class BorrowConfigurationsValueNotSetProperly : Exception
        {
            public BorrowConfigurationsValueNotSetProperly() : base("Admin Has to Set Configuration Value Properly") { }

            public BorrowConfigurationsValueNotSetProperly(string message) : base(message) { }

            public BorrowConfigurationsValueNotSetProperly(string message, Exception innerException) 
                : base(message, innerException) { }
        }
    }
}