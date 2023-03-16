using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.EntityFrameworkCore;
using Rocky_DataAccess.Data;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Utility;

namespace Rocky.Controllers
{
    [Authorize(Roles = WebConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepo;
        private readonly IWebHostEnvironment _websHostEnvironment;

        public ProductController(IProductRepository prodRepo, IWebHostEnvironment websHostEnvironment)
        {
            _prodRepo = prodRepo;
            _websHostEnvironment = websHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _prodRepo.GetAll(includeProperties: "Category,ApplicationType");
            return View(objList);

            //foreach(var obj in objList)
            //{
            //    obj.Category = _db.Category.FirstOrDefault(u => u.Id == obj.CategoryId);
            //    obj.ApplicationType = _db.ApplicationType.FirstOrDefault(u => u.Id == obj.ApplicationTypeId);
            //};
        }

        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {
            // IEnumerable <SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            //ViewBag.CategoryDropDown = CategoryDropDown;
            //ViewData["CategoryDropDown"] = CategoryDropDown;

            //Product product = new Product();

            ProductVM productVm = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetAllDropdownList(WebConstants.CategoryName),
                ApplicationTypeSelectList = _prodRepo.GetAllDropdownList(WebConstants.ApplicationTypeName)
            };

            if (id == null)
            {
                return View(productVm);
            }

            else
            {
                productVm.Product = _prodRepo.Find(id.GetValueOrDefault());
                if (productVm.Product == null)
                {
                    return NotFound();
                }

                return View(productVm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVm)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _websHostEnvironment.WebRootPath;

                string upload = webRootPath + WebConstants.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                if (productVm.Product.Id == 0)
                {
                    //creating
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVm.Product.Image = fileName + extension;

                    _prodRepo.Add(productVm.Product);
                }
                else
                {
                    //updating
                    var objFromDb = _prodRepo.FirstOrDefault(u => u.Id == productVm.Product.Id);

                    if (files.Count > 0)
                    {
                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream =
                               new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVm.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVm.Product.Image = objFromDb.Image;
                    }
                    _prodRepo.Update(productVm.Product);
                }
                _prodRepo.Save();
                return RedirectToAction("Index");
            }
            productVm.CategorySelectList = _prodRepo.GetAllDropdownList(WebConstants.CategoryName);
            productVm.ApplicationTypeSelectList = _prodRepo.GetAllDropdownList(WebConstants.ApplicationTypeName);
            return View(productVm);
        }

        //Get Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product product = _prodRepo.FirstOrDefault(u => u.Id == id, includeProperties: "Category,ApplicationType");

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //Post Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            string upload = _websHostEnvironment.WebRootPath + WebConstants.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _prodRepo.Remove(obj);
            _prodRepo.Save();
            TempData[WebConstants.Success] = "Action completed successfully";
            return RedirectToAction("Index");
        }
    }
}