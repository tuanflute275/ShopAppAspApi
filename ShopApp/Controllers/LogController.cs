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
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/log")]
    public class LogController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LogController(ApplicationDbContext context)
        { 
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Log>> FindAll(string? username, string? sort, int page = 1)
        {
            var logs = await _context.Logs.ToListAsync();
            if (!string.IsNullOrEmpty(username))
            {
                logs = await _context.Logs.Where(x => x.UserName.Contains(username) || x.WorkTation.Contains(username)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        logs = await _context.Logs.OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Date-ASC":
                        logs = await _context.Logs.OrderBy(x => x.TimeActionRequest).ToListAsync();
                        break;
                    case "Date-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.TimeActionRequest).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Date-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderBy(x => x.TimeActionRequest).ToListAsync();
                        break;
                    case "Date-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderByDescending(x => x.TimeActionRequest).ToListAsync();
                        break;
                }
            }

            if (logs.Count > 0)
            {
                int totalRecords = logs.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = logs.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", logs));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> FindById(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", log));
        }

        [HttpGet("logUser")]
        public async Task<ActionResult<Log>> LogUserLogin(string? username, string? sort, int page = 1)
        {
            var logs = await _context.Logs.Where(x => x.TimeActionRequest == null && x.TimeLogin != null).ToListAsync();
            if (!string.IsNullOrEmpty(username))
            {
                logs = await _context.Logs.Where(x => x.UserName.Contains(username)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        logs = await _context.Logs.OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Date-ASC":
                        logs = await _context.Logs.OrderBy(x => x.TimeActionRequest).ToListAsync();
                        break;
                    case "Date-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.TimeActionRequest).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Date-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderBy(x => x.TimeActionRequest).ToListAsync();
                        break;
                    case "Date-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderByDescending(x => x.TimeActionRequest).ToListAsync();
                        break;
                }
            }

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = logs.ToPagedList(page, limit);
            return Ok(new ResponseObject(200, "Query data successfully", pageData));
        }

        [HttpGet("logFunction")]
        public async Task<ActionResult<Log>> LogUserFunction(string? username, string? sort, int page = 1)
        {
            var logs = await _context.Logs.Where(x => x.TimeActionRequest != null && x.TimeLogin == null && x.TimeLogout == null).ToListAsync();
            if (!string.IsNullOrEmpty(username))
            {
                logs = await _context.Logs.Where(x => x.UserName.Contains(username)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        logs = await _context.Logs.OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Date-ASC":
                        logs = await _context.Logs.OrderBy(x => x.TimeActionRequest).ToListAsync();
                        break;
                    case "Date-DESC":
                        logs = await _context.Logs.OrderByDescending(x => x.TimeActionRequest).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderBy(x => x.LogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderByDescending(x => x.LogId).ToListAsync();
                        break;

                    case "Date-ASC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderBy(x => x.TimeActionRequest).ToListAsync();
                        break;
                    case "Date-DESC":
                        logs = await _context.Logs.Where(x => x.UserName.Contains(username)).OrderByDescending(x => x.TimeActionRequest).ToListAsync();
                        break;
                }
            }

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = logs.ToPagedList(page, limit);
            return Ok(new ResponseObject(200, "Query data successfully", pageData));
        }

        [HttpPost("saveUser")]
        public async Task<ActionResult<Log>> SaveUser(LogModel model)
        {
            try
            {
                Log log = new Log
                {
                    UserName = model.UserName,
                    WorkTation = model.WorkTation,
                    Request = model.Request,
                    Response = model.Response,
                    IpAdress = model.IpAdress,
                    TimeLogin = DateTime.Now,
                    TimeActionRequest = null,
                    TimeLogout = null
                };
                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();
                return Ok(new ResponseObject(200, "Insert data successfully", log));
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPost("saveFunction")]
        public async Task<ActionResult<Log>> SaveFunction(LogModel model)
        {
            try
            {
                Log log = new Log
                {
                    UserName = model.UserName,
                    WorkTation = model.WorkTation,
                    Request = model.Request,
                    Response = model.Response,
                    IpAdress = model.IpAdress,
                    TimeLogin = null,
                    TimeActionRequest = DateTime.Now,
                    TimeLogout = DateTime.Now
                };
                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();
                return Ok(new ResponseObject(200, "Insert data successfully", log));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Log>> Delete(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Logs.Remove(log);
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
