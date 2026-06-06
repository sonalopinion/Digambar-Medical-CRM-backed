using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElevateERP.API.Data;
using ElevateERP.API.DTOs;
using ElevateERP.API.Models;

namespace ElevateERP.API.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DepartmentsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Departments.OrderBy(d => d.DepartmentName)
                .Select(d => new DepartmentDto { Id = d.Id, DepartmentName = d.DepartmentName, Description = d.Description })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var d = await _db.Departments.FindAsync(id);
            if (d == null) return NotFound();
            return Ok(new DepartmentDto { Id = d.Id, DepartmentName = d.DepartmentName, Description = d.Description });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentCreateDto dto)
        {
            var dept = new Department { DepartmentName = dto.DepartmentName, Description = dto.Description };
            _db.Departments.Add(dept);
            await _db.SaveChangesAsync();
            return Ok(new DepartmentDto { Id = dept.Id, DepartmentName = dept.DepartmentName, Description = dept.Description });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentCreateDto dto)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            dept.DepartmentName = dto.DepartmentName; dept.Description = dto.Description;
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            _db.Departments.Remove(dept);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }

    [ApiController, Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Categories.OrderBy(c => c.CategoryName)
                .Select(c => new CategoryDto { Id = c.Id, CategoryName = c.CategoryName, Description = c.Description })
                .ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            var cat = new Category { CategoryName = dto.CategoryName, Description = dto.Description };
            _db.Categories.Add(cat);
            await _db.SaveChangesAsync();
            return Ok(new CategoryDto { Id = cat.Id, CategoryName = cat.CategoryName, Description = cat.Description });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryCreateDto dto)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            cat.CategoryName = dto.CategoryName; cat.Description = dto.Description;
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
