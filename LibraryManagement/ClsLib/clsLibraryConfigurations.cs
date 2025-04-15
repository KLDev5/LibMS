using System;
using LibraryManagement.CustomExceptions;
using LibraryManagement.Models;
using LibraryConfiguration = LibraryManagement.Migrations.LibraryConfiguration;

namespace LibraryManagement.ClsLib
{
    public class clsLibraryConfigurations
    {
        LibraryDBContext db=new LibraryDBContext(); 
        
        public Models.LibraryConfiguration getBorrowDurationConfiguration()
        {
            try
            {
                Models.LibraryConfiguration lconfig = db.LibraryConfigurations.Find(3);
                if (lconfig == null) throw new BorrowRecordsExceptions.BorrowConfigurationsNotFoundExceptions();
                return lconfig;
            }
            
            catch(BorrowRecordsExceptions.BorrowConfigurationsNotFoundExceptions ex)
            {
                throw new BorrowRecordsExceptions.BorrowConfigurationsNotFoundExceptions(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception();
                return null;
            }
           
        }
        
        public Models.LibraryConfiguration getDefaultOverdueDateConfiguration()
        {
            try
            {
                Models.LibraryConfiguration lconfig = db.LibraryConfigurations.Find(4);
                if (lconfig == null) throw new BorrowRecordsExceptions.BorrowConfigurationsNotFoundExceptions();
                return lconfig;
            }
            
            catch(BorrowRecordsExceptions.BorrowConfigurationsNotFoundExceptions ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
           
        }
    }
}