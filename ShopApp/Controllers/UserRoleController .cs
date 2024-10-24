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
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("/api/userRole")]
    public class UserRoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserRole>> FindAll(int page = 1)
        {
            var userRoles = await _context.UserRoles.Include(x => x.User).Include(x => x.Role).ToListAsync();
            var userRolesDTO = userRoles.Select(u => new UserRoleDTO
            {
               Id = u.Id,
               UserName = u.User.UserName,
               RoleName = u.Role.RoleName
            }).ToList();
            if (userRolesDTO.Count > 0)
            {
                int totalRecords = userRolesDTO.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = userRolesDTO.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", userRolesDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserRole>> FindById(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", userRole));
        }

        [HttpPost]
        public async Task<ActionResult<UserRole>> Save(UserRoleModel model)
        {
            try
            {
                UserRole userRole = new UserRole
                {
                    UserId = model.userId ?? 1,
                    RoleId = model.roleId
                };
                await _context.UserRoles.AddAsync(userRole);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Insert data successfully", userRole));
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserRole>> Update(int id, UserRoleModel model)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole != null)
            {
                try
                {
                    userRole.RoleId = model.roleId;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", userRole));
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
        public async Task<ActionResult<UserRole>> Delete(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.UserRoles.Remove(userRole);
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
