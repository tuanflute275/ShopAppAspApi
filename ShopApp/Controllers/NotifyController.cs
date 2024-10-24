using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models.Entities;
using X.PagedList;
using ShopApp.Utils;
using ShopApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("/api/notify")]
    public class NotifyController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NotifyController(ApplicationDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Notification>> FindAll(string? sort, int page = 1)
        {
            var notifications = await _context.Notifications.ToListAsync();
           
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        notifications = await _context.Notifications.OrderBy(x => x.NotificationId).ToListAsync();
                        break;
                    case "Id-DESC":
                        notifications = await _context.Notifications.OrderByDescending(x => x.NotificationId).ToListAsync();
                        break;

                    case "Date-ASC":
                        notifications = await _context.Notifications.OrderBy(x => x.DateSent).ToListAsync();
                        break;
                    case "Date-DESC":
                        notifications = await _context.Notifications.OrderByDescending(x => x.DateSent).ToListAsync();
                        break;
                }
            }

            if (notifications.Count > 0)
            {
                int totalRecords = notifications.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = notifications.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", notifications));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Notification>> FindById(int id)
        {
            var notify = await _context.Notifications.FindAsync(id);
            if (notify == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", notify));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Notification>> Save(NotifyModel model)
        {
            try
            {
                Notification notification = new Notification();
                notification.Message = model.Message;
                notification.IsRead = false;
                notification.DateSent = DateTime.Now;
                await _context.Notifications.AddAsync(notification);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Insert data successfully", notification));
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Notification>> Update(int id, NotifyModel model)
        {
            var notify = await _context.Notifications.FindAsync(id);
            if (notify != null)
            {
                try
                {
                    notify.Message = model.Message;
                    notify.IsRead = model.IsRead;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", notify));
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
        public async Task<ActionResult<Notification>> Delete(int id)
        {
            var notyfy = await _context.Notifications.FindAsync(id);
            if (notyfy == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Notifications.Remove(notyfy);
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
