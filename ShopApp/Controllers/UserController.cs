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
    [Authorize(Roles = "Admin, User")]
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
        public async Task<ActionResult<User>> FindAll(string? name, string? sort, int page = 1)
        {
            var users = await _context.Users
                .Include(x => x.UserRoles)
                .ThenInclude(u => u.Role)
                .ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                users = await _context.Users.Where(x => x.UserName.Contains(name) || x.UserEmail.Contains(name)).ToListAsync();
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
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        users = await _context.Users.Where(x => x.UserName.Contains(name)).OrderBy(x => x.Id).ToListAsync();
                        break;
                    case "Id-DESC":
                        users = await _context.Users.Where(x => x.UserName.Contains(name)).OrderByDescending(x => x.Id).ToListAsync();
                        break;

                    case "Name-ASC":
                        users = await _context.Users.Where(x => x.UserName.Contains(name)).OrderBy(x => x.Id).ToListAsync();
                        break;
                    case "Name-DESC":
                        users = await _context.Users.Where(x => x.UserName.Contains(name)).OrderByDescending(x => x.Id).ToListAsync();
                        break;
                }
            }

            var userDTOs = users.Select(user => new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                UserFullName = user.UserName,
                UserAvatar = user.UserAvatar,
                UserEmail = user.UserEmail,
                UserPhoneNumber = user.UserPhoneNumber,
                UserAddress = user.UserAddress,
                UserGender = user.UserGender,
                UserActive = user.UserActive,
                UserCount = user.UserCount,
                UserCurrentTime = user.UserCurrentTime,
                UserUnlockTime = user.UserUnlockTime,
                RoleNames = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            }).ToList();

            if (userDTOs.Count > 0)
            {
                int totalRecords = userDTOs.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = userDTOs.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", userDTOs));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> FindById(int id)
        {
            var user = await _context.Users
                .Include(x => x.UserRoles)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            var userDTO = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                UserFullName = user.UserName,
                UserAvatar  = user.UserAvatar,
                UserEmail = user.UserEmail,
                UserPhoneNumber = user.UserPhoneNumber,
                UserAddress = user.UserAddress,
                UserGender = user.UserGender,
                UserActive = user.UserActive,
                UserCount = user.UserCount,
                UserCurrentTime = user.UserCurrentTime,
                UserUnlockTime = user.UserUnlockTime,
                RoleNames = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            };
            return Ok(new ResponseObject(200, "Query data successfully", userDTO));
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
                    user.UserEmail = model.UserEmail;
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
            var userRoles = await _context.UserRoles.Where(x => x.UserId == id).ToListAsync();
            var blogs = await _context.Blogs.Where(x => x.UserId == id).ToListAsync();
            var blogCmts = await _context.BlogComments.Where(x => x.UserId == id).ToListAsync();
            var carts = await _context.Carts.Where(x => x.UserId == id).ToListAsync();
            var wishlists = await _context.Wishlists.Where(x => x.UserId == id).ToListAsync();
            var orders = await _context.Orders.Where(x => x.UserId == id).ToListAsync();
            var productCmts = await _context.ProductComments.Where(x => x.UserId == id).ToListAsync();
            var subscriptions = await _context.Subscriptions.Where(x => x.UserId == id).ToListAsync();
            if (user == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            if (userRoles != null && userRoles.Count > 0)
            {
                _context.UserRoles.RemoveRange(userRoles);
                await _context.SaveChangesAsync();
            }
            if (blogs != null && blogs.Count > 0)
            {
                _context.Blogs.RemoveRange(blogs);
                await _context.SaveChangesAsync();
            }
            if (blogCmts != null && blogCmts.Count > 0)
            {
                _context.BlogComments.RemoveRange(blogCmts);
                await _context.SaveChangesAsync();
            }
            if (carts != null && carts.Count > 0)
            {
                _context.Carts.RemoveRange(carts);
                await _context.SaveChangesAsync();
            }
            if (wishlists != null && wishlists.Count > 0)
            {
                _context.Wishlists.RemoveRange(wishlists);
                await _context.SaveChangesAsync();
            }
            if (productCmts != null && productCmts.Count > 0)
            {
                _context.ProductComments.RemoveRange(productCmts);
                await _context.SaveChangesAsync();
            }
            if (subscriptions != null && subscriptions.Count > 0)
            {
                _context.Subscriptions.RemoveRange(subscriptions);
                await _context.SaveChangesAsync();
            }

            try
            {
                _context.Users.Remove(user);
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
