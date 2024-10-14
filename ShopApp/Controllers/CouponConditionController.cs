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
    [Route("/api/couponCondition")]
    public class CouponConditionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CouponConditionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<CouponCondition>> FindAll(string? sort, int page = 1)
        {
            var couponConditions = await _context.CouponConditions.Include(x => x.Coupon).ToListAsync();
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        couponConditions = await _context.CouponConditions.OrderBy(x => x.CouponConditionId).ToListAsync();
                        break;
                    case "Id-DESC":
                        couponConditions = await _context.CouponConditions.OrderByDescending(x => x.CouponConditionId).ToListAsync();
                        break;
                }
            }

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = couponConditions.ToPagedList(page, limit);
            return Ok(new ResponseObject(200, "Query data successfully", pageData));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CouponCondition>> FindById(int id)
        {
            var couponCondition = await _context.CouponConditions.Include(x => x.Coupon).FirstOrDefaultAsync(x => x.CouponConditionId == id);
            if (couponCondition == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", couponCondition));
        }

        [HttpPost]
        public async Task<ActionResult<CouponCondition>> Save(CouponConditionModel model)
        {
            try
            {
                CouponCondition couponCondition = new CouponCondition
                {
                    CouponId = model.CouponId,
                    Attribute = model.Attribute,
                    Operator = model.Operator,
                    Value = model.Value,
                    DiscountAmount = model.DiscountAmount,
                };
                await _context.CouponConditions.AddAsync(couponCondition);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Insert data successfully", couponCondition));
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CouponCondition>> Update(int id, CouponConditionModel model)
        {
            var foundData = await _context.CouponConditions.FindAsync(id);
            if (foundData != null)
            {
                try
                {
                    foundData.CouponId = model.CouponId;
                    foundData.Attribute = model.Attribute;
                    foundData.Operator = model.Operator;
                    foundData.Value = model.Value;
                    foundData.DiscountAmount = model.DiscountAmount;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", foundData));
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
        public async Task<ActionResult<CouponCondition>> Delete(int id)
        {
            var couponCondition = await _context.CouponConditions.FindAsync(id);
            if (couponCondition == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.CouponConditions.Remove(couponCondition);
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
