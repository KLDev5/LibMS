using System;
namespace LibraryManagement.CustomExceptions
{
    public class FileOperationsExceptions
    {
        public class FileNotUploadedException : Exception
        {
            public FileNotUploadedException() : base("File not found or Corrupt") { }

            public FileNotUploadedException(string message) : base(message) { }

            public FileNotUploadedException(string message, Exception innerException) 
                : base(message, innerException) { }
        }
    }
}