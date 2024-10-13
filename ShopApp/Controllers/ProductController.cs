using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.DTO;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;
using X.PagedList;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("/api/product")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public ProductController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<Product>> FindAll(string? name, string? sort, int page = 1)
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            if (!string.IsNullOrEmpty(name))
            {
                products = await _context.Products.Where(x => x.ProductName.Contains(name)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductId).ToListAsync();
                        break;
                    case "Id-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductId).ToListAsync();
                        break;

                    case "Name-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductName).ToListAsync();
                        break;
                    case "Name-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductName).ToListAsync();
                        break;

                    case "Price-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductPrice).ToListAsync();
                        break;
                    case "Price-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductPrice).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductId).ToListAsync();
                        break;
                    case "Id-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductId).ToListAsync();
                        break;

                    case "Name-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductName).ToListAsync();
                        break;
                    case "Name-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductName).ToListAsync();
                        break;

                    case "Price-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductPrice).ToListAsync();
                        break;
                    case "Price-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductPrice).ToListAsync();
                        break;
                }
            }

            // convert to product dto
            var productDTOs = products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductImage = p.ProductImage,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                ProductPrice = p.ProductPrice,
                ProductSalePrice = p.ProductSalePrice,
                ProductStatus = p.ProductStatus,
                ProductDescription = p.ProductDescription,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                CategorySlug = p.Category.CategorySlug
            }).ToList();

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = productDTOs.ToPagedList(page, limit);
            return Ok(new ResponseObject(200, "Query data successfully", pageData));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> FindById(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }

            var productDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductImage = product.ProductImage,
                ProductName = product.ProductName,
                ProductSlug = product.ProductSlug,
                ProductPrice = product.ProductPrice,
                ProductSalePrice = product.ProductSalePrice,
                ProductStatus = product.ProductStatus,
                ProductDescription = product.ProductDescription,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                CategorySlug = product.Category.CategorySlug
            };
            return Ok(new ResponseObject(200, "Query data successfully", productDTO));
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<Product>> FindBySlug(string slug)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductSlug == slug);
            if (product == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with slug {slug}", null));
            }

            var productDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductImage = product.ProductImage,
                ProductName = product.ProductName,
                ProductSlug = product.ProductSlug,
                ProductPrice = product.ProductPrice,
                ProductSalePrice = product.ProductSalePrice,
                ProductStatus = product.ProductStatus,
                ProductDescription = product.ProductDescription,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                CategorySlug = product.Category.CategorySlug
            };
            return Ok(new ResponseObject(200, "Query data successfully", productDTO));
        }

        [HttpGet("category/{slug}")]
        public async Task<ActionResult<Product>> FindByCategory(string slug)
        {
            var products = await _context.Products.Include(p => p.Category).Where(p => p.Category.CategorySlug == slug).ToListAsync();
            var productDTOs = products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductImage = p.ProductImage,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                ProductPrice = p.ProductPrice,
                ProductSalePrice = p.ProductSalePrice,
                ProductStatus = p.ProductStatus,
                ProductDescription = p.ProductDescription,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                CategorySlug = p.Category.CategorySlug
            }).ToList();
            return Ok(new ResponseObject(200, "Query data successfully", productDTOs));
        }

        [HttpGet("new")]
        public async Task<ActionResult<Product>> FindAllDataNew(int limit = 3)
        {
            limit = limit <= 1 ? 1 : limit;
            var products = await _context.Products.Include(p => p.Category)
                .OrderByDescending(p => p.ProductId)
                .Take(limit)
                .ToListAsync();
            var productDTOs = products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductImage = p.ProductImage,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                ProductPrice = p.ProductPrice,
                ProductSalePrice = p.ProductSalePrice,
                ProductStatus = p.ProductStatus,
                ProductDescription = p.ProductDescription,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                CategorySlug = p.Category.CategorySlug
            }).ToList();
            return Ok(new ResponseObject(200, "Query data successfully", productDTOs));
        }


        [HttpGet("sale")]
        public async Task<ActionResult<Product>> FindAllDataSale(int limit = 3)
        {
            limit = limit <= 1 ? 1 : limit;
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductSalePrice > 0)
                .Take(limit)
                .ToListAsync();
            var productDTOs = products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductImage = p.ProductImage,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                ProductPrice = p.ProductPrice,
                ProductSalePrice = p.ProductSalePrice,
                ProductStatus = p.ProductStatus,
                ProductDescription = p.ProductDescription,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                CategorySlug = p.Category.CategorySlug
            }).ToList();
            return Ok(new ResponseObject(200, "Query data successfully", productDTOs));
        }

        [HttpGet("related/{slug}")]
        public async Task<ActionResult<Product>> FindAllDataRelated(string slug, int limit = 3)
        {
            limit = limit <= 1 ? 1 : limit;
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductSlug != slug)
                .Take(limit)
                .ToListAsync();
            var productDTOs = products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductImage = p.ProductImage,
                ProductName = p.ProductName,
                ProductSlug = p.ProductSlug,
                ProductPrice = p.ProductPrice,
                ProductSalePrice = p.ProductSalePrice,
                ProductStatus = p.ProductStatus,
                ProductDescription = p.ProductDescription,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                CategorySlug = p.Category.CategorySlug
            }).ToList();
            return Ok(new ResponseObject(200, "Query data successfully", productDTOs));
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Save([FromForm] ProductModel model)
        {
            try
            {
                Product product = new Product();
                List<Product> foundData = await _context.Products.Where(x => x.ProductName == model.ProductName).ToListAsync();
                if (foundData.Count > 0)
                {
                    return BadRequest(new ResponseObject(400, "Product name already taken"));
                }
                else
                {
                    if (model.ImageFile == null)
                    {
                        return BadRequest(new ResponseObject(400, "Image Is Required", null));
                    }

                    if (model.ImageFile.Length > 0)
                    {
                        // Đường dẫn lưu file
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                        var filePath = Path.Combine(uploadsFolder, model.ImageFile.FileName);

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Lưu file hình ảnh
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        // Cập nhật đường dẫn hình ảnh vào thuộc tính sản phẩm
                        product.ProductImage = $"http://{HttpContext.Request.Host.Value}/uploads/products/{model.ImageFile.FileName}";
                    }


                    product.ProductName = model.ProductName;
                    product.ProductSlug = Util.GenerateSlug(model.ProductName);
                    product.ProductPrice = model.ProductPrice;
                    product.ProductSalePrice = model.ProductSalePrice;
                    product.ProductStatus = model.ProductStatus;
                    product.CategoryId = model.CategoryId;
                    product.ProductDescription = model.ProductDescription;
                    await _context.Products.AddAsync(product);
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Insert data successfully", product));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject(404, "Insert data failed", ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Update(int id, [FromForm] ProductModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                try
                {
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        // Đường dẫn lưu hình ảnh mới
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                        var newFilePath = Path.Combine(uploadsFolder, model.ImageFile.FileName);

                        // Xóa hình ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(model.OldImage))
                        {
                            var oldFileName = model.OldImage.Split($"{HttpContext.Request.Host.Value}/uploads/products/").LastOrDefault();
                            var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Lưu file hình ảnh mới
                        using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        product.ProductImage = $"http://{HttpContext.Request.Host.Value}/uploads/products/{model.ImageFile.FileName}";
                    }
                    else
                    {
                        // Nếu không có hình ảnh mới, giữ nguyên hình ảnh cũ
                        product.ProductImage = model.OldImage;
                    }
                    product.ProductName = model.ProductName;
                    product.ProductSlug = Util.GenerateSlug(model.ProductName);
                    product.ProductPrice = model.ProductPrice;
                    product.ProductSalePrice = model.ProductSalePrice;
                    product.ProductStatus = model.ProductStatus;
                    product.CategoryId = model.CategoryId;
                    product.ProductDescription = model.ProductDescription;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", product));
                }
                catch (Exception ex)
                {
                    return BadRequest(new ResponseObject(404, "Update data failed"));
                }
            }
            else
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Delete data successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject(404, "Insert data failed"));
            }
        }
    }
}
