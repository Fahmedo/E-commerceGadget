using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.Model;
using MobileGadget.Model.ViewModels;
using MobileGaget.Models;
using MobileGaget.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.Model.ViewModels;
using MobileGaget.Model;
using MobileGaget.Utility;
using Stripe.Checkout;
using System.Security.Claims;
using MobileGadget.Model;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MobileGadgetWeb.Areas.Admin.Controllers;


[Area("Admin")]
//only signed in user can access the Package
[Authorize]
//Controller for retreving shoppingCart  item that has already been added
public class BuyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    // To bind ShoppingCartVM and ShoppingCartVM when we post form(so we dont have to write in post method)
    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }
    public int OrderTotal { get; set; }
    private readonly IEmailSender _emailSender;

    public BuyController(IUnitOfWork unitOfWork, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
    }
    //Add the shoppingCart items together with Application UserId in Index 
    public IActionResult Indexs()
    {
        var claimIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartVM = new ShoppingCartVM()
        {
            BuyList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),

            // Creating new instance for OrderHeader since we are using it insdie the ShoppingCartVM
            OrderHeader = new()
        };
        foreach (var cart in ShoppingCartVM.BuyList)
        {
            //get the price cart based on count
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price);
            //taking the total cart price
            ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }


        return View(ShoppingCartVM);

    }
    //Get method for summary page
    //Add the shoppingCart items together with Application UserId in Summary
    public IActionResult Summary()
    {
        var claimIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

        ShoppingCartVM = new ShoppingCartVM()
        {
            BuyList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
            // Creating new instance for OrderHeader since we are using it insdie the ShoppingCartVM
            OrderHeader = new()

        };
        //Getting the ApplicationUser From OrderHeader
        ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
               u => u.Id == claim.Value);

        //Assigning the ApplicationUser  to OrderHeader Properties


        ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
        ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


        foreach (var cart in ShoppingCartVM.BuyList)
        {
            //get the price cart based on count
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price);
            //taking the total cart price
            ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }
        return View("Summary");

    }

    //Post method for summary page
    //Add the shoppingCart items together with Application UserId in Summary
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Summary")]
    public IActionResult SummaryPOST(ShoppingCartVM obj)
    {
        var claimIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);


        ShoppingCartVM.BuyList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");

        // Populating the Details need  for Order Header
        //ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        //ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
        ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

        foreach (var cart in ShoppingCartVM.BuyList)
        {
            //get the price cart based on count
            cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price);
            //taking the total cart price
            ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }



        //Check if the it's companyUser and if it's companyUser implment the companyUser flow
        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

        if (applicationUser.TechnicianId.GetValueOrDefault() == 0)
        {
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
        }
        else
        {
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
        }
        //Adding the pouplated details need  for Order Header to the DB
       
        _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        _unitOfWork.Save();
       





        //  Populating the Details need  for Order Details
        foreach (var cart in ShoppingCartVM.BuyList)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = cart.ProductId,
                OrderId = ShoppingCartVM.OrderHeader.Id,
                Price = cart.Price,
                Count = cart.Count,
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
           
        }

        //Check if the it's companyUser and if it's companyUser implment the cstripe setting
        if (applicationUser.TechnicianId.GetValueOrDefault() == 0)
        {
            ///Stripe API setting

            var domain = "https://localhost:44301/";
            var options = new SessionCreateOptions
            {
                //Add new session for card payment and LineItems
                PaymentMethodTypes = new List<string>
            {
                "card",
            },
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"admin/buy/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/buy/index",
            };
            foreach (var item in ShoppingCartVM.BuyList)
            {

                //LineItems option represent all of the item we have in our shoppingcart
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ProductName
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        else
        {
            return RedirectToAction("OrderConfirmation", "buy", new { id = ShoppingCartVM.OrderHeader.Id });
        }

        //Removing ShoppingCartVM.ListCart
        //_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
        //_unitOfWork.Save();
        //return RedirectToAction("Index", "Home");
    }
    public IActionResult OrderConfirmations(int id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");
        if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
        {
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            //check the stripe payment status
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentID(id, orderHeader.SessionId, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
        }
        //Sending email after order has been made
        _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book", "<p>New Order Created</p>");
        //Removing ShoppingCartVM.ListCart
        List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId
        == orderHeader.ApplicationUserId).ToList();
        //To clear the cart after placing order
        HttpContext.Session.Clear();
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.Save();
        return View(id);
    }



    //Action method for increment when we click + button
    public IActionResult Plus(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    //Action method for decrement when we click - button
    public IActionResult Minus(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        if (cart.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Remove(cart);

            //Remove session
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
            HttpContext.Session.SetInt32(SD.SessionCart, count);
        }
        else
        {
            _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
        }

        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }


    //Action method for delete when we click delete button
    public IActionResult Remove(int cartId)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
        _unitOfWork.ShoppingCart.Remove(cart);


        //Remove Session
        var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
        HttpContext.Session.SetInt32(SD.SessionCart, count);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    //method for getting the cart price based on count
    private int GetPriceBasedOnQuantity(int quantity, int price)
    {
        if (quantity >= 1)
        {
            //return price;
        }
        return price;
        //else
        //{
        //    if (quantity <= 100)
        //    {
        //        return price50;
        //    }
        //    return price100;
        //}
    }
}
