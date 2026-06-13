using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElevateERP.API.Data;
using ElevateERP.API.DTOs;
using ElevateERP.API.Models;
using System.ComponentModel.DataAnnotations;

namespace ElevateERP.API.Controllers
{
    // ─── REWARDS ──────────────────────────────────────────────────────────────
    [ApiController]
    [Route("api/[controller]")]
    public class RewardsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RewardsController(AppDbContext db) => _db = db;

        // Parse "2026-06-13" or "2026-06-13T00:00:00Z" safely as UTC
        private static DateTime ParseDateUtc(string raw)
        {
            if (DateTime.TryParse(
                    raw,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind,
                    out var dt))
            {
                return dt.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
                    : dt.ToUniversalTime();
            }
            return DateTime.UtcNow;
        }

        // ── GET /api/Rewards/staff-rewards ──────────────────────────────────────
        [HttpGet("staff-rewards")]
        public async Task<IActionResult> GetStaffRewards([FromQuery] int? staffId)
        {
            var q = _db.StaffRewards
                       .Include(sr => sr.Staff)
                       .AsQueryable();

            if (staffId.HasValue)
                q = q.Where(sr => sr.StaffId == staffId.Value);

            var list = await q
                .OrderByDescending(sr => sr.AwardedAt)
                .Select(sr => new StaffRewardDto
                {
                    Id = sr.Id,
                    StaffId = sr.StaffId,
                    StaffName = sr.Staff != null ? sr.Staff.StaffName : "",
                    PointsValue = sr.PointsValue,
                    Notes = sr.Notes,
                    AwardedAt = sr.AwardedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        // ── POST /api/Rewards/staff-rewards ─────────────────────────────────────
        [HttpPost("staff-rewards")]
        public async Task<IActionResult> AssignReward([FromBody] StaffRewardCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Request body is required.");

            var sr = new StaffReward
            {
                StaffId = dto.StaffId,
                PointsValue = dto.PointsValue,
                Notes = dto.Notes ?? "",
                AwardedAt = ParseDateUtc(dto.AwardedAt)
            };

            _db.StaffRewards.Add(sr);
            await _db.SaveChangesAsync();

            return Ok(new { sr.Id });
        }

        // ── PUT /api/Rewards/staff-rewards/{id} ─────────────────────────────────
        [HttpPut("staff-rewards/{id}")]
        public async Task<IActionResult> UpdateStaffReward(int id, [FromBody] StaffRewardCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Request body is required.");

            var sr = await _db.StaffRewards.FindAsync(id);
            if (sr == null)
                return NotFound($"StaffReward with id {id} not found.");

            sr.StaffId = dto.StaffId;
            sr.PointsValue = dto.PointsValue;
            sr.Notes = dto.Notes ?? "";
            sr.AwardedAt = ParseDateUtc(dto.AwardedAt);

            await _db.SaveChangesAsync();
            return Ok();
        }

        // ── DELETE /api/Rewards/staff-rewards/{id} ──────────────────────────────
        [HttpDelete("staff-rewards/{id}")]
        public async Task<IActionResult> DeleteStaffReward(int id)
        {
            var sr = await _db.StaffRewards.FindAsync(id);
            if (sr == null)
                return NotFound($"StaffReward with id {id} not found.");

            _db.StaffRewards.Remove(sr);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }

    // ─── PERFORMANCE ──────────────────────────────────────────────────────────
    //[ApiController, Route("api/[controller]")]
    //[ApiController]
    //[Route("api/[controller]")]
    [ApiController, Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PerformanceController(AppDbContext db)
        {
            _db = db;
        }

        // ================= GET =================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Performances
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(list);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PerformanceCreateDto dto
        )
        {
            var performance = new Performance
            {
                Name = dto.Name,
                Marks = dto.Marks
            };

            _db.Performances.Add(performance);

            await _db.SaveChangesAsync();

            return Ok(performance);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] PerformanceCreateDto dto
        )
        {
            var performance =
                await _db.Performances.FindAsync(id);

            if (performance == null)
                return NotFound();

            performance.Name = dto.Name;

            performance.Marks = dto.Marks;

            await _db.SaveChangesAsync();

            return Ok(performance);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var performance =
                await _db.Performances.FindAsync(id);

            if (performance == null)
                return NotFound();

            _db.Performances.Remove(performance);

            await _db.SaveChangesAsync();

            return Ok();
        }
    }
    //public class PerformanceController : ControllerBase
    //{
    //    private readonly AppDbContext _db;
    //    public PerformanceController(AppDbContext db) => _db = db;

    //    [HttpGet]
    //    public async Task<IActionResult> GetAll([FromQuery] int? staffId)
    //    {
    //        var q = _db.Performances.Include(p => p.Staff).AsQueryable();
    //        if (staffId.HasValue) q = q.Where(p => p.StaffId == staffId);
    //        var list = await q.OrderByDescending(p => p.ReviewDate)
    //            .Select(p => new PerformanceDto
    //            {
    //                Id = p.Id, StaffId = p.StaffId, StaffName = p.Staff!.StaffName,
    //                Score = p.Score, Notes = p.Notes, ReviewDate = p.ReviewDate
    //            }).ToListAsync();
    //        return Ok(list);
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Create([FromBody] PerformanceCreateDto dto)
    //    {
    //        var p = new Performance { StaffId = dto.StaffId, Score = dto.Score, Notes = dto.Notes, ReviewDate = dto.ReviewDate };
    //        _db.Performances.Add(p); await _db.SaveChangesAsync(); return Ok();
    //    }

    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> Update(int id, [FromBody] PerformanceCreateDto dto)
    //    {
    //        var p = await _db.Performances.FindAsync(id);
    //        if (p == null) return NotFound();
    //        p.Score = dto.Score; p.Notes = dto.Notes; p.ReviewDate = dto.ReviewDate;
    //        await _db.SaveChangesAsync(); return Ok();
    //    }

    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var p = await _db.Performances.FindAsync(id);
    //        if (p == null) return NotFound();
    //        _db.Performances.Remove(p); await _db.SaveChangesAsync(); return Ok();
    //    }
    //}

    // ─── FEEDBACK ─────────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]
  
    public class FeedbackController : ControllerBase
    {
        private readonly AppDbContext _db;
        public FeedbackController(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Feedbacks
                .OrderByDescending(x => x.Id)
                .Select(x => new FeedbackDto
                {
                    Id = x.Id,
                    Question = x.Question,
                    Option1 = x.Option1,
                    Option2 = x.Option2,
                    Option3 = x.Option3,
                    Option4 = x.Option4
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _db.Feedbacks.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(new FeedbackDto
            {
                Id = item.Id,
                Question = item.Question,
                Option1 = item.Option1,
                Option2 = item.Option2,
                Option3 = item.Option3,
                Option4 = item.Option4
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeedbackCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var feedback = new Feedback
            {
                Question = dto.Question,
                Option1 = dto.Option1,
                Option2 = dto.Option2,
                Option3 = dto.Option3,
                Option4 = dto.Option4
            };
            _db.Feedbacks.Add(feedback);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Feedback added successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FeedbackCreateDto dto)
        {
            var feedback = await _db.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound(new { message = "Feedback not found" });
            feedback.Question = dto.Question;
            feedback.Option1 = dto.Option1;
            feedback.Option2 = dto.Option2;
            feedback.Option3 = dto.Option3;
            feedback.Option4 = dto.Option4;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Feedback updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _db.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound(new { message = "Feedback not found" });
            _db.Feedbacks.Remove(feedback);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Feedback deleted successfully" });
        }
    }

    
    // ===================== AppDbContext additions =====================
    // Add to your existing AppDbContext:
    // public DbSet<StaffFeedbackResponse> StaffFeedbackResponses { get; set; }
    //public class FeedbacksController : ControllerBase
    //{
    //    private readonly AppDbContext _db;
    //    public FeedbacksController(AppDbContext db) => _db = db;

    //    [HttpGet]
    //    public async Task<IActionResult> GetAll([FromQuery] int? staffId, [FromQuery] string? type)
    //    {
    //        var q = _db.Feedbacks.Include(f => f.Staff).AsQueryable();
    //        if (staffId.HasValue) q = q.Where(f => f.StaffId == staffId);
    //        if (!string.IsNullOrEmpty(type)) q = q.Where(f => f.FeedbackType == type);
    //        var list = await q.OrderByDescending(f => f.CreatedAt)
    //            .Select(f => new FeedbackDto
    //            {
    //                Id = f.Id, StaffId = f.StaffId, StaffName = f.Staff!.StaffName,
    //                FeedbackType = f.FeedbackType, Message = f.Message, CreatedAt = f.CreatedAt
    //            }).ToListAsync();
    //        return Ok(list);
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Create([FromBody] FeedbackCreateDto dto)
    //    {
    //        var fb = new Feedback { StaffId = dto.StaffId, FeedbackType = dto.FeedbackType, Message = dto.Message };
    //        _db.Feedbacks.Add(fb); await _db.SaveChangesAsync(); return Ok();
    //    }

    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var fb = await _db.Feedbacks.FindAsync(id);
    //        if (fb == null) return NotFound();
    //        _db.Feedbacks.Remove(fb); await _db.SaveChangesAsync(); return Ok();
    //    }
    //}

    // ─── SURPRISE ─────────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]
    // ── Controller ────────────────────────────────────────────

    //[ApiController]
    //[Route("api/[controller]")]
    //public class SurpriseRewardsController : ControllerBase
    //{
    //    private readonly AppDbContext _db;

    //    public SurpriseRewardsController(AppDbContext db)
    //    {
    //        _db = db;
    //    }

    //    // ================= GET ALL (optional filter by staffId) =================
    //    [HttpGet]
    //    public async Task<IActionResult> GetAll([FromQuery] int? staffId)
    //    {
    //        var q = _db.Surprises
    //            .Include(s => s.Staff)
    //            .AsQueryable();

    //        if (staffId.HasValue)
    //            q = q.Where(s => s.StaffId == staffId.Value);

    //        var list = await q
    //            .OrderByDescending(s => s.SurpriseDate)
    //            .Select(s => new SurpriseDto
    //            {
    //                Id = s.Id,
    //                StaffId = s.StaffId,
    //                StaffName = s.Staff!.StaffName,
    //                RewardName = s.Title,
    //                RewardFor = s.Description,
    //                Marks = s.Marks,
    //                SurpriseDate = s.SurpriseDate
    //            })
    //            .ToListAsync();

    //        return Ok(list);
    //    }

    //    // ================= CREATE =================
    //    //[HttpPost]
    //    //public async Task<IActionResult> Create([FromBody] SurpriseCreateDto dto)
    //    //{
    //    //    // Validate staffId exists in DB
    //    //    var staffExists = await _db.Staff.AnyAsync(s => s.Id == dto.StaffId);
    //    //    if (!staffExists)
    //    //        return BadRequest(new { message = "Staff not found." });

    //    //    var surprise = new Surprise
    //    //    {
    //    //        StaffId = dto.StaffId,
    //    //        Title = dto.RewardName,       // maps rewardName → Title
    //    //        Description = dto.RewardFor,        // maps rewardFor  → Description
    //    //        Marks = dto.Marks,
    //    //        SurpriseDate = DateTime.Now
    //    //    };

    //    //    _db.Surprises.Add(surprise);
    //    //    await _db.SaveChangesAsync();

    //    //    return Ok(new { message = "Reward given successfully.", id = surprise.Id });
    //    //}
    //    [HttpPost]
    //    public async Task<IActionResult> Create([FromBody] SurpriseCreateDto dto)
    //    {
    //        try
    //        {
    //            Console.WriteLine($"StaffId={dto.StaffId}, RewardName={dto.RewardName}, RewardFor={dto.RewardFor}, Marks={dto.Marks}");

    //            var staffExists = await _db.Staff.AnyAsync(s => s.Id == dto.StaffId);
    //            if (!staffExists)
    //                return BadRequest(new { message = "Staff not found." });

    //            var surprise = new Surprise
    //            {
    //                StaffId = dto.StaffId,
    //                Title = dto.RewardName,
    //                Description = dto.RewardFor,
    //                Marks = dto.Marks,
    //                SurpriseDate = DateTime.Now
    //            };

    //            _db.Surprises.Add(surprise);
    //            await _db.SaveChangesAsync();

    //            return Ok(new { message = "Reward given successfully.", id = surprise.Id });
    //        }
    //        catch (Exception ex)
    //        {
    //            // This will show exact error in browser network tab
    //            return StatusCode(500, new
    //            {
    //                message = ex.Message,
    //                inner = ex.InnerException?.Message,
    //                detail = ex.ToString()
    //            });
    //        }
    //    }

    //    // ================= DELETE =================
    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var surprise = await _db.Surprises.FindAsync(id);
    //        if (surprise == null)
    //            return NotFound(new { message = "Reward not found." });

    //        _db.Surprises.Remove(surprise);
    //        await _db.SaveChangesAsync();

    //        return Ok(new { message = "Reward deleted." });
    //    }
    //}
    //[ApiController]
    //[Route("api/[controller]")]
    public class SurpriseRewardsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SurpriseRewardsController(AppDbContext db)
        {
            _db = db;
        }

        // ================= GET =================
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? staffId)
        {
            var q = _db.Surprises
                .Include(s => s.Staff)
                .AsQueryable();

            if (staffId.HasValue)
                q = q.Where(s => s.StaffId == staffId.Value);

            var list = await q
                .OrderByDescending(s => s.SurpriseDate)
                .Select(s => new SurpriseDto
                {
                    Id = s.Id,
                    StaffId = s.StaffId,
                    StaffName = s.Staff!.StaffName,
                    RewardName = s.Title,
                    RewardFor = s.Description,
                    Marks = s.Marks,
                    SurpriseDate = s.SurpriseDate
                })
                .ToListAsync();

            return Ok(list);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SurpriseRewardCreateDto dto)
        {
            try
            {
                var staffExists = await _db.Staff.AnyAsync(s => s.Id == dto.StaffId);
                if (!staffExists)
                    return BadRequest(new { message = "Staff not found." });

                var surprise = new Surprise
                {
                    StaffId = dto.StaffId,
                    Title = dto.RewardName,
                    Description = dto.RewardFor,
                    Marks = dto.Marks,
                    SurpriseDate = DateTime.UtcNow
                };

                _db.Surprises.Add(surprise);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Reward given successfully.", id = surprise.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var surprise = await _db.Surprises.FindAsync(id);
            if (surprise == null)
                return NotFound(new { message = "Reward not found." });

            _db.Surprises.Remove(surprise);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Reward deleted." });
        }
    }

    // ================= DTOs =================
    public class SurpriseRewardCreateDto
    {
        public int StaffId { get; set; }
        public string RewardName { get; set; } = string.Empty;
        public string RewardFor { get; set; } = string.Empty;
        public int Marks { get; set; }
    }

    public class SurpriseDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public string RewardName { get; set; } = string.Empty;
        public string RewardFor { get; set; } = string.Empty;
        public int Marks { get; set; }
        public DateTime SurpriseDate { get; set; }
    }
    //public class SurpriseRewardsController : ControllerBase
    //{
    //    private readonly AppDbContext _db;

    //    public SurpriseRewardsController(AppDbContext db)
    //    {
    //        _db = db;
    //    }

    //    // ================= GET =================
    //    [HttpGet]
    //    public async Task<IActionResult> GetAll(
    //        [FromQuery] int? staffId
    //    )
    //    {
    //        var q = _db.Surprises
    //            .Include(s => s.Staff)
    //            .AsQueryable();

    //        if (staffId.HasValue)
    //        {
    //            q = q.Where(s => s.StaffId == staffId);
    //        }

    //        var list = await q
    //            .OrderByDescending(s => s.SurpriseDate)
    //            .Select(s => new SurpriseDto
    //            {
    //                Id = s.Id,
    //                StaffId = s.StaffId,
    //                StaffName = s.Staff!.StaffName,

    //                RewardName = s.Title,
    //                RewardFor = s.Description,

    //                Marks = s.Marks,

    //                SurpriseDate = s.SurpriseDate
    //            })
    //            .ToListAsync();

    //        return Ok(list);
    //    }

    //    // ================= CREATE =================
    //    [HttpPost]
    //    public async Task<IActionResult> Create(
    //        [FromBody] SurpriseCreateDto dto
    //    )
    //    {
    //        var surprise = new Surprise
    //        {
    //            StaffId = dto.StaffId,

    //            Title = dto.RewardName,
    //            Description = dto.RewardFor,

    //            Marks = dto.Marks,

    //            SurpriseDate = DateTime.Now
    //        };

    //        _db.Surprises.Add(surprise);

    //        await _db.SaveChangesAsync();

    //        return Ok();
    //    }

    //    // ================= DELETE =================
    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var surprise = await _db.Surprises.FindAsync(id);

    //        if (surprise == null)
    //        {
    //            return NotFound();
    //        }

    //        _db.Surprises.Remove(surprise);

    //        await _db.SaveChangesAsync();

    //        return Ok();
    //    }
    //}


    //public class SurpriseRewardsController : ControllerBase
    //{
    //    private readonly AppDbContext _db;

    //    public SurpriseRewardsController(AppDbContext db)
    //    {
    //        _db = db;
    //    }

    //    // ================= GET ALL =================
    //    [HttpGet]
    //    public async Task<IActionResult> GetAll([FromQuery] int? staffId)
    //    {
    //        var query = _db.Surprises
    //            .Include(s => s.Staff)
    //            .AsQueryable();

    //        if (staffId.HasValue)
    //        {
    //            query = query.Where(x => x.StaffId == staffId);
    //        }

    //        var list = await query
    //            .OrderByDescending(x => x.SurpriseDate)
    //            .Select(x => new
    //            {
    //                id = x.Id,
    //                staffId = x.StaffId,
    //                staffName = x.Staff!.StaffName,

    //                // UI FORMAT
    //                rewardFor = x.Description,
    //                rewardName = x.Title,
    //                marks = x.Marks,

    //                surpriseDate = x.SurpriseDate
    //            })
    //            .ToListAsync();

    //        return Ok(list);
    //    }

    //    // ================= CREATE =================
    //    [HttpPost]
    //    public async Task<IActionResult> Create([FromBody] SurpriseRewardCreateDto dto)
    //    {
    //        if (dto == null)
    //        {
    //            return BadRequest("Invalid data");
    //        }

    //        var surprise = new Surprise
    //        {
    //            StaffId = dto.StaffId,

    //            // UI -> DB Mapping
    //            Title = dto.RewardName,
    //            Description = dto.RewardFor,
    //            Marks = dto.Marks,

    //            SurpriseDate = DateTime.Now
    //        };

    //        _db.Surprises.Add(surprise);

    //        await _db.SaveChangesAsync();

    //        return Ok(new
    //        {
    //            message = "Reward added successfully"
    //        });
    //    }

    //    // ================= UPDATE =================
    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> Update(
    //        int id,
    //        [FromBody] SurpriseRewardCreateDto dto
    //    )
    //    {
    //        var surprise = await _db.Surprises.FindAsync(id);

    //        if (surprise == null)
    //        {
    //            return NotFound();
    //        }

    //        surprise.StaffId = dto.StaffId;

    //        // UI -> DB Mapping
    //        surprise.Title = dto.RewardName;
    //        surprise.Description = dto.RewardFor;
    //        surprise.Marks = dto.Marks;

    //        await _db.SaveChangesAsync();

    //        return Ok(new
    //        {
    //            message = "Reward updated successfully"
    //        });
    //    }

    //    // ================= DELETE =================
    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var surprise = await _db.Surprises.FindAsync(id);

    //        if (surprise == null)
    //        {
    //            return NotFound();
    //        }

    //        _db.Surprises.Remove(surprise);

    //        await _db.SaveChangesAsync();

    //        return Ok(new
    //        {
    //            message = "Reward deleted successfully"
    //        });
    //    }
    //}

    // ================= DTO =================
    //public class SurpriseRewardCreateDto
    //{
    //    public int StaffId { get; set; }

    //    // UI FIELD
    //    public string RewardFor { get; set; } = string.Empty;

    //    // UI FIELD
    //    public string RewardName { get; set; } = string.Empty;

    //    // UI FIELD
    //    public int Marks { get; set; }
    //}
    //public class SurpriseRewardCreateDto
    //{
    //    public int StaffId { get; set; }

    //    public string RewardName { get; set; } = string.Empty;

    //    public string RewardFor { get; set; } = string.Empty;

    //    public int Marks { get; set; }
    //}
    //public class SurprisesController : ControllerBase
    //{
    //    private readonly AppDbContext _db;
    //    public SurprisesController(AppDbContext db) => _db = db;

    //    [HttpGet]
    //    public async Task<IActionResult> GetAll([FromQuery] int? staffId)
    //    {
    //        var q = _db.Surprises.Include(s => s.Staff).AsQueryable();
    //        if (staffId.HasValue) q = q.Where(s => s.StaffId == staffId);
    //        var list = await q.OrderByDescending(s => s.SurpriseDate)
    //            .Select(s => new SurpriseDto
    //            {
    //                Id = s.Id, StaffId = s.StaffId, StaffName = s.Staff!.StaffName,
    //                Title = s.Title, Description = s.Description, SurpriseDate = s.SurpriseDate
    //            }).ToListAsync();
    //        return Ok(list);
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Create([FromBody] SurpriseCreateDto dto)
    //    {
    //        var s = new Surprise { StaffId = dto.StaffId, Title = dto.Title, Description = dto.Description, SurpriseDate = dto.SurpriseDate };
    //        _db.Surprises.Add(s); await _db.SaveChangesAsync(); return Ok();
    //    }

    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var s = await _db.Surprises.FindAsync(id);
    //        if (s == null) return NotFound();
    //        _db.Surprises.Remove(s); await _db.SaveChangesAsync(); return Ok();
    //        _db.Surprises.Remove(s); await _db.SaveChangesAsync(); return Ok();
    //    }
    //}

    // ─── FOLLOW UP ────────────────────────────────────────────────────────────
    [ApiController, Route("api/[controller]")]
    public class FollowUpsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public FollowUpsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? staffId, [FromQuery] string? status)
        {
            var q = _db.FollowUps.Include(f => f.Staff).AsQueryable();
            if (staffId.HasValue) q = q.Where(f => f.StaffId == staffId);
            if (!string.IsNullOrEmpty(status)) q = q.Where(f => f.Status == status);
            var list = await q.OrderByDescending(f => f.FollowUpDate)
                .Select(f => new FollowUpDto
                {
                    Id = f.Id, StaffId = f.StaffId, StaffName = f.Staff!.StaffName,
                    Title = f.Title, Description = f.Description,
                    FollowUpDate = f.FollowUpDate, Status = f.Status
                }).ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FollowUpCreateDto dto)
        {
            var fu = new FollowUp { StaffId = dto.StaffId, Title = dto.Title, Description = dto.Description, FollowUpDate = dto.FollowUpDate, Status = dto.Status ?? "Pending" };
            _db.FollowUps.Add(fu); await _db.SaveChangesAsync(); return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FollowUpCreateDto dto)
        {
            var fu = await _db.FollowUps.FindAsync(id);
            if (fu == null) return NotFound();
            fu.Title = dto.Title; fu.Description = dto.Description;
            fu.FollowUpDate = dto.FollowUpDate; fu.Status = dto.Status ?? fu.Status;
            await _db.SaveChangesAsync(); return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fu = await _db.FollowUps.FindAsync(id);
            if (fu == null) return NotFound();
            _db.FollowUps.Remove(fu); await _db.SaveChangesAsync(); return Ok();
        }
    }
}
