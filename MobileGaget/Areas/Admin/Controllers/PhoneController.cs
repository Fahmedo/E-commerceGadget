using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.Model;
using MobileGadget.Model.ViewModels;
using MobileGadget.Model.ViewModels;
using MobileGaget.Utility;

namespace MobileGadgetWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class PhoneController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;
    public PhoneController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {

        return View();
    }

    //Get (Get what to create)
    public IActionResult Upsert(int? id)
    {
        PhoneVM PhoneVM = new()
        {
            Phone = new()
        };
        if (id == null || id == 0)
        {
            return View(PhoneVM);
        }
        else
        {
            //Update Product
            PhoneVM.Phone = _unitOfWork.Phone.GetFirstOrDefault(u => u.Id == id);
            return View(PhoneVM);
        }
        //return View();
    }
    //POST (Create it)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Phone phone, PhoneVM obj, IFormFile? file)
    {
        if (phone.OperatingSystem == phone.DisplayOrder.ToString())
        {
            ModelState.AddModelError("OperatingSystem", "The Display Order exactly match the OPeratingSystem");
        }
        if (ModelState.IsValid)
        {


            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (file != null)
            {

                //creating file name
                string fileName = Guid.NewGuid().ToString();

                //uploading the file:the location the file go when uploaded
                var uploads = Path.Combine(wwwRootPath, @"Images\Phones");

                //file exension

                var extension = Path.GetExtension(file.FileName);

                //working on update
                //checking if file exist in the file , if exist we delete the file
                if (obj.Phone.ImageUrl != null)
                {
                    //getting the image old path
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Phone.ImageUrl.TrimStart('\\'));
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
                obj.Phone.ImageUrl = @"\Images\Phones\" + fileName + extension;
            }
            if (obj.Phone.Id == 0)
            {
                _unitOfWork.Phone.Add(obj.Phone);
            }
            else
            {
                _unitOfWork.Phone.Update(obj.Phone);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Create successful";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    public IActionResult Details()
    {
        IEnumerable<Phone> phoneList = _unitOfWork.Phone.GetAll();
        return View(phoneList);
    }



    #region API CALLs
    [HttpGet]
    //To get what you want to delete
    public IActionResult GetAll()
    {
        var PhoneList = _unitOfWork.Phone.GetAll();
        return Json(new { data = PhoneList });
    }
    //Post 

    [HttpDelete]
    //To delete it
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Phone.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleteing" });
        }
        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        _unitOfWork.Phone.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete successful" });
    }
}
#endregion

