using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.DTO;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult> FindAll()
        {
            var carts = await _context.Carts.Include(x => x.User).Include(x => x.Product).ToListAsync();
            var listCartDTO = carts.Select(c => new CartDTO
            {
                CartId = c.CartId,
                Quantity = c.Quantity,
                TotalAmount = c.TotalAmount,
                UserId = c.User.Id,
                UserName = c.User.UserName,
                UserFullName = c.User.UserFullName,
                UserEmail = c.User.UserEmail,
                UserPhoneNumber = c.User.UserPhoneNumber,
                UserAddress = c.User.UserAddress,
                ProductId = c.ProductId,
                ProductName = c.Product.ProductName,
                ProductSlug = c.Product.ProductSlug,
                ProductImage = c.Product.ProductImage,
                ProductPrice = c.Product.ProductPrice > 0 ? c.Product.ProductPrice : c.Product.ProductSalePrice,
                ProductStatus = c.Product.ProductStatus,
            }).ToList();

            // count quantity and totalAmount
            TotalDTO total = new TotalDTO();
            foreach (var item in listCartDTO)
            {
                total.Quantity += item.Quantity;
                total.Amount += (double)item.TotalAmount;
            }
            return Ok(new ResponseObjectTotal(200, "Query data successfully", total, listCartDTO));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> FindById(int userId)
        {
            var carts = await _context.Carts.Include(x => x.User).Include(x => x.Product).Where(x => x.UserId == userId).ToListAsync();
            var listCartDTO = carts.Select(c => new CartDTO
            {
                CartId = c.CartId,
                Quantity = c.Quantity,
                TotalAmount = c.TotalAmount,
                UserId = c.User.Id,
                UserName = c.User.UserName,
                UserFullName = c.User.UserFullName,
                UserEmail = c.User.UserEmail,
                UserPhoneNumber = c.User.UserPhoneNumber,
                UserAddress = c.User.UserAddress,
                ProductId = c.ProductId,
                ProductName = c.Product.ProductName,
                ProductSlug = c.Product.ProductSlug,
                ProductImage = c.Product.ProductImage,
                ProductPrice = c.Product.ProductPrice > 0 ? c.Product.ProductPrice : c.Product.ProductSalePrice,
                ProductStatus = c.Product.ProductStatus,
            }).ToList();

            // count quantity and totalAmount
            TotalDTO total = new TotalDTO();
            foreach (var item in listCartDTO)
            {
                total.Quantity += item.Quantity;
                total.Amount += (double)item.TotalAmount;
            }
            return Ok(new ResponseObjectTotal(200, "Query data successfully", total, listCartDTO));
        }

        [HttpPost]
        public async Task<ActionResult<Cart>> Save(CartModel model)
        {
            try
            {
                var product = await _context.Products.FindAsync(model.ProductId);
                Cart cart = new Cart
                {
                    UserId = model.UserId,
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                    TotalAmount = model.Quantity * (product.ProductSalePrice > 0 ? product.ProductSalePrice : product.ProductPrice)
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
                // save memory

                return Ok(new ResponseObject(200, "Insert data successfully", model));
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPut("{id}/{quantity}")]
        public async Task<ActionResult<Cart>> Update(int id, int quantity)
        {
            var cart = await _context.Carts.FindAsync(id);
            var product = await _context.Products.FindAsync(cart.ProductId);
            if (cart != null)
            {
                try
                {
                    cart.Quantity = quantity;
                    cart.TotalAmount = quantity * (product.ProductSalePrice > 0 ? product.ProductSalePrice : product.ProductPrice);
                await _context.SaveChangesAsync();
                    return Ok(new ResponseObject(200, "Update data successfully"));
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<Cart>> Delete(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Carts.Remove(cart);
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
