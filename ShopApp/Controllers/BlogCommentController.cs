﻿using Microsoft.AspNetCore.Authorization;
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
    [Route("api/blogComment")]
    public class BlogCommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BlogCommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BlogComment>> FindAll(string? name, string? sort, int page = 1)
        {
            var blogCmts = await _context.BlogComments.ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                blogCmts = await _context.BlogComments.Where(x => x.Name.Contains(name)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        blogCmts = await _context.BlogComments.OrderBy(x => x.BlogCommentId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogCmts = await _context.BlogComments.OrderByDescending(x => x.BlogCommentId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogCmts = await _context.BlogComments.OrderBy(x => x.Name).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogCmts = await _context.BlogComments.OrderByDescending(x => x.Name).ToListAsync();
                        break;

                    case "Date-ASC":
                        blogCmts = await _context.BlogComments.OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        blogCmts = await _context.BlogComments.OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.Name.Contains(name)).OrderBy(x => x.BlogCommentId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.BlogCommentId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.Name).ToListAsync();
                        break;

                    case "Date-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.Name.Contains(name)).OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (blogCmts.Count > 0)
            {
                int totalRecords = blogCmts.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = blogCmts.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", blogCmts));
        }

        [HttpGet("{blogId}")]
        public async Task<ActionResult<BlogComment>> FindByBlogId(int blogId, string? name, string? sort, int page = 1)
        {
            var blogCmts = await _context.BlogComments
                .Where(x => x.BlogId == blogId)
                .ToListAsync();
            if (blogCmts == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with blogId {blogId} ", null));
            }

            if (!string.IsNullOrEmpty(name))
            {
                blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId && x.Name.Contains(name) || x.Email.Contains(name)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId).OrderBy(x => x.BlogCommentId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId).OrderByDescending(x => x.BlogCommentId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId).OrderBy(x => x.Name).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId).OrderByDescending(x => x.Name).ToListAsync();
                        break;

                    case "Date-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId).OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId).OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId && x.Name.Contains(name) || x.Email.Contains(name)).OrderBy(x => x.BlogCommentId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId && x.Name.Contains(name) || x.Email.Contains(name)).OrderByDescending(x => x.BlogCommentId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId && x.Name.Contains(name) || x.Email.Contains(name)).OrderBy(x => x.Name).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId && x.Name.Contains(name) || x.Email.Contains(name)).OrderByDescending(x => x.Name).ToListAsync();
                        break;

                    case "Date-ASC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId && x.Name.Contains(name) || x.Email.Contains(name)).OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        blogCmts = await _context.BlogComments.Where(x => x.BlogId == blogId && x.Name.Contains(name) || x.Email.Contains(name)).OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (blogCmts.Count > 0)
            {
                int totalRecords = blogCmts.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = blogCmts.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", blogCmts));
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{userId}/{blogId}")]
        public async Task<ActionResult<BlogComment>> FindById(int userId, int blogId)
        {
            var blogCmt = await _context.BlogComments
                .Where(x => x.UserId == userId && x.BlogId == blogId)
                .ToListAsync();
            if (blogCmt == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with userId {userId} or blogId {blogId} ", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", blogCmt));
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<BlogComment>> Save(CommentModel model)
        {
            try
            {

                BlogComment blogCmt = new BlogComment
                {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message,
                    CreateDate = DateTime.Now,
                    UpdateDate = null,
                    UserId = model.UserId,
                    BlogId = model.IdPost
                };
                await _context.BlogComments.AddAsync(blogCmt);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Insert data successfully", blogCmt));
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<ActionResult<BlogComment>> Update(int id, CommentUpdateModel model)
        {
            var blogCmt = await _context.BlogComments.FindAsync(id);
            if (blogCmt != null)
            {
                try
                {
                    blogCmt.Name = model.Name;
                    blogCmt.Email = model.Email;
                    blogCmt.Message = model.Message;
                    blogCmt.UpdateDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok(new ResponseObject(200, "Update data successfully", blogCmt));
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

        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BlogComment>> Delete(int id)
        {
            var blogCmt = await _context.BlogComments.FindAsync(id);
            if (blogCmt == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.BlogComments.Remove(blogCmt);
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
