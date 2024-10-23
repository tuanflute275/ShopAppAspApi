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
    [Route("/api/role")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Role>> FindAll(string? name, string? sort, int page = 1)
        {
            var roles = await _context.Roles.ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                roles = await _context.Roles.Where(x => x.RoleName.Contains(name)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        roles = await _context.Roles.OrderBy(x => x.Id).ToListAsync();
                        break;
                    case "Id-DESC":
                        roles = await _context.Roles.OrderByDescending(x => x.Id).ToListAsync();
                        break;

                    case "Name-ASC":
                        roles = await _context.Roles.OrderBy(x => x.RoleName).ToListAsync();
                        break;
                    case "Name-DESC":
                        roles = await _context.Roles.OrderByDescending(x => x.RoleName).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        roles = await _context.Roles.Where(x => x.RoleName.Contains(name)).OrderBy(x => x.Id).ToListAsync();
                        break;
                    case "Id-DESC":
                        roles = await _context.Roles.Where(x => x.RoleName.Contains(name)).OrderByDescending(x => x.Id).ToListAsync();
                        break;

                    case "Name-ASC":
                        roles = await _context.Roles.Where(x => x.RoleName.Contains(name)).OrderBy(x => x.Id).ToListAsync();
                        break;
                    case "Name-DESC":
                        roles = await _context.Roles.Where(x => x.RoleName.Contains(name)).OrderByDescending(x => x.Id).ToListAsync();
                        break;
                }
            }

            if (roles.Count > 0)
            {
                int totalRecords = roles.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = roles.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", roles));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Role>> FindById(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", role));
        }

        [HttpPost]
        public async Task<ActionResult<Role>> Save(RoleModel model)
        {
            try
            {
                Role role = new Role
                {
                    RoleName = model.RoleName,
                    RoleDescription = model.RoleDesc
                };
                await _context.Roles.AddAsync(role);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Insert data successfully", role));
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Role>> Update(int id, RoleModel model)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                try
                {
                    role.RoleName = model.RoleName;
                    role.RoleDescription = model.RoleDesc;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", role));
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
        public async Task<ActionResult<Role>> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Roles.Remove(role);
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
