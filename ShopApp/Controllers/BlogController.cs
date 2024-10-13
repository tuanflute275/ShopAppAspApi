﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;
using X.PagedList;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("/api/blog")]
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public BlogController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<Blog>> FindAll(string? name, string? sort, int page = 1)
        {
            var blogs = await _context.Blogs.Include(x => x.User).ToListAsync();

            if (!string.IsNullOrEmpty(name))
            {
                blogs = await _context.Blogs.Where(x => x.BlogTitle.Contains(name)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        blogs = await _context.Blogs.OrderBy(x => x.BlogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogs = await _context.Blogs.OrderByDescending(x => x.BlogId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogs = await _context.Blogs.OrderBy(x => x.BlogTitle).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogs = await _context.Blogs.OrderByDescending(x => x.BlogTitle).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        blogs = await _context.Blogs.Where(x => x.BlogTitle.Contains(name)).OrderBy(x => x.BlogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogs = await _context.Blogs.Where(x => x.BlogTitle.Contains(name)).OrderByDescending(x => x.BlogId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogs = await _context.Blogs.Where(x => x.BlogTitle.Contains(name)).OrderBy(x => x.BlogTitle).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogs = await _context.Blogs.Where(x => x.BlogTitle.Contains(name)).OrderByDescending(x => x.BlogTitle).ToListAsync();
                        break;
                }
            }

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = blogs.ToPagedList(page, limit);
            return Ok(new ResponseObject(200, "Query data successfully", pageData));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Blog>> FindById(int id)
        {
            var blog = await _context.Blogs.Include(p => p.User).FirstOrDefaultAsync(p => p.BlogId == id);
            if (blog == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", blog));
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<Blog>> FindBySlug(string slug)
        {
            var blog = await _context.Blogs.Include(p => p.User).FirstOrDefaultAsync(p => p.BlogSlug == slug);
            if (blog == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with slug {slug}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", blog));
        }

        [HttpGet("condition")]
        public async Task<ActionResult<Blog>> FindAllDataByCondition(string? sort, int limit = 5)
        {
            limit = limit <= 1 ? 1 : limit;
            var blogs = await _context.Blogs.Include(p => p.User).Take(limit).ToListAsync();
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        blogs = await _context.Blogs.OrderBy(x => x.BlogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogs = await _context.Blogs.OrderByDescending(x => x.BlogId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogs = await _context.Blogs.OrderBy(x => x.BlogTitle).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogs = await _context.Blogs.OrderByDescending(x => x.BlogTitle).ToListAsync();
                        break;
                }
            }
            return Ok(new ResponseObject(200, "Query data successfully", blogs));
        }

        [HttpPost]
        public async Task<ActionResult<Blog>> Save([FromForm] BlogModel model)
        {
            try
            {
                Blog blog = new Blog();
                List<Blog> foundData = await _context.Blogs.Where(x => x.BlogTitle == model.BlogTitle).ToListAsync();
                if (foundData.Count > 0)
                {
                    return BadRequest(new ResponseObject(400, "Blog name already taken"));
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
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "blogs");
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
                        blog.BlogImage = $"http://{HttpContext.Request.Host.Value}/uploads/blogs/{model.ImageFile.FileName}";
                    }

                    blog.BlogTitle = model.BlogTitle;
                    blog.BlogSlug = Util.GenerateSlug(model.BlogTitle);
                    blog.UserId = model.UserId;
                    blog.BlogDescription = model.BlogDescription;
                    await _context.Blogs.AddAsync(blog);
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Insert data successfully", blog));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject(404, "Insert data failed", ex.Message));
            }
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Blog>> Update(int id, [FromForm] BlogModel model)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                try
                {
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        // Đường dẫn lưu hình ảnh mới
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "blogs");
                        var newFilePath = Path.Combine(uploadsFolder, model.ImageFile.FileName);

                        // Xóa hình ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(model.OldImage))
                        {
                            var oldFileName = model.OldImage.Split($"{HttpContext.Request.Host.Value}/uploads/blogs/").LastOrDefault();
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

                        blog.BlogImage = $"http://{HttpContext.Request.Host.Value}/uploads/blogs/{model.ImageFile.FileName}";
                    }
                    else
                    {
                        // Nếu không có hình ảnh mới, giữ nguyên hình ảnh cũ
                        blog.BlogImage = model.OldImage;
                    }
                    blog.BlogTitle = model.BlogTitle;
                    blog.BlogSlug = Util.GenerateSlug(model.BlogTitle);
                    blog.UserId = model.UserId;
                    blog.BlogDescription = model.BlogDescription;
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", blog));
                }
                catch (Exception ex)
                {
                    return BadRequest(new ResponseObject(404, "Update data failed"));
                }
            }
            else
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Blog>> Delete(int id)
        {
            var blog = await _context.Blogs.Include(p => p.User).FirstOrDefaultAsync(p => p.BlogId == id);
            if (blog == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Delete data successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject(404, "Insert data failed"));
            }
        }
    }
}