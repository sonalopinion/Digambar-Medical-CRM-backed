
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using ElevateERP.API.Data;
using ElevateERP.API.DTOs;
using ElevateERP.API.Models;

namespace ElevateERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StaffController(AppDbContext db)
        {
            _db = db;
        }

        private static StaffDto Map(Staff s)
        {
            return new StaffDto
            {
                Id = s.Id,

                FirmId = s.FirmId,
                FirmName = s.Firm?.FirmName,

                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department?.DepartmentName,

                StaffName = s.StaffName,
                Role = s.Role,
                MobileNumber = s.MobileNumber,
                EmailId = s.EmailId,
                Username = s.Username,

                //Password = "",
                //Password = s.PasswordHash,
                Password = s.PlainPassword,

                Address = s.Address,

                JoiningDate = s.JoiningDate,
                DateOfBirth = s.DateOfBirth,

                Education = s.Education,
                Skill = s.Skill,

                AdharNumber = s.AdharNumber,
                PanNumber = s.PanNumber,
                AccountNumber = s.AccountNumber,

                InsuranceDetails = s.InsuranceDetails,
                InsuranceStart = s.InsuranceStart,
                InsuranceEnd = s.InsuranceEnd
            };
        }
        // ================= GET ALL =================

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? firmId,
            [FromQuery] int? departmentId,
            [FromQuery] string? search)
        {
            try
            {
                var query = _db.Staff
                    .Include(s => s.Firm)
                    .Include(s => s.Department)
                    .Where(s => s.IsActive)
                    .AsQueryable();

                if (firmId.HasValue)
                    query = query.Where(s => s.FirmId == firmId.Value);

                if (departmentId.HasValue)
                    query = query.Where(s => s.DepartmentId == departmentId.Value);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(s =>
                        (s.StaffName != null &&
                         s.StaffName.Contains(search)) ||

                        (s.MobileNumber != null &&
                         s.MobileNumber.Contains(search)));
                }

                var list = await query
                    .OrderBy(s => s.StaffName)
                    .ToListAsync();

                return Ok(list.Select(Map));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ================= GET BY ID =================

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var staff = await _db.Staff
                    .Include(s => s.Firm)
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (staff == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Staff not found"
                    });

                return Ok(Map(staff));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var list = await _db.Staff
        //        .Where(s => s.IsActive)
        //        .OrderBy(s => s.StaffName)
        //        .Select(s => new StaffDropdownDto
        //        {
        //            Id = s.Id,
        //            StaffName = s.StaffName
        //        })
        //        .ToListAsync();
        //    return Ok(list);
        //}

        // ================= CREATE =================

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request body"
                    });
                }

                // CHECK FIRM EXISTS
                var firmExists = await _db.Firms
                    .AnyAsync(f => f.Id == dto.FirmId);

                if (!firmExists)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid FirmId"
                    });
                }

                // CHECK DEPARTMENT EXISTS
                var departmentExists = await _db.Departments
                    .AnyAsync(d => d.Id == dto.DepartmentId);

                if (!departmentExists)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid DepartmentId"
                    });
                }

                // CHECK USERNAME EXISTS
                var usernameExists = await _db.Staff
                    .AnyAsync(s => s.Username == dto.Username);

                if (usernameExists)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Username already exists"
                    });
                }

                var staff = new Staff
                {
                    FirmId = dto.FirmId,
                    DepartmentId = dto.DepartmentId,

                    StaffName = dto.StaffName,
                    Role = dto.Role,
                    MobileNumber = dto.MobileNumber,
                    EmailId = dto.EmailId,
                    Username = dto.Username,

                    //PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    //    string.IsNullOrWhiteSpace(dto.Password)
                    //        ? "Staff@123"
                    //        : dto.Password
                    //),
                    PlainPassword = string.IsNullOrWhiteSpace(dto.Password)
    ? "Staff@123"
    : dto.Password,

                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(
    string.IsNullOrWhiteSpace(dto.Password)
        ? "Staff@123"
        : dto.Password
),

                    Address = dto.Address,

                    JoiningDate = dto.JoiningDate?.ToUniversalTime(),
                    DateOfBirth = dto.DateOfBirth?.ToUniversalTime(),

                    Education = dto.Education,
                    Skill = dto.Skill,

                    AdharNumber = dto.AdharNumber,
                    PanNumber = dto.PanNumber,
                    AccountNumber = dto.AccountNumber,

                    InsuranceDetails = dto.InsuranceDetails,
                    InsuranceStart = dto.InsuranceStart?.ToUniversalTime(),
                    InsuranceEnd = dto.InsuranceEnd?.ToUniversalTime(),

                    IsActive = true
                };

                _db.Staff.Add(staff);

                await _db.SaveChangesAsync();

                await _db.Entry(staff)
                    .Reference(s => s.Firm)
                    .LoadAsync();

                await _db.Entry(staff)
                    .Reference(s => s.Department)
                    .LoadAsync();

                return Ok(new
                {
                    success = true,
                    message = "Staff created successfully",
                    data = Map(staff)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ================= UPDATE =================

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] StaffCreateDto dto)
        {
            try
            {
                var staff = await _db.Staff.FindAsync(id);

                if (staff == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Staff not found"
                    });
                }

                staff.FirmId = dto.FirmId;
                staff.DepartmentId = dto.DepartmentId;

                staff.StaffName = dto.StaffName;
                staff.Role = dto.Role;

                staff.MobileNumber = dto.MobileNumber;
                staff.EmailId = dto.EmailId;

                staff.Username = dto.Username;
                staff.Address = dto.Address;

                staff.JoiningDate = dto.JoiningDate?.ToUniversalTime();
                staff.DateOfBirth = dto.DateOfBirth?.ToUniversalTime();

                staff.Education = dto.Education;
                staff.Skill = dto.Skill;

                staff.AdharNumber = dto.AdharNumber;
                staff.PanNumber = dto.PanNumber;
                staff.AccountNumber = dto.AccountNumber;

                staff.InsuranceDetails = dto.InsuranceDetails;
                staff.InsuranceStart = dto.InsuranceStart?.ToUniversalTime();
                staff.InsuranceEnd = dto.InsuranceEnd?.ToUniversalTime();

                //if (!string.IsNullOrWhiteSpace(dto.Password))
                //{
                //    staff.PasswordHash =
                //        BCrypt.Net.BCrypt.HashPassword(dto.Password);
                //}
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    staff.PlainPassword = dto.Password;

                    staff.PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Staff updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ================= DELETE =================

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var staff = await _db.Staff.FindAsync(id);

                if (staff == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Staff not found"
                    });
                }

                staff.IsActive = false;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Staff deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ================= GET ALL STAFF =================

        [HttpGet("all")]
        public async Task<IActionResult> GetAllStaff()
        {
            try
            {
                var staffList = await _db.Staff
                    .Include(s => s.Firm)
                    .Include(s => s.Department)
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.StaffName)
                    .ToListAsync();

                return Ok(staffList.Select(Map));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ================= EXPORT EXCEL =================

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                var list = await _db.Staff
                    .Include(s => s.Firm)
                    .Include(s => s.Department)
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.StaffName)
                    .ToListAsync();

                using var workbook = new XLWorkbook();

                var worksheet = workbook.Worksheets.Add("Staff");

                var headers = new[]
                {
                    "ID",
                    "Firm",
                    "Department",
                    "Role",
                    "Name",
                    "Mobile",
                    "Email",
                    "Username",
                    "Joining Date",
                    "DateOfBirth",
                    "Education",
                    "Skill",
                    "Account"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }

                for (int i = 0; i < list.Count; i++)
                {
                    var s = list[i];

                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = s.Id;
                    worksheet.Cell(row, 2).Value = s.Firm?.FirmName;
                    worksheet.Cell(row, 3).Value = s.Department?.DepartmentName;
                    worksheet.Cell(row, 4).Value = s.Role;
                    worksheet.Cell(row, 5).Value = s.StaffName;
                    worksheet.Cell(row, 6).Value = s.MobileNumber;
                    worksheet.Cell(row, 7).Value = s.EmailId;
                    worksheet.Cell(row, 8).Value = s.Username;
                    worksheet.Cell(row, 9).Value = s.JoiningDate?.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 10).Value = s.DateOfBirth?.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 11).Value = s.Education;
                    worksheet.Cell(row, 12).Value = s.Skill;
                    worksheet.Cell(row, 13).Value = s.AccountNumber;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();

                workbook.SaveAs(stream);

                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Staff.xlsx"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}