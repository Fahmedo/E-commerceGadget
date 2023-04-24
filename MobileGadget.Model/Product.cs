using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MobileGadget.Model
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Required]
        public string About { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        public int Price { get; set; }


        [Required]
        [Display(Name = "Battery Capacity")]
        [Range(1, 100, ErrorMessage = "Battery capacity must be in the range of 1-100!!")]
        public int BatteryCapacity { get; set; }
        [Required]
        public string Storage { get; set; }
        public string Microprocessor { get; set; }
        [Display(Name = "Hard Drive")]
        public string HardDrive { get; set; }
        [Required]
        [Display(Name = "Product Model")]
        public string ProductModel { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string Condition { get; set; }


        [Required]
        [Display(Name = "Phone Type")]
        public int PhoneId { get; set; }
        [ForeignKey("PhoneId")]
        [ValidateNever]
        public Phone Phone { get; set; }


        [Required]
        [Display(Name = "Laptop Type")]
        public int LaptopId { get; set; }
        [ForeignKey("LaptopId")]
        [ValidateNever]
        public Laptop Laptop { get; set; }




    }
}
