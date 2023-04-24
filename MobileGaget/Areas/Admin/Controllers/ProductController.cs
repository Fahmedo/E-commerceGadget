
using MobileGaget.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.DataAccess;
using MobileGadget.DataAccess.Repository;

using MobileGadget.Model;
using MobileGaget.Model.ViewModels;
using IServiceScope = Microsoft.Extensions.DependencyInjection.IServiceScope;
using Microsoft.AspNetCore.Authorization;
using MobileGaget.Utility;

namespace BulkyBookWeb.Areas.Admin.Controllers;
[Area("Admin")]
// Controller with one method for create and update(Upsert) and API for delete
// Use for CRUD Operation
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }
    public IActionResult Index()
    {
        return View();
    }
    //Code for Create(create) in this controller
    public IActionResult Upsert(int? id)
    {

        ProductVM productVM = new()
        {



            Product = new(),


            PhoneList = _unitOfWork.Phone.GetAll().Select(i => new SelectListItem
            {

                Text = i.OperatingSystem,
                Value = i.Id.ToString()
            }),
            LapTopList = _unitOfWork.Laptop.GetAll().Select(i => new SelectListItem
            {
                Text = i.OperatingSystem,
                Value = i.Id.ToString()
            }),
        };



        if (id == null || id == 0)
        {
            // Create Product
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CoverTypeList"] = CoverTypeList;
            return View(productVM);
        }

        else
        {
            //Update Product
            productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productVM);
        }

    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    //Code for Edit(Update) in this controller
    public IActionResult Upsert(ProductVM obj, IFormFile? file)
    {
        if (ModelState.IsValid)
        {


            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (file != null)
            {

                //creating file name
                string fileName = Guid.NewGuid().ToString();

                //uploading the file:the location the file go when uploaded
                var uploads = Path.Combine(wwwRootPath, @"Images\Products");

                //file exension

                var extension = Path.GetExtension(file.FileName);

                //working on update
                //checking if file exist in the file , if exist we delete the file
                if (obj.Product.ImageUrl != null)
                {
                    //getting the image old path
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // copy the file to fileStream location
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);

                }
                obj.Product.ImageUrl = @"\Images\Products\" + fileName + extension;
            }
            if (obj.Product.Id == 0)
            {
                _unitOfWork.Product.Add(obj.Product);
            }
            else
            {
                _unitOfWork.Product.Update(obj.Product);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Product created successfully";
            return RedirectToAction("Index");
        }
        return View(obj);

    }


    #region API CALLs
    [HttpGet]
    //To get what you want to delete
    public IActionResult GetAll()
    {
        var ProductList = _unitOfWork.Product.GetAll(includeProperties: "Phone,Laptop");
        return Json(new { data = ProductList });
    }
    //Post 

    [HttpDelete]
    //To delete it
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleteing" });
        }
        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete successful" });
    }
    #endregion

}

