//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaconAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class history
    {
        [Required]
        [MinLength(16)]
        [MaxLength(16)]
        [RegularExpression("[a-f0-9]+")]
        public string id { get; set; }

        [Required]
        [MinLength(16)]
        [MaxLength(16)]
        [RegularExpression("[a-f0-9]+")]
        public string bank_id { get; set; }

        [Required(ErrorMessage = "This field is important.")]
        [MinLength(3, ErrorMessage = "Must be more than 2 alphanumeric characters.")]
        [MaxLength(35, ErrorMessage = "Must be less than 36 alphanumeric characters.")]
        [RegularExpression("[a-zA-Z0-9\\s]+", ErrorMessage = "Special characters are not allowed.")]
        public string hist_label { get; set; }

        [Required]
        [MaxLength(10)]
        [RegularExpression("[a-zA-Z0-9\\s]+")]
        public string hist_type { get; set; }

        [MaxLength(255, ErrorMessage = "Must be less than 256 characters.")]
        public string reason { get; set; }

        public Nullable<double> initialAmount { get; set; }
        public Nullable<double> finalAmount { get; set; }
        public Nullable<System.DateTime> createdAt { get; set; }
        public Nullable<bool> isArchived { get; set; }
        
    }
}