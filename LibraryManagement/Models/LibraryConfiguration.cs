using System;
using System.Data.Entity;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace LibraryManagement.Models
{
    public class LibraryConfiguration
    {       [Required]
            [Key]
            public long ConfigId { get; set; }                // Primary key, unique identifier
            public string ConfigName { get; set; }           // Name of the configuration (e.g., "Default Borrow Duration")
            public string ConfigValue { get; set; }          // The value of the configuration (e.g., "14" days)
            public string ConfigType { get; set; }           // Type of configuration (e.g., "Borrow", "Fee", "Schedule")
            public DateTime EffectiveDate { get; set; }      // The date when this configuration becomes effective
            public DateTime? ExpiryDate { get; set; }        // The date when this configuration expires (nullable)
            public bool IsActive { get; set; }               // Indicates if the configuration is active
            public DateTime CreatedAt { get; set; }          // Timestamp when the configuration was created
            public DateTime UpdatedAt { get; set; }          // Timestamp when the configuration was last updated
        
    }
}