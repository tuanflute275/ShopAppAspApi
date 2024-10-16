using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;

namespace ShopApp.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<ActionResult> FindByProductId(int productId)
        {
            var productImages = await _context.ProductImages.Where(x => x.ProductId == productId).ToListAsync();
            if (productImages == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with productId {productId}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", productImages));
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Save([FromForm] ProductImageModel model)
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

                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
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
                            Path = $"http://{HttpContext.Request.Host.Value}/uploads/products/{uniqueFileName}"
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
