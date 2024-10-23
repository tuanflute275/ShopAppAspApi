using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;
using System.Reflection.Metadata;
using X.PagedList;

namespace ShopApp.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/banner")]
    public class BannerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BannerController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<Banner>> Index(string? name, string? sort, int page = 1)
        {
            var banners = await _context.Banners.ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                banners = await _context.Banners.Where(x => x.Title.Contains(name)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        banners = await _context.Banners.OrderBy(x => x.BannerId).ToListAsync();
                        break;
                    case "Id-DESC":
                        banners = await _context.Banners.OrderByDescending(x => x.BannerId).ToListAsync();
                        break;

                    case "Date-ASC":
                        banners = await _context.Banners.OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        banners = await _context.Banners.OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        banners = await _context.Banners.Where(x => x.Title.Contains(name)).OrderBy(x => x.BannerId).ToListAsync();
                        break;
                    case "Id-DESC":
                        banners = await _context.Banners.Where(x => x.Title.Contains(name)).OrderByDescending(x => x.BannerId).ToListAsync();
                        break;

                    case "Date-ASC":
                        banners = await _context.Banners.Where(x => x.Title.Contains(name)).OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        banners = await _context.Banners.Where(x => x.Title.Contains(name)).OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (banners.Count > 0)
            {
                int totalRecords = banners.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = banners.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", banners));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Banner>> FindById(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", banner));
        }

        [HttpPost]
        public async Task<ActionResult<Banner>> Save([FromForm] BannerModel model)
        {
            try
            {
                Banner banner = new Banner();
                List<Banner> foundData = await _context.Banners.Where(x => x.Title == model.Title).ToListAsync();
                if (foundData.Count > 0)
                {
                    return BadRequest(new ResponseObject(400, "Banner title already taken"));
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
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banners");
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
                        banner.Image = $"http://{HttpContext.Request.Host.Value}/uploads/blogs/{model.ImageFile.FileName}";
                    }

                    banner.Title = model.Title;
                    banner.CreateDate = DateTime.Now;
                    banner.UpdateDate = null;
                    await _context.Banners.AddAsync(banner);
                    await _context.SaveChangesAsync();
                    return Ok(new ResponseObject(200, "Insert data successfully", banner));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Banner>> Update(int id, [FromForm] BannerModel model)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                try
                {
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        // Đường dẫn lưu hình ảnh mới
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banners");
                        var newFilePath = Path.Combine(uploadsFolder, model.ImageFile.FileName);

                        // Xóa hình ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(model.OldImage))
                        {
                            var oldFileName = model.OldImage.Split($"{HttpContext.Request.Host.Value}/uploads/banners/").LastOrDefault();
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

                        banner.Image = $"http://{HttpContext.Request.Host.Value}/uploads/banners/{model.ImageFile.FileName}";
                    }
                    else
                    {
                        // Nếu không có hình ảnh mới, giữ nguyên hình ảnh cũ
                        banner.Image = model.OldImage;
                    }
                    banner.Title = model.Title;
                    banner.CreateDate = null;
                    banner.UpdateDate = DateTime.Now;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", banner));
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
        public async Task<ActionResult<Banner>> Delete(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
           
            try
            {
                _context.Banners.Remove(banner);
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
