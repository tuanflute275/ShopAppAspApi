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
    [Route("api/subscription")]
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string? type, string? sort, int page = 1)
        {
            var subscriptions = await _context.Subscriptions.Include(x => x.User).ToListAsync();
            if (!string.IsNullOrEmpty(type))
            {
                subscriptions = await _context.Subscriptions.Where(x => x.SubscriptionType.Contains(type)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        subscriptions = await _context.Subscriptions.OrderBy(x => x.SubscriptionId).ToListAsync();
                        break;
                    case "Id-DESC":
                        subscriptions = await _context.Subscriptions.OrderByDescending(x => x.SubscriptionId).ToListAsync();
                        break;

                    case "Date-ASC":
                        subscriptions = await _context.Subscriptions.OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        subscriptions = await _context.Subscriptions.OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        subscriptions = await _context.Subscriptions.Where(x => x.SubscriptionType.Contains(type)).OrderBy(x => x.SubscriptionId).ToListAsync();
                        break;
                    case "Id-DESC":
                        subscriptions = await _context.Subscriptions.Where(x => x.SubscriptionType.Contains(type)).OrderByDescending(x => x.SubscriptionId).ToListAsync();
                        break;

                    case "Date-ASC":
                        subscriptions = await _context.Subscriptions.Where(x => x.SubscriptionType.Contains(type)).OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        subscriptions = await _context.Subscriptions.Where(x => x.SubscriptionType.Contains(type)).OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }

            var subscriptionDTOs = subscriptions.Select(s => new SubscriptionDTO
            {
               SubscriptionId = s.SubscriptionId,
               SubscriptionType = s.SubscriptionType,
               CreateDate = s.CreateDate,
               UpdateDate = s.UpdateDate,
               Username = s.User.UserName
            }).ToList();

            if (subscriptionDTOs.Count > 0)
            {
                int totalRecords = subscriptionDTOs.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = subscriptionDTOs.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", subscriptionDTOs));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Subscription>> FindById(int id)
        {
            var subcription = await _context.Subscriptions.FindAsync(id);
            if (subcription == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", subcription));
        }


        [HttpPost]
        public async Task<ActionResult<Subscription>> Save(SubscriptionModel model)
        {
            try
            {
                Subscription subscription = new Subscription();
                subscription.SubscriptionType = model.SubscriptionType;
                subscription.UserId = model.UserId;
                subscription.CreateDate = DateTime.Now;
                subscription.UpdateDate = null;
                await _context.Subscriptions.AddAsync(subscription);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Insert data successfully", subscription));
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Subscription>> Update(int id, SubscriptionModel model)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                try
                {
                    subscription.SubscriptionType = model.SubscriptionType;
                    subscription.UserId=model.UserId;
                    subscription.UpdateDate= DateTime.Now;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", subscription));
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
        public async Task<ActionResult<Subscription>> Delete(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Subscriptions.Remove(subscription);
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
