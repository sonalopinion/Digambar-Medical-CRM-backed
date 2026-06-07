using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElevateERP.API.Data;
using ElevateERP.API.DTOs;
using ElevateERP.API.Models;
using ClosedXML.Excel;

namespace ElevateERP.API.Controllers
{
    // ─── SHIFTS ───────────────────────────────────────────────────────────────


    [ApiController, Route("api/[controller]")]
    public class StaffFeedbackController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StaffFeedbackController(AppDbContext db)
        {
            _db = db;
        }

        // GET /api/stafffeedback?staffName=NIKAM&month=2026-04
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? staffName,
            [FromQuery] string? month)
        {
            var query = _db.StaffFeedbacks
                .Include(x => x.Feedback)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(staffName))
                query = query.Where(x => x.StaffName == staffName);

            if (!string.IsNullOrWhiteSpace(month) &&
                DateTime.TryParse(month + "-01", out var parsedMonth))
            {
                query = query.Where(x =>
                    x.FeedbackDate.Year == parsedMonth.Year &&
                    x.FeedbackDate.Month == parsedMonth.Month);
            }

            // Materialize first, then project — avoids EF translation errors
            // and null-ref on Feedback navigation property.
            var raw = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var data = raw.Select(x => new StaffFeedbackDto
            {
                Id = x.Id,
                FeedbackId = x.FeedbackId,
                FeedbackQuestion = x.Feedback?.Question ?? string.Empty,
                StaffName = x.StaffName,
                Feedback = x.SelectedAnswer,
                FeedbackDate = x.FeedbackDate.ToString("yyyy-MM-dd")
            }).ToList();

            return Ok(data);
        }

        //    [HttpGet]
        //    public async Task<IActionResult> GetAllBYDate(
        //[FromQuery] string? staffName,
        //[FromQuery] string? month)
        //    {
        //        var query = _db.StaffFeedbacks
        //            .Include(x => x.Feedback)
        //            .AsQueryable();

        //        if (!string.IsNullOrWhiteSpace(staffName))
        //        {
        //            query = query.Where(x => x.StaffName == staffName);
        //        }

        //        if (!string.IsNullOrWhiteSpace(month) &&
        //            DateTime.TryParse(month, out var selectedDate))
        //        {
        //            query = query.Where(x =>
        //                x.FeedbackDate.Date == selectedDate.Date);
        //        }

        //        var raw = await query
        //            .OrderByDescending(x => x.Id)
        //            .ToListAsync();

        //        var data = raw.Select(x => new StaffFeedbackDto
        //        {
        //            Id = x.Id,
        //            FeedbackId = x.FeedbackId,
        //            FeedbackQuestion = x.Feedback?.Question ?? "",
        //            StaffName = x.StaffName,
        //            Feedback = x.SelectedAnswer,
        //            FeedbackDate = x.FeedbackDate.ToString("yyyy-MM-dd")
        //        }).ToList();

        //        return Ok(data);
        //    }

        // GET /api/stafffeedback/staff-names
        [HttpGet("staff-names")]
        public async Task<IActionResult> GetStaffNames()
        {
            var names = await _db.StaffFeedbacks
                .Select(x => x.StaffName)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return Ok(names);
        }

        // POST /api/stafffeedback
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffFeedbackCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid payload" });

            // ── Validate ──────────────────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(dto.StaffName))
                return BadRequest(new { message = "StaffName is required." });

            if (string.IsNullOrWhiteSpace(dto.SelectedAnswer))
                return BadRequest(new { message = "SelectedAnswer is required." });

            bool isManual = dto.FeedbackId == 0 && !string.IsNullOrWhiteSpace(dto.CustomQuestion);

            if (!isManual && dto.FeedbackId <= 0)
                return BadRequest(new { message = "A valid FeedbackId or a CustomQuestion must be provided." });

            // ── Resolve FeedbackId ────────────────────────────────────────────────
            int resolvedFeedbackId = dto.FeedbackId;

            if (isManual)
            {
                // EF Core can translate EF.Functions or simple string comparison.
                // Use EF.Functions.Like for case-insensitive match (works on SQL Server & SQLite).
                var trimmed = dto.CustomQuestion!.Trim();

                var existing = await _db.Feedbacks
                    .FirstOrDefaultAsync(f => f.Question.ToUpper() == trimmed.ToUpper());

                if (existing != null)
                {
                    resolvedFeedbackId = existing.Id;
                }
                else
                {
                    // Insert new question into the master Feedbacks table
                    var newQuestion = new Feedback
                    {
                        Question = trimmed
                    };

                    _db.Feedbacks.Add(newQuestion);
                    await _db.SaveChangesAsync();          // get generated Id

                    resolvedFeedbackId = newQuestion.Id;
                }
            }

            // ── Insert StaffFeedback ──────────────────────────────────────────────
            var item = new StaffFeedback
            {
                FeedbackId = resolvedFeedbackId,
                StaffName = dto.StaffName.Trim(),
                SelectedAnswer = dto.SelectedAnswer.Trim(),
                FeedbackDate = DateTime.UtcNow
            };

            _db.StaffFeedbacks.Add(item);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Feedback submitted successfully", id = item.Id });
        }
    }

    //public class StaffFeedbackController : ControllerBase
    //{
    //    private readonly AppDbContext _db;

    //    public StaffFeedbackController(AppDbContext db)
    //    {
    //        _db = db;
    //    }

    //    // GET /api/stafffeedback?staffName=NIKAM&month=2026-04
    //    [HttpGet]
    //    public async Task<IActionResult> GetAll(
    //        [FromQuery] string? staffName,
    //        [FromQuery] string? month)
    //    {
    //        var query = _db.StaffFeedbacks
    //            .Include(x => x.Feedback)
    //            .AsQueryable();

    //        if (!string.IsNullOrWhiteSpace(staffName))
    //            query = query.Where(x => x.StaffName == staffName);

    //        if (!string.IsNullOrWhiteSpace(month) &&
    //            DateTime.TryParse(month + "-01", out var parsedMonth))
    //        {
    //            query = query.Where(x =>
    //                x.FeedbackDate.Year == parsedMonth.Year &&
    //                x.FeedbackDate.Month == parsedMonth.Month);
    //        }

    //        var data = await query
    //            .OrderByDescending(x => x.Id)
    //            .Select(x => new StaffFeedbackDto
    //            {
    //                Id = x.Id,
    //                FeedbackId = x.FeedbackId,
    //                FeedbackQuestion = x.Feedback.Question,
    //                StaffName = x.StaffName,
    //                Feedback = x.SelectedAnswer,
    //                FeedbackDate = x.FeedbackDate.ToString("yyyy-MM-dd")
    //            })
    //            .ToListAsync();

    //        return Ok(data);
    //    }

    //    // GET /api/stafffeedback/staff-names
    //    [HttpGet("staff-names")]
    //    public async Task<IActionResult> GetStaffNames()
    //    {
    //        var names = await _db.StaffFeedbacks
    //            .Select(x => x.StaffName)
    //            .Distinct()
    //            .OrderBy(x => x)
    //            .ToListAsync();

    //        return Ok(names);
    //    }

    //    // POST /api/stafffeedback
    //    [HttpPost]
    //    public async Task<IActionResult> Create([FromBody] StaffFeedbackCreateDto dto)
    //    {
    //        if (dto == null)
    //            return BadRequest(new { message = "Invalid payload" });

    //        var item = new StaffFeedback
    //        {
    //            FeedbackId = dto.FeedbackId,
    //            StaffName = dto.StaffName,
    //            SelectedAnswer = dto.SelectedAnswer,
    //            FeedbackDate = DateTime.Now
    //        };

    //        _db.StaffFeedbacks.Add(item);
    //        await _db.SaveChangesAsync();

    //        return Ok(new { message = "Feedback submitted successfully" });
    //    }
    //}

    [ApiController, Route("api/[controller]")]
    public class StaffShiftsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StaffShiftsController(AppDbContext db)
        {
            _db = db;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.StaffShifts
                .Include(s => s.Staff)
                .OrderByDescending(s => s.Id)
                .Select(s => new StaffShiftDto
                {
                    Id = s.Id,
                    StaffId = s.StaffId,
                    StaffName = s.Staff!.StaffName,
                    // ✅ Normalize to Title Case so frontend Select matches "Day"/"Night"
                    ShiftType = s.ShiftType != null
                        ? char.ToUpper(s.ShiftType[0]) + s.ShiftType.Substring(1).ToLower()
                        : "",
                    //StartDate = s.StartDate,
                    //EndDate = s.EndDate,
                    StartDate = s.StartDate.HasValue
    ? DateTime.SpecifyKind(s.StartDate.Value, DateTimeKind.Utc)
    : null,

                    EndDate = s.EndDate.HasValue
    ? DateTime.SpecifyKind(s.EndDate.Value, DateTimeKind.Utc)
    : null,
                    // ✅ HH:mm (24-hour) so browser <input type="time"> pre-fills correctly
                    StartTime = s.StartTime.HasValue
                        ? s.StartTime.Value.ToString(@"hh\:mm")
                        : "",
                    EndTime = s.EndTime.HasValue
                        ? s.EndTime.Value.ToString(@"hh\:mm")
                        : ""
                })
                .ToListAsync();

            return Ok(list);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffShiftCreateDto dto)
        {
            if (!TimeSpan.TryParse(dto.StartTime, out var st))
                return BadRequest("Invalid Start Time");

            if (!TimeSpan.TryParse(dto.EndTime, out var et))
                return BadRequest("Invalid End Time");

            var shift = new StaffShift
            {
                StaffId = dto.StaffId,
                ShiftType = dto.ShiftType,
                //StartDate = dto.StartDate,
                //EndDate = dto.EndDate,
                StartDate = dto.StartDate.HasValue
    ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc)
    : null,

                EndDate = dto.EndDate.HasValue
    ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc)
    : null,
                StartTime = st,
                EndTime = et
            };

            _db.StaffShifts.Add(shift);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Shift added successfully" });
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] StaffShiftCreateDto dto)
        {
            var shift = await _db.StaffShifts.FindAsync(id);
            if (shift == null)
                return NotFound();

            if (!TimeSpan.TryParse(dto.StartTime, out var st))
                return BadRequest("Invalid Start Time");

            if (!TimeSpan.TryParse(dto.EndTime, out var et))
                return BadRequest("Invalid End Time");

            shift.StaffId = dto.StaffId;
            shift.ShiftType = dto.ShiftType;
            shift.StartDate = dto.StartDate;
            shift.EndDate = dto.EndDate;
            shift.StartTime = st;
            shift.EndTime = et;

            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Shift updated successfully" });
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shift = await _db.StaffShifts.FindAsync(id);
            if (shift == null)
                return NotFound();

            _db.StaffShifts.Remove(shift);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Deleted successfully" });
        }
    }
    //public class StaffShiftsController : ControllerBase
    //{
    //    private readonly AppDbContext _db;

    //    public StaffShiftsController(AppDbContext db)
    //    {
    //        _db = db;
    //    }

    //    // ================= GET ALL =================
    //    [HttpGet]
    //    public async Task<IActionResult> GetAll()
    //    {
    //        var list = await _db.StaffShifts
    //            .Include(s => s.Staff)
    //            .OrderByDescending(s => s.Id)
    //            .Select(s => new StaffShiftDto
    //            {
    //                Id = s.Id,
    //                StaffId = s.StaffId,
    //                StaffName = s.Staff!.StaffName,
    //                ShiftType = s.ShiftType,
    //                StartDate = s.StartDate,
    //                EndDate = s.EndDate,
    //                StartTime = s.StartTime.HasValue
    //                    ? s.StartTime.Value.ToString(@"hh\:mm")
    //                    : "",
    //                EndTime = s.EndTime.HasValue
    //                    ? s.EndTime.Value.ToString(@"hh\:mm")
    //                    : ""
    //            })
    //            .ToListAsync();

    //        return Ok(list);
    //    }

    //    // ================= CREATE =================
    //    [HttpPost]
    //    public async Task<IActionResult> Create( [FromBody] StaffShiftCreateDto dto)
    //    {
    //        if (!TimeSpan.TryParse(dto.StartTime, out var st))
    //            return BadRequest("Invalid Start Time");

    //        if (!TimeSpan.TryParse(dto.EndTime, out var et))
    //            return BadRequest("Invalid End Time");

    //        var shift = new StaffShift
    //        {
    //            StaffId = dto.StaffId,
    //            ShiftType = dto.ShiftType,
    //            StartDate = dto.StartDate,
    //            EndDate = dto.EndDate,
    //            StartTime = st,
    //            EndTime = et
    //        };

    //        _db.StaffShifts.Add(shift);
    //        await _db.SaveChangesAsync();

    //        return Ok(new { success = true, message = "Shift added successfully" });
    //    }

    //    // ================= UPDATE =================
    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> Update(
    //        int id,
    //        [FromBody] StaffShiftCreateDto dto)
    //    {
    //        var shift = await _db.StaffShifts.FindAsync(id);
    //        if (shift == null)
    //            return NotFound();

    //        if (!TimeSpan.TryParse(dto.StartTime, out var st))
    //            return BadRequest("Invalid Start Time");

    //        if (!TimeSpan.TryParse(dto.EndTime, out var et))
    //            return BadRequest("Invalid End Time");

    //        shift.StaffId = dto.StaffId;
    //        shift.ShiftType = dto.ShiftType;
    //        shift.StartDate = dto.StartDate;
    //        shift.EndDate = dto.EndDate;
    //        shift.StartTime = st;
    //        shift.EndTime = et;

    //        await _db.SaveChangesAsync();

    //        return Ok(new { success = true, message = "Shift updated successfully" });
    //    }

    //    // ================= DELETE =================
    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var shift = await _db.StaffShifts.FindAsync(id);
    //        if (shift == null)
    //            return NotFound();

    //        _db.StaffShifts.Remove(shift);
    //        await _db.SaveChangesAsync();

    //        return Ok(new { success = true, message = "Deleted successfully" });
    //    }
    //}

    //public class ShiftsController : ControllerBase
    //{
    //    private readonly AppDbContext _db;

    //    public ShiftsController(AppDbContext db)
    //    {
    //        _db = db;
    //    }

    //    // ================= GET ALL =================

    //    [HttpGet]
    //    public async Task<IActionResult> GetAll()
    //    {
    //        var list = await _db.StaffShifts
    //            .Include(s => s.Staff)
    //            .OrderByDescending(s => s.Id)
    //            .Select(s => new StaffShiftDto
    //            {
    //                Id = s.Id,

    //                StaffId = s.StaffId,

    //                StaffName = s.Staff!.StaffName,

    //                ShiftType = s.ShiftType,

    //                StartDate = s.StartDate,

    //                EndDate = s.EndDate,

    //                StartTime = s.StartTime.HasValue
    //                    ? s.StartTime.Value.ToString(@"hh\:mm")
    //                    : "",

    //                EndTime = s.EndTime.HasValue
    //                    ? s.EndTime.Value.ToString(@"hh\:mm")
    //                    : ""
    //            })
    //            .ToListAsync();

    //        return Ok(list);
    //    }

    //    // ================= CREATE =================

    //    [HttpPost]
    //    public async Task<IActionResult> Create(
    //        [FromBody] StaffShiftCreateDto dto)
    //    {
    //        if (!TimeSpan.TryParse(dto.StartTime, out var st))
    //        {
    //            return BadRequest("Invalid Start Time");
    //        }

    //        if (!TimeSpan.TryParse(dto.EndTime, out var et))
    //        {
    //            return BadRequest("Invalid End Time");
    //        }

    //        var shift = new StaffShift
    //        {
    //            StaffId = dto.StaffId,

    //            ShiftType = dto.ShiftType,

    //            StartDate = dto.StartDate,

    //            EndDate = dto.EndDate,

    //            StartTime = st,

    //            EndTime = et
    //        };

    //        _db.StaffShifts.Add(shift);

    //        await _db.SaveChangesAsync();

    //        return Ok(new
    //        {
    //            success = true,
    //            message = "Shift added successfully"
    //        });
    //    }

    //    // ================= UPDATE =================

    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> Update(
    //        int id,
    //        [FromBody] StaffShiftCreateDto dto)
    //    {
    //        var shift = await _db.StaffShifts.FindAsync(id);

    //        if (shift == null)
    //        {
    //            return NotFound();
    //        }

    //        if (!TimeSpan.TryParse(dto.StartTime, out var st))
    //        {
    //            return BadRequest("Invalid Start Time");
    //        }

    //        if (!TimeSpan.TryParse(dto.EndTime, out var et))
    //        {
    //            return BadRequest("Invalid End Time");
    //        }

    //        shift.StaffId = dto.StaffId;

    //        shift.ShiftType = dto.ShiftType;

    //        shift.StartDate = dto.StartDate;

    //        shift.EndDate = dto.EndDate;

    //        shift.StartTime = st;

    //        shift.EndTime = et;

    //        await _db.SaveChangesAsync();

    //        return Ok(new
    //        {
    //            success = true,
    //            message = "Shift updated successfully"
    //        });
    //    }

    //    // ================= DELETE =================

    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var shift = await _db.StaffShifts.FindAsync(id);

    //        if (shift == null)
    //        {
    //            return NotFound();
    //        }

    //        _db.StaffShifts.Remove(shift);

    //        await _db.SaveChangesAsync();

    //        return Ok(new
    //        {
    //            success = true,
    //            message = "Deleted successfully"
    //        });
    //    }
    //}

    // ─── ATTENDANCE ───────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]
    public class StaffAttendanceController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StaffAttendanceController(AppDbContext db)
        {
            _db = db;
        }

        // GET /api/StaffAttendance?staff=John&month=2024-06-01
        [HttpGet]
        public async Task<IActionResult> GetAttendance(
            [FromQuery] string? staff,
            [FromQuery] DateTime? month)
        {
            var query = _db.Attendances
                .Include(x => x.Staff)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(staff))
            {
                query = query.Where(x =>
                    x.Staff != null &&
                    x.Staff.StaffName == staff);
            }

            if (month.HasValue)
            {
                query = query.Where(x =>
                    x.Date.Month == month.Value.Month &&
                    x.Date.Year == month.Value.Year);
            }

            var data = await query
                .OrderBy(x => x.Date)
                .Select(x => new StaffAttendanceDto
                {
                    Id = x.Id,
                    InDate = x.ClockIn.HasValue
                        ? x.ClockIn.Value.ToString("yyyy-MM-dd HH:mm")
                        : "",
                    OutDate = x.ClockOut.HasValue
                        ? x.ClockOut.Value.ToString("yyyy-MM-dd HH:mm")
                        : "",
                    TotalHrs = x.ClockIn.HasValue && x.ClockOut.HasValue
                        ? (x.ClockOut.Value - x.ClockIn.Value)
                            .TotalHours
                            .ToString("0.##")
                        : "0",
                    Status = x.Status
                })
                .ToListAsync();

            return Ok(data);
        }

        // POST /api/StaffAttendance
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateStaffAttendanceDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Request body is required.");

                if (dto.OutDate <= dto.InDate)
                    return BadRequest("Out date must be after in date.");

                var staff = await _db.Staff
                    .FirstOrDefaultAsync(x => x.Id == dto.StaffId);

                if (staff == null)
                    return BadRequest("Invalid Staff");
                var inDateUtc = dto.InDate.Kind == DateTimeKind.Unspecified
    ? DateTime.SpecifyKind(dto.InDate, DateTimeKind.Utc)
    : dto.InDate.ToUniversalTime();

                var outDateUtc = dto.OutDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(dto.OutDate, DateTimeKind.Utc)
                    : dto.OutDate.ToUniversalTime();

                var attendance = new Attendance
                {
                    StaffId = dto.StaffId,
                    //Date = dto.InDate.Date,
                    Date = inDateUtc.Date,
                    //ClockIn = dto.InDate,
                    //ClockOut = dto.OutDate,
                    ClockIn = inDateUtc,
                    ClockOut = outDateUtc,
                    Status = dto.Status
                };

                _db.Attendances.Add(attendance);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Attendance added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        // DELETE /api/StaffAttendance/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _db.Attendances.FindAsync(id);
            if (record == null)
                return NotFound("Attendance record not found.");

            _db.Attendances.Remove(record);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Attendance deleted successfully" });
        }

        // PUT /api/StaffAttendance/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] CreateStaffAttendanceDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Request body is required.");

                if (dto.OutDate <= dto.InDate)
                    return BadRequest("Out date must be after in date.");

                var attendance = await _db.Attendances
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (attendance == null)
                    return NotFound("Attendance record not found.");

                var staff = await _db.Staff
                    .FirstOrDefaultAsync(x => x.Id == dto.StaffId);

                if (staff == null)
                    return BadRequest("Invalid Staff");

                var inDateUtc = dto.InDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(dto.InDate, DateTimeKind.Utc)
                    : dto.InDate.ToUniversalTime();

                var outDateUtc = dto.OutDate.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(dto.OutDate, DateTimeKind.Utc)
                    : dto.OutDate.ToUniversalTime();

                attendance.StaffId = dto.StaffId;
                attendance.Date = inDateUtc.Date;
                attendance.ClockIn = inDateUtc;
                attendance.ClockOut = outDateUtc;
                attendance.Status = dto.Status;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Attendance updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
    }




    // ─── LEAVES ───────────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]
    public class LeavesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public LeavesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? staffId,
            [FromQuery] string? status)
        {
            var q = _db.Leaves
                .Include(l => l.Staff)
                .AsQueryable();

            if (staffId.HasValue)
                q = q.Where(l => l.StaffId == staffId);

            if (!string.IsNullOrEmpty(status))
                q = q.Where(l => l.Status == status);

            var list = await q
                .OrderByDescending(l => l.StartDate)
                .Select(l => new LeaveDto
                {
                    Id = l.Id,
                    StaffId = l.StaffId,
                    StaffName = l.Staff!.StaffName,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Reason = l.Reason ?? "",   // "Subject" in UI
                    Remarks = l.Remarks ?? "", // "Others" in UI
                    Status = l.Status
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid payload" });

            var staffExists = await _db.Staff.AnyAsync(s => s.Id == dto.StaffId);
            if (!staffExists)
                return BadRequest(new { message = "Invalid Staff ID" });

            if (dto.EndDate < dto.StartDate)
                return BadRequest(new { message = "End date cannot be before start date" });

            var leave = new Leave
            {
                StaffId = dto.StaffId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Remarks = dto.Remarks,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.Leaves.Add(leave);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Leave submitted successfully", id = leave.Id });
        }

        // FIXED: Changed [FromBody] string status → [FromBody] StatusUpdateDto
        // Root cause of 500: ASP.NET Core cannot bind a bare JSON string
        // to [FromBody] string. Frontend sends { "status": "Approved" }
        // which requires an object wrapper DTO.
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] StatusUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest(new { message = "Status is required" });

            var allowed = new[] { "Pending", "Approved", "Rejected" };
            if (!allowed.Contains(dto.Status))
                return BadRequest(new { message = "Invalid status value" });

            var leave = await _db.Leaves.FindAsync(id);
            if (leave == null) return NotFound();

            leave.Status = dto.Status;
            await _db.SaveChangesAsync();
            return Ok(new { message = $"Leave {dto.Status.ToLower()} successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var leave = await _db.Leaves.FindAsync(id);
            if (leave == null) return NotFound();
            _db.Leaves.Remove(leave);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Deleted successfully" });
        }
    }


    // ─── TASKS ────────────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        // IWebHostEnvironment is injected so we can resolve the wwwroot
        // path for saving uploaded attachments.
        public TasksController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ----------------------------------------------------------
        // GET api/tasks
        // Returns all tasks ordered newest-first.
        // ----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _db.Tasks
                .Include(x => x.AssignedTo)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new TaskDto
                {
                    Id = x.Id,
                    AssignedToStaffId = x.AssignedToStaffId,
                    AssignedToName = x.AssignedTo != null ? x.AssignedTo.StaffName : "",
                    Details = x.Details,
                    //StartDate = x.StartDate,
                    //EndDate = x.EndDate,
                    StartDate = x.StartDate.HasValue
    ? DateTime.SpecifyKind(x.StartDate.Value, DateTimeKind.Utc)
    : null,

                    EndDate = x.EndDate.HasValue
    ? DateTime.SpecifyKind(x.EndDate.Value, DateTimeKind.Utc)
    : null,
                    Time = x.Time,
                    Status = x.Status,
                    Priority = x.Priority,
                    Department = x.Department,
                    Category = x.Category,
                    Every = x.Every,
                    AttachmentPath = x.AttachmentPath,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // ----------------------------------------------------------
        // GET api/tasks/staff
        // FIX: New endpoint. The original code had no way for the UI
        // to fetch the staff list — it was hardcoded on the frontend.
        // ----------------------------------------------------------
        //[HttpGet("staff")]
        //public async Task<IActionResult> GetStaff()
        //{
        //    var staff = await _db.Staff
        //        .OrderBy(s => s.StaffName)
        //        .Select(s => new StaffDto
        //        {
        //            Id = s.Id,
        //            StaffName = s.StaffName,
        //            //Department = s.Department
        //        })
        //        .ToListAsync();

        //    return Ok(staff);
        //}

        // ----------------------------------------------------------
        // POST api/tasks/bulk
        // FIX: New endpoint. The original single-create endpoint could
        // not handle multi-staff selection. This endpoint accepts a
        // list of staff IDs and creates one task row per staff member
        // in one round-trip, matching the UI's multi-select behaviour.
        // Uses [FromForm] because we also accept an optional file.
        // ----------------------------------------------------------
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate([FromForm] TaskBulkCreateDto dto)
        {
            if (dto.AssignedToStaffIds == null || dto.AssignedToStaffIds.Count == 0)
                return BadRequest("At least one staff member must be selected.");

            if (string.IsNullOrWhiteSpace(dto.Details))
                return BadRequest("Task details are required.");

            // Save attachment once and reuse the path across all created tasks.
            string? attachmentPath = await SaveAttachmentAsync(dto.Attachment);

            var newTasks = dto.AssignedToStaffIds.Select(staffId => new TaskItem
            {
                AssignedToStaffId = staffId,
                Details = dto.Details,
                //StartDate = dto.StartDate,
                //EndDate = dto.EndDate,
                StartDate = dto.StartDate.HasValue
    ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc)
    : null,

                EndDate = dto.EndDate.HasValue
    ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc)
    : null,
                Time = dto.Time,
                // FIX: Status always starts as "Pending" on the server;
                // the original backend required the client to set it.
                Status = "Pending",
                Priority = dto.Priority,
                Department = dto.Department,
                Category = dto.Category,
                Every = dto.Every,
                AttachmentPath = attachmentPath,
                // FIX: CreatedAt was never set server-side in the original code.
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _db.Tasks.AddRange(newTasks);
            await _db.SaveChangesAsync();

            // Return the created task IDs so the UI can refresh.
            return Ok(newTasks.Select(t => t.Id).ToList());
        }

        // ----------------------------------------------------------
        // POST api/tasks  (single create — kept for API completeness)
        // FIX: Added [FromForm] to support the optional file upload.
        //      Added server-side Status default and CreatedAt stamp.
        // ----------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] TaskCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Details))
                return BadRequest("Task details are required.");

            string? attachmentPath = await SaveAttachmentAsync(dto.Attachment);

            var task = new TaskItem
            {
                AssignedToStaffId = dto.AssignedToStaffId,
                Details = dto.Details,
                //StartDate = dto.StartDate,
                //EndDate = dto.EndDate,
                StartDate = dto.StartDate.HasValue
    ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc)
    : null,

                EndDate = dto.EndDate.HasValue
    ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc)
    : null,
                Time = dto.Time,
                Status = "Pending",           // FIX: always default server-side
                Priority = dto.Priority,
                Department = dto.Department,
                Category = dto.Category,
                Every = dto.Every,
                AttachmentPath = attachmentPath,
                CreatedAt = DateTime.UtcNow   // FIX: was never set in original
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return Ok(task);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] TaskUpdateDto dto)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            task.AssignedToStaffId = dto.AssignedToStaffId;
            task.Details = dto.Details;
            task.StartDate = dto.StartDate;
            task.EndDate = dto.EndDate;
            task.Time = dto.Time;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.Department = dto.Department;
            task.Category = dto.Category;
            task.Every = dto.Every;

            // FIX: Original Update never touched AttachmentPath.
            // If a new file is provided, save it and replace the old path.
            if (dto.Attachment != null)
            {
                // Delete old file if it exists
                if (!string.IsNullOrEmpty(task.AttachmentPath))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, task.AttachmentPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }
                task.AttachmentPath = await SaveAttachmentAsync(dto.Attachment);
            }

            await _db.SaveChangesAsync();
            return Ok(task);
        }

        // ----------------------------------------------------------
        // DELETE api/tasks/{id}
        // No changes needed from original — kept intact.
        // ----------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _db.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            // Clean up attachment file from disk when task is deleted
            if (!string.IsNullOrEmpty(task.AttachmentPath))
            {
                var filePath = Path.Combine(_env.WebRootPath, task.AttachmentPath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // ----------------------------------------------------------
        // Private helper: saves an uploaded IFormFile to wwwroot/attachments
        // and returns the relative web path, or null if no file provided.
        // ----------------------------------------------------------
        private async Task<string?> SaveAttachmentAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var folder = Path.Combine(_env.WebRootPath, "attachments");
            Directory.CreateDirectory(folder);

            // Use a GUID prefix to avoid filename collisions
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/attachments/{fileName}";
        }
    }

    // ─── DSI ──────────────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]

    public class DSIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public DSIController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ── GET /api/DSI ────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? staffId,
            [FromQuery] DateTime? date)
        {
            var q = _db.DSIRecords
                .Include(d => d.Staff)
                .AsQueryable();

            if (staffId.HasValue)
                q = q.Where(d => d.StaffId == staffId.Value);
            if (date.HasValue)
                q = q.Where(d => d.RecordDate.Date == date.Value.Date);

            var list = await q
                .OrderByDescending(d => d.RecordDate)
                .Select(d => new DSIDto
                {
                    Id = d.Id,
                    StaffId = d.StaffId,
                    StaffName = d.Staff != null ? d.Staff.StaffName : "",
                    Department = d.DepartmentId.HasValue ? d.DepartmentId.Value.ToString() : "",
                    Problem = d.Problem,
                    Priority = d.Priority,
                    Message = d.Message,
                    IsSolved = d.IsSolved,
                    DocumentPath = d.DocumentPath,
                    RecordDate = d.RecordDate.ToString("dd/MMM/yyyy")
                })
                .ToListAsync();

            return Ok(list);
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
    [FromForm] DSICreateDto dto,
    IFormFile? document)
        {
            try
            {
                if (dto.StaffId <= 0)
                    return BadRequest("StaffId is required.");

                if (string.IsNullOrWhiteSpace(dto.Problem))
                    return BadRequest("Problem is required.");

                // Parse IsSolved
                var isSolved = dto.IsSolved?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

                string? docPath = null;

                if (document != null && document.Length > 0)
                {
                    var webRoot = _env.WebRootPath
                        ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                    var uploads = Path.Combine(webRoot, "uploads", "dsi");
                    Directory.CreateDirectory(uploads);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(document.FileName)}";
                    var filePath = Path.Combine(uploads, fileName);

                    using var stream = System.IO.File.Create(filePath);
                    await document.CopyToAsync(stream);

                    docPath = $"/uploads/dsi/{fileName}";
                }

                var record = new DSIRecord
                {
                    StaffId = dto.StaffId,
                    DepartmentId = dto.DepartmentId,
                    Problem = dto.Problem,
                    Priority = dto.Priority,
                    Message = dto.Message ?? "",
                    IsSolved = isSolved,
                    DocumentPath = docPath,
                    //RecordDate = dto.RecordDate == default ? DateTime.Today : dto.RecordDate
                    RecordDate = dto.RecordDate == default
    ? DateTime.UtcNow
    : dto.RecordDate.ToUniversalTime()
                };

                _db.DSIRecords.Add(record);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "DSI record added successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
        // ── POST /api/DSI ───────────────────────────────────────────────────────
        //[HttpPost]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> Create(
        //    [FromForm] DSICreateDto dto,
        //    IFormFile? document)
        //{
        //    if (dto.StaffId <= 0)
        //        return BadRequest("StaffId is required.");
        //    if (string.IsNullOrWhiteSpace(dto.Problem))
        //        return BadRequest("Problem is required.");

        //    // FIX: parse IsSolved from string to avoid multipart bool-binding issues
        //    var isSolved = dto.IsSolved?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

        //    // FIX: guard WebRootPath null — happens when wwwroot folder doesn't exist
        //    string? docPath = null;
        //    if (document != null && document.Length > 0)
        //    {
        //        var webRoot = _env.WebRootPath
        //            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        //        var uploads = Path.Combine(webRoot, "uploads", "dsi");
        //        Directory.CreateDirectory(uploads);
        //        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(document.FileName)}";
        //        var filePath = Path.Combine(uploads, fileName);
        //        using var stream = System.IO.File.Create(filePath);
        //        await document.CopyToAsync(stream);
        //        docPath = $"/uploads/dsi/{fileName}";
        //    }

        //    var record = new DSIRecord
        //    {
        //        StaffId = dto.StaffId,
        //        DepartmentId = dto.DepartmentId,
        //        Problem = dto.Problem,
        //        Priority = dto.Priority,
        //        Message = dto.Message ?? "",
        //        IsSolved = isSolved,
        //        DocumentPath = docPath,
        //        RecordDate = dto.RecordDate == default ? DateTime.Today : dto.RecordDate
        //    };

        //    _db.DSIRecords.Add(record);
        //    await _db.SaveChangesAsync();
        //    return Ok(new { message = "DSI record added successfully." });
        //}

        // ── PUT /api/DSI/{id} ───────────────────────────────────────────────────
        // PUT /api/DSI/{id}
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] DSICreateDto dto,
            IFormFile? document)
        {
            try
            {
                var record = await _db.DSIRecords
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (record == null)
                    return NotFound("DSI record not found.");

                if (dto.StaffId <= 0)
                    return BadRequest("StaffId is required.");

                if (string.IsNullOrWhiteSpace(dto.Problem))
                    return BadRequest("Problem is required.");

                var isSolved = dto.IsSolved?.Equals(
                    "true",
                    StringComparison.OrdinalIgnoreCase) ?? false;

                // Upload new document if provided
                if (document != null && document.Length > 0)
                {
                    var webRoot = _env.WebRootPath
                        ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                    var uploads = Path.Combine(webRoot, "uploads", "dsi");
                    Directory.CreateDirectory(uploads);

                    var fileName =
                        $"{Guid.NewGuid()}{Path.GetExtension(document.FileName)}";

                    var filePath = Path.Combine(uploads, fileName);

                    using var stream = System.IO.File.Create(filePath);
                    await document.CopyToAsync(stream);

                    record.DocumentPath = $"/uploads/dsi/{fileName}";
                }

                DateTime recordDate = dto.RecordDate == default
                    ? record.RecordDate
                    : dto.RecordDate;

                if (recordDate.Kind == DateTimeKind.Unspecified)
                {
                    recordDate = DateTime.SpecifyKind(
                        recordDate,
                        DateTimeKind.Utc);
                }
                else
                {
                    recordDate = recordDate.ToUniversalTime();
                }

                record.StaffId = dto.StaffId;
                record.DepartmentId = dto.DepartmentId;
                record.Problem = dto.Problem;
                record.Priority = dto.Priority;
                record.Message = dto.Message ?? "";
                record.IsSolved = isSolved;
                record.RecordDate = recordDate;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "DSI record updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
        //[HttpPut("{id}")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> Update(
        //    int id,
        //    [FromForm] DSICreateDto dto,
        //    IFormFile? document)
        //{
        //    var record = await _db.DSIRecords.FindAsync(id);
        //    if (record == null) return NotFound();

        //    // FIX: parse IsSolved from string to avoid multipart bool-binding issues
        //    var isSolved = dto.IsSolved?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;

        //    // FIX: guard WebRootPath null
        //    if (document != null && document.Length > 0)
        //    {
        //        var webRoot = _env.WebRootPath
        //            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        //        var uploads = Path.Combine(webRoot, "uploads", "dsi");
        //        Directory.CreateDirectory(uploads);
        //        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(document.FileName)}";
        //        var filePath = Path.Combine(uploads, fileName);
        //        using var stream = System.IO.File.Create(filePath);
        //        await document.CopyToAsync(stream);
        //        record.DocumentPath = $"/uploads/dsi/{fileName}";
        //    }

        //    record.StaffId = dto.StaffId;
        //    record.DepartmentId = dto.DepartmentId;
        //    record.Problem = dto.Problem;
        //    record.Priority = dto.Priority;
        //    record.Message = dto.Message ?? "";
        //    record.IsSolved = isSolved;
        //    record.RecordDate = dto.RecordDate == default ? record.RecordDate : dto.RecordDate;

        //    await _db.SaveChangesAsync();
        //    return Ok(new { message = "DSI record updated successfully." });
        //}

        // ── DELETE /api/DSI/{id} ────────────────────────────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _db.DSIRecords.FindAsync(id);
            if (record == null) return NotFound();
            _db.DSIRecords.Remove(record);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Deleted successfully." });
        }

        // ── GET /api/DSI/export — returns CSV ──────────────────────────────────
        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            var list = await _db.DSIRecords
                .Include(d => d.Staff)
                .OrderByDescending(d => d.RecordDate)
                .ToListAsync();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Staff Name,Department,Priority,Problem,Message,Status,Date");

            foreach (var d in list)
            {
                sb.AppendLine(
                    $"\"{d.Staff?.StaffName}\"," +
                    $"\"{d.DepartmentId}\"," +
                    $"{d.Priority}," +
                    $"\"{d.Problem}\"," +
                    $"\"{d.Message}\"," +
                    $"{(d.IsSolved ? "Solved" : "Unsolved")}," +
                    $"{d.RecordDate:dd/MMM/yyyy}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "DSI_Export.csv");
        }
    }

    // ─── MOTIVATIONAL MESSAGES ────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]
    public class MotivationalController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public MotivationalController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET /api/Motivational
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.MotivationalMessages
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MotivationalMessageDto
                {
                    Id = m.Id,
                    Message = m.Message,
                    Author = m.Author,
                    VideoLink = m.VideoLink,
                    ImagePath = m.ImagePath,
                    Date = m.Date.ToString("dd/MMM/yyyy"),
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET /api/Motivational/random
        [HttpGet("random")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRandom()
        {
            var msgs = await _db.MotivationalMessages
                .Where(m => m.IsActive)
                .ToListAsync();

            if (!msgs.Any()) return NotFound();

            var rnd = msgs[new Random().Next(msgs.Count)];

            return Ok(new MotivationalMessageDto
            {
                Id = rnd.Id,
                Message = rnd.Message,
                Author = rnd.Author,
                VideoLink = rnd.VideoLink,
                ImagePath = rnd.ImagePath,
                Date = rnd.Date.ToString("dd/MMM/yyyy"),
                IsActive = rnd.IsActive
            });
        }

        // POST /api/Motivational  (multipart/form-data to support image upload)
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] MotivationalMessageCreateDto dto,
                                                 IFormFile? image)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Message))
                    return BadRequest("Message is required.");

                string? imagePath = null;

                if (image != null && image.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads", "motivational");
                    Directory.CreateDirectory(uploads);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(uploads, fileName);
                    using var stream = System.IO.File.Create(filePath);
                    await image.CopyToAsync(stream);
                    imagePath = $"/uploads/motivational/{fileName}";
                }
                DateTime dateValue = dto.Date == default
    ? DateTime.UtcNow
    : dto.Date;

                if (dateValue.Kind == DateTimeKind.Unspecified)
                {
                    dateValue = DateTime.SpecifyKind(dateValue, DateTimeKind.Utc);
                }
                else
                {
                    dateValue = dateValue.ToUniversalTime();
                }
                var msg = new MotivationalMessage
                {
                    Message = dto.Message,
                    Author = dto.Author,
                    VideoLink = dto.VideoLink ?? "",
                    ImagePath = imagePath,
                    //Date = dto.Date == default ? DateTime.Today : dto.Date,
                    Date = dto.Date == default
        ? DateTime.UtcNow
        : DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                    IsActive = dto.IsActive,
                    //CreatedAt = DateTime.UtcNow
                    CreatedAt = DateTime.UtcNow
                };

                _db.MotivationalMessages.Add(msg);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Saved and sent to all staff." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
        // PUT /api/Motivational/{id}
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] MotivationalMessageCreateDto dto,
            IFormFile? image)
        {
            try
            {
                var msg = await _db.MotivationalMessages
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (msg == null)
                    return NotFound("Record not found.");

                if (string.IsNullOrWhiteSpace(dto.Message))
                    return BadRequest("Message is required.");

                // Upload new image if provided
                if (image != null && image.Length > 0)
                {
                    var uploads = Path.Combine(
                        _env.WebRootPath,
                        "uploads",
                        "motivational");

                    Directory.CreateDirectory(uploads);

                    var fileName =
                        $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

                    var filePath = Path.Combine(uploads, fileName);

                    using var stream = System.IO.File.Create(filePath);
                    await image.CopyToAsync(stream);

                    msg.ImagePath = $"/uploads/motivational/{fileName}";
                }

                DateTime dateValue = dto.Date == default
                    ? msg.Date
                    : dto.Date;

                if (dateValue.Kind == DateTimeKind.Unspecified)
                {
                    dateValue = DateTime.SpecifyKind(
                        dateValue,
                        DateTimeKind.Utc);
                }
                else
                {
                    dateValue = dateValue.ToUniversalTime();
                }

                msg.Message = dto.Message;
                msg.Author = dto.Author;
                msg.VideoLink = dto.VideoLink ?? "";
                msg.Date = dateValue;
                msg.IsActive = dto.IsActive;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Motivational message updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
        // PUT /api/Motivational/{id}
        //[HttpPut("{id}")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> Update(int id,
        //                                         [FromForm] MotivationalMessageCreateDto dto,
        //                                         IFormFile? image)
        //{
        //    try
        //    {
        //        var msg = await _db.MotivationalMessages.FindAsync(id);
        //        if (msg == null) return NotFound();

        //        if (image != null && image.Length > 0)
        //        {
        //            var uploads = Path.Combine(_env.WebRootPath, "uploads", "motivational");
        //            Directory.CreateDirectory(uploads);
        //            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        //            var filePath = Path.Combine(uploads, fileName);
        //            using var stream = System.IO.File.Create(filePath);
        //            await image.CopyToAsync(stream);
        //            msg.ImagePath = $"/uploads/motivational/{fileName}";
        //        }

        //        msg.Message = dto.Message;
        //        msg.Author = dto.Author;
        //        msg.VideoLink = dto.VideoLink ?? "";
        //        msg.Date = dto.Date == default ? msg.Date : dto.Date;
        //        msg.IsActive = dto.IsActive;

        //        await _db.SaveChangesAsync();
        //        return Ok(new { message = "Updated successfully." });
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        // DELETE /api/Motivational/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var msg = await _db.MotivationalMessages.FindAsync(id);
            if (msg == null) return NotFound();

            _db.MotivationalMessages.Remove(msg);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Deleted successfully." });
        }
    }
    // ─── HR POLICY ────────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]
    public class StaffPolicyController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StaffPolicyController(AppDbContext db)
        {
            _db = db;
        }

        // ── Map helper (mirrors StaffController pattern) ──────────────────────
        private static StaffPolicyDto Map(StaffPolicy p) => new StaffPolicyDto
        {
            Id = p.Id,
            PolicyName = p.PolicyName,
            Department = p.Department,
            GoogleDriveLink = p.GoogleDriveLink,
            IsActive = p.IsActive,
            UpdatedAt = p.UpdatedAt
        };

        // ── GET ALL  api/staffpolicy?search=xyz ───────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            try
            {
                var query = _db.StaffPolicies
                    .Where(p => p.IsActive)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(p =>
                        p.PolicyName.Contains(search) ||
                        p.Department.Contains(search));

                var list = await query
                    .OrderBy(p => p.PolicyName)
                    .ToListAsync();

                return Ok(new { success = true, data = list.Select(Map) });
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

        // ── GET BY ID  api/staffpolicy/{id} ───────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var policy = await _db.StaffPolicies
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

                if (policy == null)
                    return NotFound(new { success = false, message = "Policy not found" });

                return Ok(new { success = true, data = Map(policy) });
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

        // ── CREATE  POST api/staffpolicy ──────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StaffPolicyCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid input", errors = ModelState });

                var policy = new StaffPolicy
                {
                    PolicyName = dto.PolicyName,
                    Department = dto.Department,
                    GoogleDriveLink = dto.GoogleDriveLink,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.StaffPolicies.Add(policy);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Staff policy created successfully",
                    data = Map(policy)
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

        // ── UPDATE  PUT api/staffpolicy/{id} ──────────────────────────────────
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StaffPolicyCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid input", errors = ModelState });

                var policy = await _db.StaffPolicies.FindAsync(id);

                if (policy == null || !policy.IsActive)
                    return NotFound(new { success = false, message = "Policy not found" });

                policy.PolicyName = dto.PolicyName;
                policy.Department = dto.Department;
                policy.GoogleDriveLink = dto.GoogleDriveLink;
                policy.IsActive = dto.IsActive;
                policy.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Staff policy updated successfully",
                    data = Map(policy)
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

        // ── SOFT DELETE  DELETE api/staffpolicy/{id} ──────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var policy = await _db.StaffPolicies.FindAsync(id);

                if (policy == null || !policy.IsActive)
                    return NotFound(new { success = false, message = "Policy not found" });

                policy.IsActive = false;          // soft delete — mirrors StaffController
                policy.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Staff policy deleted successfully" });
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

        // ── BULK UPLOAD  POST api/staffpolicy/bulk-upload ─────────────────────
        [HttpPost("bulk-upload")]
        public async Task<IActionResult> BulkUpload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "No file uploaded." });

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".xlsx" && ext != ".csv")
                    return BadRequest(new { success = false, message = "Only .xlsx or .csv files are supported." });

                var policies = new List<StaffPolicy>();

                if (ext == ".xlsx")
                {
                    // ── XLSX via ClosedXML (same dependency used in StaffController) ──
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using var workbook = new XLWorkbook(stream);
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1); // skip header

                    if (rows != null)
                    {
                        foreach (var row in rows)
                        {
                            var policyName = row.Cell(1).GetValue<string>().Trim();
                            var department = row.Cell(2).GetValue<string>().Trim();
                            var driveLink = row.Cell(3).GetValue<string>().Trim();

                            if (string.IsNullOrWhiteSpace(policyName)) continue;

                            policies.Add(new StaffPolicy
                            {
                                PolicyName = policyName,
                                Department = department,
                                GoogleDriveLink = driveLink,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }
                else
                {
                    // ── CSV fallback ──────────────────────────────────────────
                    using var reader = new StreamReader(file.OpenReadStream());
                    bool isHeader = true;
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (isHeader) { isHeader = false; continue; }
                        var cols = line.Split(',');
                        if (cols.Length < 3) continue;
                        policies.Add(new StaffPolicy
                        {
                            PolicyName = cols[0].Trim(),
                            Department = cols[1].Trim(),
                            GoogleDriveLink = cols[2].Trim(),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                if (policies.Count == 0)
                    return BadRequest(new { success = false, message = "No valid rows found in file." });

                _db.StaffPolicies.AddRange(policies);
                await _db.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"{policies.Count} policies imported successfully.",
                    imported = policies.Count
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

        // ── EXPORT EXCEL  GET api/staffpolicy/export/excel ────────────────────
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                var list = await _db.StaffPolicies
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.PolicyName)
                    .ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("StaffPolicies");

                // Header row
                var headers = new[] { "ID", "Policy Name", "Department", "Google Drive Link", "Updated At" };
                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cell(1, i + 1).Value = headers[i];

                // Data rows
                for (int i = 0; i < list.Count; i++)
                {
                    var p = list[i];
                    int row = i + 2;
                    worksheet.Cell(row, 1).Value = p.Id;
                    worksheet.Cell(row, 2).Value = p.PolicyName;
                    worksheet.Cell(row, 3).Value = p.Department;
                    worksheet.Cell(row, 4).Value = p.GoogleDriveLink;
                    worksheet.Cell(row, 5).Value = p.UpdatedAt.ToString("yyyy-MM-dd HH:mm");
                }

                worksheet.Columns().AdjustToContents();

                using var ms = new MemoryStream();
                workbook.SaveAs(ms);

                return File(
                    ms.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "StaffPolicies.xlsx"
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


    [ApiController, Route("api/[controller]")]
    public class HRPolicyController : ControllerBase
    {
        private readonly AppDbContext _db;
        public HRPolicyController(AppDbContext db) => _db = db;

        // GET api/hrpolicy
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.HRPolicies
                .OrderBy(p => p.PolicyName)
                .Select(p => new HRPolicyDto
                {
                    Id = p.Id,
                    PolicyName = p.PolicyName,
                    Department = p.Department,
                    GoogleDriveLink = p.GoogleDriveLink,
                    IsActive = p.IsActive,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
            return Ok(list);
        }

        // POST api/hrpolicy
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HRPolicyCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var policy = new HRPolicy
            {
                PolicyName = dto.PolicyName,
                Department = dto.Department,
                GoogleDriveLink = dto.GoogleDriveLink,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.HRPolicies.Add(policy);
            await _db.SaveChangesAsync();
            return Ok(new HRPolicyDto
            {
                Id = policy.Id,
                PolicyName = policy.PolicyName,
                Department = policy.Department,
                GoogleDriveLink = policy.GoogleDriveLink,
                IsActive = policy.IsActive,
                UpdatedAt = policy.UpdatedAt
            });
        }

        // PUT api/hrpolicy/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HRPolicyCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var policy = await _db.HRPolicies.FindAsync(id);
            if (policy == null) return NotFound();

            policy.PolicyName = dto.PolicyName;
            policy.Department = dto.Department;
            policy.GoogleDriveLink = dto.GoogleDriveLink;
            policy.IsActive = dto.IsActive;
            policy.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(new HRPolicyDto
            {
                Id = policy.Id,
                PolicyName = policy.PolicyName,
                Department = policy.Department,
                GoogleDriveLink = policy.GoogleDriveLink,
                IsActive = policy.IsActive,
                UpdatedAt = policy.UpdatedAt
            });
        }

        // DELETE api/hrpolicy/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var policy = await _db.HRPolicies.FindAsync(id);
            if (policy == null) return NotFound();
            _db.HRPolicies.Remove(policy);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // POST api/hrpolicy/bulk-upload
        [HttpPost("bulk-upload")]
        public async Task<IActionResult> BulkUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedExtensions = new[] { ".csv", ".xlsx" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Only .csv or .xlsx files are supported.");

            // CSV parsing example — swap for EPPlus/ClosedXML for xlsx
            using var reader = new StreamReader(file.OpenReadStream());
            var policies = new List<HRPolicy>();
            string? line;
            bool isHeader = true;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (isHeader) { isHeader = false; continue; } // skip header row
                var cols = line.Split(',');
                if (cols.Length < 3) continue;
                policies.Add(new HRPolicy
                {
                    PolicyName = cols[0].Trim(),
                    Department = cols[1].Trim(),
                    GoogleDriveLink = cols[2].Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (policies.Count == 0) return BadRequest("No valid rows found in file.");
            _db.HRPolicies.AddRange(policies);
            await _db.SaveChangesAsync();
            return Ok(new { imported = policies.Count });
        }
    }
    // ─── DASHBOARD ────────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]

    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var today = DateTime.UtcNow.Date;
            var last7Days = today.AddDays(-7);
            var last30Days = today.AddDays(-30);

            // Total Active Staff
            var totalStaff = await _db.Staff
                .CountAsync(s => s.IsActive);

            // Present Today
            var presentToday = await _db.Attendances
                .CountAsync(a =>
                    a.Date.Date == today &&
                    a.Status == "Present");

            // Clocked Out / Absent
            var clockedOut = await _db.Attendances
                .CountAsync(a =>
                    a.Date.Date == today &&
                    a.Status == "Absent");

            // Leaves Today
            var leavesToday = await _db.Leaves
                .CountAsync(l =>
                    l.StartDate.Date <= today &&
                    l.EndDate.Date >= today &&
                    l.Status == "Approved");

            //// Tasks Due Today
            //var tasksDue = await _db.Tasks
            //    .CountAsync(t =>
            //        t.DueDate.HasValue &&
            //        t.DueDate.Value.Date == today &&
            //        t.Status != "Done");
            // Tasks Due Today
            var tasksDue = await _db.Tasks
                .CountAsync(t =>
                    t.EndDate.HasValue &&
                    t.EndDate.Value.Date == today &&
                    t.Status != "Done");

            // Pending Reviews
            var pendingReviews = await _db.Performances
                .CountAsync();

            // New Feedbacks
            var newFeedbacks = await _db.Feedbacks
                .CountAsync(f =>
                    f.CreatedAt.Date >= last7Days);

            // DSI Average Today
            //var dsiToday = await _db.DSIRecords
            //    .Where(d => d.RecordDate.Date == today)
            //    .Select(d => d.Score)
            //    .ToListAsync();
            var dsiScore = await _db.DSIRecords
    .CountAsync(d => d.RecordDate.Date == today);

            //var dsiScore = dsiToday.Any()
            //    ? Math.Round(dsiToday.Average(), 1)
            //    : 0;

            // Leaderboard Top 5
            var leaderboard = await _db.DSIRecords
                .Where(d => d.RecordDate >= last30Days)
                .Include(d => d.Staff)
                    .ThenInclude(s => s.Department)
                .GroupBy(d => new
                {
                    d.StaffId,
                    StaffName = d.Staff.StaffName,
                    Department = d.Staff.Department.DepartmentName
                })
                .Select(g => new LeaderboardEntry
                {
                    StaffName = g.Key.StaffName,
                    Department = g.Key.Department,
                    Score = g.Count()
                    //Score = (int)g.Average(x => x.Score)
                })
                .OrderByDescending(x => x.Score)
                .Take(5)
                .ToListAsync();

            // Birthdays
            var birthdays = await _db.Staff
                .Where(s =>
                    s.IsActive &&
                    s.DateOfBirth.HasValue &&
                    s.DateOfBirth.Value.Month == today.Month)
                .Select(s => new BirthdayEntry
                {
                    StaffName = s.StaffName,
                    DateOfBirth = s.DateOfBirth.Value
                })
                .ToListAsync();

            var response = new DashboardStatsDto
            {
                TotalStaff = totalStaff,
                PresentToday = presentToday,
                ClockedOut = clockedOut,
                LeavesToday = leavesToday,
                TasksDue = tasksDue,
                PendingReviews = pendingReviews,
                NewFeedbacks = newFeedbacks,
                DSIScore = dsiScore,
                Leaderboard = leaderboard,
                Birthdays = birthdays
            };

            return Ok(response);
        }
    }
}

