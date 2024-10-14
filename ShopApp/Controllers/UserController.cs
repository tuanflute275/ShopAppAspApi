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
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<User>> FindAll(string? username, string? email, string? sort, int page = 1)
        {
            var users = await _context.Users.ToListAsync();
            if (!string.IsNullOrEmpty(username))
            {
                users = await _context.Users.Where(x => x.UserName.Contains(username)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(email))
            {
                users = await _context.Users.Where(x => x.UserEmail.Contains(email)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        users = await _context.Users.OrderBy(x => x.Id).ToListAsync();
                        break;
                    case "Id-DESC":
                        users = await _context.Users.OrderByDescending(x => x.Id).ToListAsync();
                        break;

                    case "Name-ASC":
                        users = await _context.Users.OrderBy(x => x.UserName).ToListAsync();
                        break;
                    case "Name-DESC":
                        users = await _context.Users.OrderByDescending(x => x.UserName).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        users = await _context.Users.Where(x => x.UserName.Contains(username)).OrderBy(x => x.Id).ToListAsync();
                        break;
                    case "Id-DESC":
                        users = await _context.Users.Where(x => x.UserName.Contains(username)).OrderByDescending(x => x.Id).ToListAsync();
                        break;

                    case "Name-ASC":
                        users = await _context.Users.Where(x => x.UserName.Contains(username)).OrderBy(x => x.Id).ToListAsync();
                        break;
                    case "Name-DESC":
                        users = await _context.Users.Where(x => x.UserName.Contains(username)).OrderByDescending(x => x.Id).ToListAsync();
                        break;
                }
            }

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = users.ToPagedList(page, limit);
            return Ok(new ResponseObject(200, "Query data successfully", pageData));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> FindById(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", user));
        }

        [HttpPost]
        public async Task<ActionResult<User>> Save([FromForm] UserModel model)
        {
            try
            {
                User user = new User();
                List<User> foundData = await _context.Users.Where(x => x.UserName == model.UserName).ToListAsync();
                if (foundData.Count > 0)
                {
                    return BadRequest(new ResponseObject(400, "Username name already taken"));
                }
                else
                {
                    if (model.ImageFile == null)
                    {
                        return BadRequest(new ResponseObject(400, "Image Is Required", null));
                    }

                    if (model.ImageFile.Length > 0)
                    {
                        // Đường dẫn lưu file
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users");
                        var filePath = Path.Combine(uploadsFolder, model.ImageFile.FileName);

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Lưu file hình ảnh
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        // Cập nhật đường dẫn hình ảnh vào thuộc tính sản phẩm
                        user.UserAvatar = $"http://{HttpContext.Request.Host.Value}/uploads/users/{model.ImageFile.FileName}";
                    }
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.UserPassword, 12);
                    user.UserName = model.UserName;
                    user.UserFullName = model.UserName;
                    user.UserEmail = model.UserName;
                    user.UserPassword = passwordHash;
                    user.UserPhoneNumber = model.UserName;
                    user.UserAddress = model.UserName;
                    user.UserGender = model.UserGender;
                    user.UserActive = model.UserActive;
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    // lấy userId vừa tạo
                    var userId = user.Id;
                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == model.Role);
                    if (role != null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = userId,
                            RoleId = role.Id
                        };

                        _context.UserRoles.Add(userRole);
                        await _context.SaveChangesAsync();
                    }
                    return Ok(new ResponseObject(200, "Insert data successfully", model));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromForm] UserModel model)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                try
                {
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        // Đường dẫn lưu hình ảnh mới
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "users");
                        var newFilePath = Path.Combine(uploadsFolder, model.ImageFile.FileName);

                        // Xóa hình ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(model.OldImage))
                        {
                            var oldFileName = model.OldImage.Split($"{HttpContext.Request.Host.Value}/uploads/users/").LastOrDefault();
                            var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Lưu file hình ảnh mới
                        using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        user.UserAvatar = $"http://{HttpContext.Request.Host.Value}/uploads/users/{model.ImageFile.FileName}";
                    }
                    else
                    {
                        // Nếu không có hình ảnh mới, giữ nguyên hình ảnh cũ
                        user.UserAvatar = model.OldImage;
                    }
                    user.UserName = model.UserName;
                    user.UserFullName = model.UserName;
                    user.UserEmail = model.UserEmail;
                    user.UserPassword = BCrypt.Net.BCrypt.HashPassword(model.UserPassword, 12);
                    user.UserPhoneNumber = model.UserPhoneNumber;
                    user.UserAddress = model.UserAddress;
                    user.UserGender = model.UserGender;
                    user.UserActive = model.UserActive;
                    await _context.SaveChangesAsync();

                    // lấy userId vừa tạo
                    var userId = id;
                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == model.Role);
                    if (role != null)
                    {
                        // check data role
                        var checkRoleInDb = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
                        if (checkRoleInDb == null) 
                        {
                            var userRole = new UserRole
                            {
                                UserId = userId,
                                RoleId = role.Id
                            };
                            _context.UserRoles.Add(userRole);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            checkRoleInDb.RoleId = role.Id;
                            await _context.SaveChangesAsync();
                        }

                       
                    }
                    return Ok(new ResponseObject(200, "Update data successfully", model));
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
        public async Task<ActionResult<User>> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Users.Remove(user);
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
