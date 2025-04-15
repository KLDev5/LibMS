using System;

namespace LibraryManagement.ClsLib
{
    public class ClsMembership
    {
        public static string GenerateMembershipCode(string input)
        {
            try
            {
                char initial = input.ToUpper().Trim()[0];
                DateTime now = DateTime.UtcNow;
                long microseconds = now.Ticks / 10 % 1000000; // Extract microseconds

                string timestamp = now.ToString("yyyyMMddHHmmss") + microseconds.ToString("D6");
                
                string MembershipCode=initial.ToString() + timestamp;
                
                return MembershipCode;

            }
            catch (Exception e)
            {

                throw new Exception("Error generating membership code: " + e.Message);  
            }
            
        }
        
    }
}