using Microsoft.AspNetCore.Mvc;
using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.Model;
using MobileGadget.Model.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using MobileGaget.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using MobileGaget.Utility;

namespace MobileGadgetWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]

public class TechnicianController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;
    public TechnicianController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
        TechnicianVM technicianVM = new()
        {
            Technician = new()
        };
        if (id == null || id == 0)
        {
            return View(technicianVM);
        }
        else
        {
            //Update Product
            technicianVM.Technician = _unitOfWork.Technician.GetFirstOrDefault(u => u.Id == id);
            return View(technicianVM);
        }
        //return View();
    }
    //POST (Create it)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(TechnicianVM obj, IFormFile? file)
    {
        if (ModelState.IsValid)
        {


            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (file != null)
            {

                //creating file name
                string fileName = Guid.NewGuid().ToString();

                //uploading the file:the location the file go when uploaded
                var uploads = Path.Combine(wwwRootPath, @"Images\Technician");

                //file exension

                var extension = Path.GetExtension(file.FileName);

                //working on update
                //checking if file exist in the file , if exist we delete the file
                if (obj.Technician.ImageUrl != null)
                {
                    //getting the image old path
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Technician.ImageUrl.TrimStart('\\'));
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
                obj.Technician.ImageUrl = @"\Images\Technician\" + fileName + extension;
            }
            if (obj.Technician.Id == 0)
            {
                _unitOfWork.Technician.Add(obj.Technician);
            }
            else
            {
                _unitOfWork.Technician.Update(obj.Technician);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Created successful";
            return RedirectToAction("Index");
        }
        return View(obj);

    }

    public IActionResult Details()
    {
        IEnumerable<Technician> TechList = _unitOfWork.Technician.GetAll();
        return View(TechList);
    }

    #region API CALLs
    [HttpGet]
    //To get what you want to delete
    public IActionResult GetAll()
    {
        var TechnicianList = _unitOfWork.Technician.GetAll();
        return Json(new { data = TechnicianList });
    }
    //Post 

    [HttpDelete]
    //To delete it
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Technician.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleteing" });
        }
        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        _unitOfWork.Technician.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete successful" });
    }
}
#endregion

