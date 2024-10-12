using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models.Entities;
using X.PagedList;
using ShopApp.Utils;
using ShopApp.Models.ViewModels;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("/api/category")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Category>> FindAll(string? name, string? sort, int page = 1)
        {
            var categories = await _context.Categories.ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                categories = _context.Categories.Where(x => x.CategoryName.Contains(name)).ToList();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        categories = await _context.Categories.OrderBy(x => x.CategoryId).ToListAsync();
                        break;
                    case "Id-DESC":
                        categories = await _context.Categories.OrderByDescending(x => x.CategoryId).ToListAsync();
                        break;

                    case "Name-ASC":
                        categories = await _context.Categories.OrderBy(x => x.CategoryName).ToListAsync();
                        break;
                    case "Name-DESC":
                        categories = await _context.Categories.OrderByDescending(x => x.CategoryName).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderBy(x => x.CategoryId).ToListAsync();
                        break;
                    case "Id-DESC":
                        categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderByDescending(x => x.CategoryId).ToListAsync();
                        break;

                    case "Name-ASC":
                        categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderBy(x => x.CategoryName).ToListAsync();
                        break;
                    case "Name-DESC":
                        categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderByDescending(x => x.CategoryName).ToListAsync();
                        break;
                }
            }

            int limit = 10;
            page = page <= 1 ? 1 : page;
            var pageData = categories.ToPagedList(page, limit);
            return Ok(new ResponseObject(200, "Query data successfully", pageData));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> FindById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", category));
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<Category>> FindBySlug(string slug)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategorySlug == slug);
            if (category == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with slug {slug}", null));
            }
            return Ok(new ResponseObject(200, "Query data successfully", category));
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Save(CategoryModel model)
        {
            try
            {
                List<Category> foundData = _context.Categories.Where(x => x.CategoryName == model.CategoryName).ToList();
                if (foundData.Count > 0)
                {
                    return BadRequest(new ResponseObject(400, "Category name already taken"));
                }
                else
                {
                    Category category = new Category
                    {
                        CategoryName = model.CategoryName,
                        CategoryStatus = model.CategoryStatus,
                        CategorySlug = Util.GenerateSlug(model.CategoryName),
                    };
                    await _context.Categories.AddAsync(category);
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Insert data successfully", model));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject(404, "Insert data failed"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> Update(int id, CategoryModel model)
        {
            var cate = await _context.Categories.FindAsync(id);
            if (cate != null)
            {
                try
                {
                    cate.CategoryName = model.CategoryName;
                    cate.CategoryStatus = model.CategoryStatus;
                    cate.CategorySlug = Util.GenerateSlug(model.CategoryName);
                    _context.SaveChanges();
                    return Ok(new ResponseObject(200, "Update data successfully", model));
                }
                catch (Exception ex) 
                {
                    return BadRequest(new ResponseObject(404, "Insert data failed"));
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new ResponseObject(404, $"Cannot find data with id {id}", null));
            }
            try
            {
                _context.Categories.Remove(category);
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
