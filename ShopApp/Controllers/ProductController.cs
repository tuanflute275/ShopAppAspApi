using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.DTO;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("/api/product")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
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

        [HttpPost]
        public async Task<ActionResult<Product>> Save(ProductModel model)
        {
            try
            {
                List<Product> foundData = await _context.Products.Where(x => x.ProductName == model.ProductName).ToListAsync();
                if (foundData.Count > 0)
                {
                    return BadRequest(new ResponseObject(400, "Product name already taken"));
                }
                else
                {
                    Product product = new Product
                    {
                        ProductName = model.ProductName,
                        ProductSlug = Util.GenerateSlug(model.ProductName),
                        ProductImage = model.ProductImage,
                        ProductPrice = model.ProductPrice,
                        ProductSalePrice = model.ProductSalePrice,
                        ProductStatus = model.ProductStatus,
                        CategoryId = model.CategoryId,
                        ProductDescription = model.ProductDescription,
                    };
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
        public async Task<ActionResult<Product>> Update(int id, ProductModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                try
                {
                    product.ProductName = model.ProductName;
                    product.ProductSlug = Util.GenerateSlug(model.ProductName);
                    product.ProductImage = model.ProductImage;
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
