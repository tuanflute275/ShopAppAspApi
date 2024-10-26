using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;
using X.PagedList;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("api/productImage")]
    public class ProductImageController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductImageController(ApplicationDbContext context)
        { 
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> FindAll()
        {
            var productImages = await _context.ProductImages.ToListAsync();
            return Ok(new ResponseObject(200, "Query data successfully", productImages));
        }
        [HttpGet("{productId}")]
        public async Task<ActionResult> FindByProductId(int productId, int page = 1)
        {
            var productImages = await _context.ProductImages
                .Where(x => x.ProductId == productId)
                .ToListAsync();
            if (productImages == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with productId {productId}", null));
            }
            if (productImages.Count > 0)
            {
                int totalRecords = productImages.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = productImages.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", productImages));
        }

        [HttpGet("image/{id}")]
        public async Task<ActionResult<ProductImage>> FindByImageId(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", productImage));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Save([FromForm] ProductImageModel model)
        {
            try
            {
                int count = 0;
                foreach (var imageFile in model.ImageFiles)
                {
                    if (imageFile.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            return BadRequest(new ResponseObject(400, "Invalid image format", null));
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "productImages");
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        var productImage = new ProductImage
                        {
                            ProductId = model.ProductId,
                            CreateDate = DateTime.Now,
                            UpdateDate = null,
                            Path = $"http://{HttpContext.Request.Host.Value}/uploads/productImages/{uniqueFileName}"
                        };

                        await _context.ProductImages.AddAsync(productImage);
                    }
                    count++;
                }

                await _context.SaveChangesAsync();
                return Ok(new ResponseObjectTotal(200, "Insert data successfully",count, model));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult> UpdateSingleImage([FromForm] ProductImageUpdateModel model)
        {
            try
            {
                // Lấy ảnh cũ theo imageId
                var productImage = await _context.ProductImages.FindAsync(model.ImageId);
                if (productImage == null)
                {
                    return NotFound(new ResponseObject(404, "Image not found", null));
                }

                // Nếu có ảnh mới được tải lên, tiến hành cập nhật ảnh
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Kiểm tra định dạng file hợp lệ
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        return BadRequest(new ResponseObject(400, "Invalid image format", null));
                    }

                    // Xóa ảnh cũ khỏi thư mục
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "productImages", Path.GetFileName(productImage.Path));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Tạo tên file mới và lưu ảnh mới vào thư mục
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "productImages");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn ảnh trong cơ sở dữ liệu
                    productImage.Path = $"http://{HttpContext.Request.Host.Value}/uploads/productImages/{uniqueFileName}";
                }

                // Cập nhật ngày thay đổi dù ảnh có thay đổi hay không
                productImage.UpdateDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(new ResponseObject(200, "Image updated successfully", productImage));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductImage>> Delete(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();
                return Ok(new ResponseObject(200, "Delete data successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }
    }
}
