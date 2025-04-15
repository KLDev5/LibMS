
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace LibraryManagement.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<LibraryManagement.Models.LibraryDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}