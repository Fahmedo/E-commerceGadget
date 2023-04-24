using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.Model;
using MobileGaget.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.Model;
using MobileGaget.Models;
using System.Diagnostics;
using System.Security.Claims;
using MobileGadget.DataAccess.Repository;
using MobileGadget.Model;
using MobileGadget.Model;
using MobileGadget.DataAccess.Repository.IRepository;
using MobileGaget.Model.ViewModels;
using MobileGadget.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using MobileGaget.Utility;
//using Stripe;

namespace MobileGadget.Areas.Customer.Controllers;
[Area("Customer")]
//Controller for Adding ShopppingCart items(Product)
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        //To get all the product as model in this controller and include properties of category and coverType on index page

        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Phone,Laptop");
        return View(productList);
    }
    [HttpGet]
    public  async Task<IActionResult> Index(string SearchString)
    {
        ViewData["ProductFilter"] = SearchString;
        var ProductSearch = from u in _dbContext.products
                            select u;
        if (!string.IsNullOrEmpty(SearchString))
        {
            ProductSearch = ProductSearch.Where(b => b.ProductName.Contains(SearchString));
        }
        return View(await ProductSearch.AsNoTracking().ToListAsync());
    }

    //[HttpGet]
    //public async Task<IActionResult> Index(string SerachString)
    //{
    //    ViewData["ProductFilter"] = SerachString;
    //    var productSearch = from u in _dbContext.products
    //                        select u;
    //    if (!String.IsNullOrEmpty(SerachString))
    //    {
    //        productSearch = productSearch.Where(b => b.ProductName.Contains(SerachString));
    //    }
    //    return View(await productSearch.AsNoTracking().ToListAsync());
    //}

    public IActionResult Details(int productId)
    {
        ShoppingCart cartObj = new()
        {
            Count = 1,
            ProductId = productId,
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Phone,Laptop")
        };
        return View(cartObj);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    //only authorize user(sign in user can access this postActionMethod)
    [Authorize]
    //Add the shoppingCart items together with Application UserId 
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserId = claim.Value;

        //Adding shoppingCart to the existing shoppingCart the use added earlier
        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

        if (cartFromDb == null)
        {
            _unitOfWork.ShoppingCart.Add(shoppingCart);
            _unitOfWork.Save();
            //Adding session to the shoppingCart
            HttpContext.Session.SetInt32(SD.SessionCart,
               _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
        }
        else
        {
            _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
        }
      
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


