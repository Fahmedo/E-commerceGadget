using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileGadget.Model
{
    public class Technician
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Company's Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }
}
