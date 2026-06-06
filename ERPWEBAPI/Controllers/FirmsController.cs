using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElevateERP.API.Data;
using ElevateERP.API.DTOs;
using ElevateERP.API.Models;

namespace ElevateERP.API.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class FirmsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public FirmsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Firms.OrderBy(f => f.FirmName)
                .Select(f => new FirmDto { Id = f.Id, FirmName = f.FirmName, Address = f.Address, Phone = f.Phone, Email = f.Email })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var f = await _db.Firms.FindAsync(id);
            if (f == null) return NotFound();
            return Ok(new FirmDto { Id = f.Id, FirmName = f.FirmName, Address = f.Address, Phone = f.Phone, Email = f.Email });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FirmCreateDto dto)
        {
            var firm = new Firm { FirmName = dto.FirmName, Address = dto.Address, Phone = dto.Phone, Email = dto.Email };
            _db.Firms.Add(firm);
            await _db.SaveChangesAsync();
            return Ok(new FirmDto { Id = firm.Id, FirmName = firm.FirmName, Address = firm.Address, Phone = firm.Phone, Email = firm.Email });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FirmCreateDto dto)
        {
            var firm = await _db.Firms.FindAsync(id);
            if (firm == null) return NotFound();
            firm.FirmName = dto.FirmName; firm.Address = dto.Address;
            firm.Phone = dto.Phone; firm.Email = dto.Email;
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var firm = await _db.Firms.FindAsync(id);
            if (firm == null) return NotFound();
            _db.Firms.Remove(firm);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
