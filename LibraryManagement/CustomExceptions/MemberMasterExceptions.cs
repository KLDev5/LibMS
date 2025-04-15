using System;

namespace LibraryManagement.CustomExceptions
{
    public class MemberMasterExceptions
    {
        public class MemberNotFoundException : Exception
        {
            public MemberNotFoundException() : base("Member not found") { }

            public MemberNotFoundException(string message) : base(message) { }

            public MemberNotFoundException(string message, Exception innerException) 
                : base(message, innerException) { }
        }
    }
}