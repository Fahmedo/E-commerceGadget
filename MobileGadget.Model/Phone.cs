using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MobileGadget.Model
{
    public class Phone
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Operating System")]
        public string OperatingSystem { get; set; }


        [Display(Name = "Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be in the range of 1-100!!")]
        public int DisplayOrder { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }



    }
}
