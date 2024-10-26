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
    [ApiController]
    [Route("api/productComment")]
    public class ProductCommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductCommentController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<ProductComment>> FindAll(string? name, string? sort, int page = 1)
        {
            var productCmts = await _context.ProductComments.ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                productCmts = await _context.ProductComments.Where(x => x.Name.Contains(name)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        productCmts = await _context.ProductComments.OrderBy(x => x.ProductCommentId).ToListAsync();
                        break;
                    case "Id-DESC":
                        productCmts = await _context.ProductComments.OrderByDescending(x => x.ProductCommentId).ToListAsync();
                        break;

                    case "Name-ASC":
                        productCmts = await _context.ProductComments.OrderBy(x => x.Name).ToListAsync();
                        break;
                    case "Name-DESC":
                        productCmts = await _context.ProductComments.OrderByDescending(x => x.Name).ToListAsync();
                        break;

                    case "Date-ASC":
                        productCmts = await _context.ProductComments.OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        productCmts = await _context.ProductComments.OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.Name.Contains(name)).OrderBy(x => x.ProductCommentId).ToListAsync();
                        break;
                    case "Id-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.ProductCommentId).ToListAsync();
                        break;

                    case "Name-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.Name.Contains(name)).OrderBy(x => x.Name).ToListAsync();
                        break;
                    case "Name-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.Name).ToListAsync();
                        break;

                    case "Date-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.Name.Contains(name)).OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (productCmts.Count > 0)
            {
                int totalRecords = productCmts.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = productCmts.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", productCmts));
        }


        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductComment>> FindByProductId(int productId, string? name, string? sort, int page = 1)
        {
            var productCmts = await _context.ProductComments
                .Where(x => x.ProductId == productId)
                .ToListAsync();
            if (productCmts == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with productId {productId} ", null));
            }
            if (!string.IsNullOrEmpty(name))
            {
                productCmts = await _context.ProductComments.Where(x => x.ProductId == productId && x.Name.Contains(name) || x.Email.Contains(name)).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId).OrderBy(x => x.ProductCommentId).ToListAsync();
                        break;
                    case "Id-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId).OrderByDescending(x => x.ProductCommentId).ToListAsync();
                        break;

                    case "Name-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId).OrderBy(x => x.Name).ToListAsync();
                        break;
                    case "Name-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId).OrderByDescending(x => x.Name).ToListAsync();
                        break;

                    case "Date-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId).OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId).OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId && x.Name.Contains(name) || x.Email.Contains(name)).OrderBy(x => x.ProductCommentId).ToListAsync();
                        break;
                    case "Id-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId && x.Name.Contains(name) || x.Email.Contains(name)).OrderByDescending(x => x.ProductCommentId).ToListAsync();
                        break;

                    case "Name-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId && x.Name.Contains(name) || x.Email.Contains(name)).OrderBy(x => x.Name).ToListAsync();
                        break;
                    case "Name-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId && x.Name.Contains(name) || x.Email.Contains(name)).OrderByDescending(x => x.Name).ToListAsync();
                        break;

                    case "Date-ASC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId && x.Name.Contains(name)).OrderBy(x => x.CreateDate).ToListAsync();
                        break;
                    case "Date-DESC":
                        productCmts = await _context.ProductComments.Where(x => x.ProductId == productId && x.Name.Contains(name)).OrderByDescending(x => x.CreateDate).ToListAsync();
                        break;
                }
            }
            if (productCmts.Count > 0)
            {
                int totalRecords = productCmts.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = productCmts.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = pageData
                };

                return Ok(new ResponseObject(200, "Query data successfully", response));
            }
            return Ok(new ResponseObject(200, "Query data successfully", productCmts));
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{userId}/{productId}")]
        public async Task<ActionResult<BlogComment>> FindById(int userId, int productId)
        {
            var productCmt = await _context.ProductComments
                .Where(x => x.UserId == userId && x.ProductId == productId)
                .ToListAsync();
            if (productCmt == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with userId {userId} or productId {productId} ", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", productCmt));
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<ProductComment>> Save(CommentModel model)
        {
            try
            {

                ProductComment productCmt = new ProductComment
                {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message,
                    CreateDate = DateTime.Now,
                    UpdateDate = null,
                    UserId = model.UserId,
                    ProductId = model.IdPost
                };
                await _context.ProductComments.AddAsync(productCmt);
                _context.SaveChanges();
                return Ok(new ResponseObject(200, "Insert data successfully", productCmt));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductComment>> Update(int id, CommentUpdateModel model)
        {
            var productCmt = await _context.ProductComments.FindAsync(id);
            if (productCmt != null)
            {
                try
                {
                    productCmt.Name = model.Name;
                    productCmt.Email = model.Email;
                    productCmt.Message = model.Message;
                    productCmt.UpdateDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok(new ResponseObject(200, "Update data successfully", productCmt));
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
        public async Task<ActionResult<ProductComment>> Delete(int id)
        {
            var productCmt = await _context.ProductComments.FindAsync(id);
            if (productCmt == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.ProductComments.Remove(productCmt);
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
