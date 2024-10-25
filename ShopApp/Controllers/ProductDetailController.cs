using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("api/productDetail")]
    public class ProductDetailController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductDetailController(ApplicationDbContext context) 
        { 
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> FindAll()
        {
            var productDetails = await _context.ProductDetails.ToListAsync();
            return Ok(new ResponseObject(200, "Query data successfully", productDetails));
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult> FindByProductId(int productId)
        {
            var productDetails = await _context.ProductDetails.Where(x => x.ProductId == productId).ToListAsync();
            if (productDetails == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with productId {productId}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", productDetails));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Save(ProductDetailModel model)
        {
            try
            {
                ProductDetail productDetail = new ProductDetail
                {
                    Color = model.Color,
                    Size = model.Size,
                    Quantity = model.Quantity,
                    ProductId = model.ProductId,
                };
                await _context.ProductDetails.AddAsync(productDetail);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Insert data successfully", productDetail));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateProductDetailModel model)
        {
            var productDetail = await _context.ProductDetails.FindAsync(id);
            if (productDetail != null)
            {
                try
                {
                    productDetail.Color = model.Color;
                    productDetail.Size = model.Size;
                    productDetail.Quantity = model.Quantity;
                    productDetail.UpdateDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok(new ResponseObject(200, "Update data successfully", productDetail));
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

        [Authorize(Roles = "Admin, User")]
        [HttpPut("{id}/{quantity}")]
        public async Task<ActionResult> UpdateQuantity(int id, int quantity)
        {
            var productDetail = await _context.ProductDetails.FindAsync(id);
            if (productDetail != null)
            {
                try
                {
                    productDetail.Quantity = quantity;
                    productDetail.UpdateDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok(new ResponseObject(200, "Update data successfully", productDetail));
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
        public async Task<ActionResult<ProductDetail>> Delete(int id)
        {
            var productDetail = await _context.ProductDetails.FindAsync(id);
            if (productDetail == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.ProductDetails.Remove(productDetail);
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
