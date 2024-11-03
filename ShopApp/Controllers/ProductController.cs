using Microsoft.AspNetCore.Authorization;
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

            if (products.Count > 0)
            {
                int totalRecords = products.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = products.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", products));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> FindById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductComments)
                .Include(x => x.ProductDetails)
                .FirstOrDefaultAsync(p => p.ProductId == id);
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
                CategorySlug = product.Category.CategorySlug,
                ProductImages = product.ProductImages.Select(i => i.Path).ToList(),
                ProductComments = product.ProductComments.Select(c => new ProductCommentDTO
                {
                    UserId = c.UserId,
                    Email = c.Email,
                    Message = c.Message,
                    Name = c.Name
                }).ToList(),
                ProductDetails = product.ProductDetails.Select(d => new ProductDetailDTO
                {
                    Color = d.Color,
                    Size = d.Size,
                    Quantity = d.Quantity,
                }).ToList()
            };
            return Ok(new ResponseObject(200, "Query data successfully", productDTO));
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<Product>> FindBySlug(string slug)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductComments)
                .Include(x => x.ProductDetails)
                .FirstOrDefaultAsync(p => p.ProductSlug == slug);
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
                CategorySlug = product.Category.CategorySlug,
                ProductImages = product.ProductImages.Select(i => i.Path).ToList(),
                ProductComments = product.ProductComments.Select(c => new ProductCommentDTO
                {
                    UserId = c.UserId,
                    Email = c.Email,
                    Message = c.Message,
                    Name = c.Name
                }).ToList(),
                ProductDetails = product.ProductDetails.Select(d => new ProductDetailDTO
                {
                    Color = d.Color,
                    Size = d.Size,
                    Quantity = d.Quantity,
                }).ToList()
            };
            return Ok(new ResponseObject(200, "Query data successfully", productDTO));
        }

        [HttpGet("category/{slug}")]
        public async Task<ActionResult<Product>> FindByCategory(string slug, int page = 1)
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
            if (productDTOs.Count > 0)
            {
                int totalRecords = productDTOs.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = productDTOs.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
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

        [HttpGet("related/{id}")]
        public async Task<ActionResult<Product>> FindAllDataRelated(int id, int limit = 3)
        {
            limit = limit <= 1 ? 1 : limit;
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId != id)
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

        [Authorize(Roles = "Admin")]
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
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [Authorize(Roles = "Admin")]
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
                    return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
                }
            }
            else
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> Delete(int id)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
            var product = await _context.Products.FindAsync(id);
            var productCmts = await _context.ProductComments.Where(x => x.ProductId == id).ToListAsync();
            var productDetails = await _context.ProductDetails.Where(x => x.ProductId == id).ToListAsync();
            var productImages = await _context.ProductImages.Where(x => x.ProductId == id).ToListAsync();
            var orderDetails = await _context.OrderDetails.Where(x => x.ProductId == id).ToListAsync();
            var wishlists = await _context.Wishlists.Where(x => x.ProductId == id).ToListAsync();
            var carts = await _context.Carts.Where(x => x.ProductId == id).ToListAsync();
            if (product == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            if (orderDetails != null && orderDetails.Count > 0) 
            {
                _context.OrderDetails.RemoveRange(orderDetails);
                await _context.SaveChangesAsync();
            }
            if (productCmts != null && productCmts.Count > 0)
            {
                _context.ProductComments.RemoveRange(productCmts);
                await _context.SaveChangesAsync();
            }
            if (productDetails != null && productDetails.Count > 0)
            {
                _context.ProductDetails.RemoveRange(productDetails);
                await _context.SaveChangesAsync();
            }
            if (wishlists != null && wishlists.Count > 0)
            {
                _context.Wishlists.RemoveRange(wishlists);
                await _context.SaveChangesAsync();
            }
            if (carts != null && carts.Count > 0)
            {
                _context.Carts.RemoveRange(carts);
                await _context.SaveChangesAsync();
            }
            if (productImages != null && productImages.Count > 0)
            {
                foreach (var item in productImages)
                {
                    var oldFileName = item.Path;
                    var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                _context.ProductImages.RemoveRange(productImages);
                await _context.SaveChangesAsync();
            }
            try
            {
                var oldFileName = product.ProductImage;
                var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                _context.Products.Remove(product);
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
