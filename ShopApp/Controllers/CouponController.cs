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
    //[Authorize]
    [ApiController]
    [Route("/api/coupon")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Coupon>> FindAll(string? code, string? sort, int page = 1)
        {
            var coupons = await _context.Coupons.Include(x => x.CouponConditions).ToListAsync();
            if (!string.IsNullOrEmpty(code))
            {
                coupons = await _context.Coupons.Where(x => x.Code.Contains(code)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        coupons = await _context.Coupons.OrderBy(x => x.CouponId).ToListAsync();
                        break;
                    case "Id-DESC":
                        coupons = await _context.Coupons.OrderByDescending(x => x.CouponId).ToListAsync();
                        break;

                    case "Name-ASC":
                        coupons = await _context.Coupons.OrderBy(x => x.Code).ToListAsync();
                        break;
                    case "Name-DESC":
                        coupons = await _context.Coupons.OrderByDescending(x => x.Code).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        coupons = await _context.Coupons.Where(x => x.Code.Contains(code)).OrderBy(x => x.CouponId).ToListAsync();
                        break;
                    case "Id-DESC":
                        coupons = await _context.Coupons.Where(x => x.Code.Contains(code)).OrderByDescending(x => x.CouponId).ToListAsync();
                        break;

                    case "Name-ASC":
                        coupons = await _context.Coupons.Where(x => x.Code.Contains(code)).OrderBy(x => x.Code).ToListAsync();
                        break;
                    case "Name-DESC":
                        coupons = await _context.Coupons.Where(x => x.Code.Contains(code)).OrderByDescending(x => x.Code).ToListAsync();
                        break;
                }
            }

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = coupons.ToPagedList(page, limit);
            return Ok(new ResponseObject(200, "Query data successfully", pageData));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Coupon>> FindById(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", coupon));
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<Coupon>> FindByCode(string code)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Code == code);
            if (coupon == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with code {code}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", coupon));
        }

        [HttpGet("caculateValue")]
        public async Task<double> CalculateCouponValueAsync(string couponCode, double totalAmount)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode)
                          ?? throw new ArgumentException("Coupon not found");

            if (!coupon.Active)
            {
                throw new ArgumentException("Coupon is not active");
            }

            double discount = await CalculateDiscountAsync(coupon, totalAmount);
            double finalAmount = totalAmount - discount;
            return finalAmount;
        }
        [HttpGet("caculateDiscount")]
        public async Task<double> CalculateDiscountAsync(Coupon coupon, double totalAmount)
        {
            var conditions = await _context.CouponConditions
                .Where(cc => cc.CouponId == coupon.CouponId)
                .ToListAsync();

            double discount = 0.0;
            foreach (var condition in conditions)
            {
                string attribute = condition.Attribute;
                string operator_ = condition.Operator;
                string value = condition.Value;

                double percentDiscount = Convert.ToDouble(condition.DiscountAmount);

                // Điều kiện "minimum_amount"
                if (attribute == "minimum_amount")
                {
                    if (operator_ == ">" && totalAmount > Convert.ToDouble(value))
                    {
                        discount += totalAmount * percentDiscount / 100;
                    }
                }
                // Điều kiện "applicable_date"
                else if (attribute == "applicable_date")
                {
                    DateTime applicableDate = DateTime.Parse(value);
                    DateTime currentDate = DateTime.Now;
                    if (operator_.Equals("BETWEEN", StringComparison.OrdinalIgnoreCase) && currentDate.Date == applicableDate.Date)
                    {
                        discount += totalAmount * percentDiscount / 100;
                    }
                }
                // Các điều kiện khác có thể bổ sung tại đây
            }

            return discount;
        }


        [HttpPost]
        public async Task<ActionResult<Coupon>> Save(Coupon coupon)
        {
            try
            {
                List<Coupon> foundData = _context.Coupons.Where(x => x.Code == coupon.Code).ToList();
                if (foundData.Count > 0)
                {
                    return BadRequest(new ResponseObject(400, "Coupon name already taken"));
                }
                else
                {
                    await _context.Coupons.AddAsync(coupon);
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Insert data successfully", coupon));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Coupon>> Update(int id, Coupon coupon)
        {
            var foundData = await _context.Coupons.FindAsync(id);
            if (foundData != null)
            {
                try
                {
                    foundData.Code = coupon.Code;
                    foundData.Percent = coupon.Percent;
                    foundData.Active = coupon.Active;
                    foundData.Description = coupon.Description;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", coupon));
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
        public async Task<ActionResult<Coupon>> Delete(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Delete data successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }
    }
}
