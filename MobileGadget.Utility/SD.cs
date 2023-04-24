using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MobileGaget.Utility
{
    public static class SD
    {
        public const string Role_User_Technician = "Technician";
        public const string Role_User_Indi = "Individual";
        
        public const string Role_Admin = "Admin";
        //  public const string Role_Empolyee = "Technician";


        // Details for Order Status  


        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";


        // Details fpr Payment Status

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";


        // Session
        public const string SessionCart = "SessionShoppingCart";
    }



}
