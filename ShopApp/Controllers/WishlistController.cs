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
    [Route("api/wishlish")]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string? sort, int page = 1)
        {
            var wishlishs = await _context.Wishlists.Include(x => x.User).Include(x => x.Product).ToListAsync();
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        wishlishs = await _context.Wishlists.OrderBy(x => x.WishlistId).ToListAsync();
                        break;
                    case "Id-DESC":
                        wishlishs = await _context.Wishlists.OrderByDescending(x => x.WishlistId).ToListAsync();
                        break;

                    case "Date-ASC":
                        wishlishs = await _context.Wishlists.OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        wishlishs = await _context.Wishlists.OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            var listDTO = wishlishs.Select(c => new WishlishDTO
            {
                WishlistId = c.WishlistId,
                CreateDate = c.CreateDate,
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
            foreach (var item in listDTO)
            {
                total.Quantity ++;
            }

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = listDTO.ToPagedList(page, limit);
            return Ok(new ResponseObjectTotal(200, "Query data successfully",total, pageData));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> FindById(int userId)
        {
            var wishlishs = await _context.Wishlists.Include(x => x.User).Include(x => x.Product).Where(x => x.UserId == userId).ToListAsync();
            var listDTO = wishlishs.Select(c => new WishlishDTO
            {
                WishlistId = c.WishlistId,
                CreateDate = c.CreateDate,
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
            foreach (var item in listDTO)
            {
                total.Quantity ++;
            }
            return Ok(new ResponseObjectTotal(200, "Query data successfully", total, listDTO));
        }

        [HttpPost]
        public async Task<ActionResult<Wishlist>> Save(CartModel model)
        {
            try
            {
                var product = await _context.Products.FindAsync(model.ProductId);
                Wishlist wishlist = new Wishlist
                {
                    UserId = model.UserId,
                    ProductId = model.ProductId,
                    CreateDate = DateTime.Now,
                };
                await _context.Wishlists.AddAsync(wishlist);
                await _context.SaveChangesAsync();
                // save memory

                return Ok(new ResponseObject(200, "Insert data successfully", model));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Wishlist>> Delete(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Wishlists.Remove(wishlist);
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
