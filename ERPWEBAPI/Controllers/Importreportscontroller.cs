
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ElevateERP.API.Data;
using ElevateERP.API.DTOs;
using ElevateERP.API.Models;
using System.Globalization;

namespace ERPWEBAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportReportsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Folder inside wwwroot where uploaded files are stored
        private const string UploadFolder = "uploads/reports";

        public ImportReportsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/ImportReports
        // Returns list of all import reports
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportReportDto>>> GetAll()
        {
            var reports = await _context.ImportReports
                .OrderByDescending(r => r.ReportDate)
                .Select(r => new ImportReportDto
                {
                    Id = r.Id,
                    ReportName = r.ReportName,
                    DriveLink = r.DriveLink,
                    ReportDate = r.ReportDate.ToString("dd/MMM/yyyy"),
                    FilePath = r.FilePath
                })
                .ToListAsync();

            return Ok(reports);
        }

        // ─────────────────────────────────────────────────────────────────────
        // POST /api/ImportReports
        // Accepts multipart/form-data: DriveLink, ReportDate, File
        // ─────────────────────────────────────────────────────────────────────

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ImportReportDto>> Create(
    [FromForm] string driveLink,
    [FromForm] string reportDate,
    IFormFile? file)
        {
            try
            {
                // Validate Report Date
                if (string.IsNullOrWhiteSpace(reportDate))
                {
                    return BadRequest(new
                    {
                        message = "Report date is required."
                    });
                }

                // Parse yyyy-MM-dd coming from HTML input type=date
                if (!DateTime.TryParseExact(
                        reportDate,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime parsedDate))
                {
                    return BadRequest(new
                    {
                        message = "Invalid date format. Expected yyyy-MM-dd."
                    });
                }

                // IMPORTANT FOR POSTGRESQL timestamp with time zone
                parsedDate = DateTime.SpecifyKind(
                    parsedDate,
                    DateTimeKind.Utc);

                string savedFilePath = string.Empty;
                string originalFileName = string.Empty;

                // File Upload
                if (file != null && file.Length > 0)
                {
                    originalFileName = Path.GetFileName(file.FileName);

                    string uploadDir = Path.Combine(
                        _env.WebRootPath ?? "wwwroot",
                        UploadFolder);

                    Directory.CreateDirectory(uploadDir);

                    string uniqueFileName =
                        $"{Guid.NewGuid()}_{originalFileName}";

                    string fullPath =
                        Path.Combine(uploadDir, uniqueFileName);

                    using (var stream = new FileStream(
                        fullPath,
                        FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedFilePath =
                        $"{UploadFolder}/{uniqueFileName}";
                }

                var report = new ImportReport
                {
                    ReportName = originalFileName,
                    DriveLink = driveLink ?? string.Empty,

                    // FIXED
                    ReportDate = parsedDate,

                    FilePath = savedFilePath,

                    // UTC
                    CreatedAt = DateTime.UtcNow
                };

                _context.ImportReports.Add(report);

                await _context.SaveChangesAsync();

                return Ok(new ImportReportDto
                {
                    Id = report.Id,
                    ReportName = report.ReportName,
                    DriveLink = report.DriveLink,
                    ReportDate = report.ReportDate.ToString("dd/MMM/yyyy"),
                    FilePath = report.FilePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }
        //[HttpPost]
        //[Consumes("multipart/form-data")]
        //public async Task<ActionResult<ImportReportDto>> Create(
        //    [FromForm] string driveLink,
        //    [FromForm] string reportDate,
        //    IFormFile? file)
        //{
        //    // Validate required fields
        //    if (string.IsNullOrWhiteSpace(reportDate))
        //        return BadRequest(new { message = "Report date is required." });

        //    if (!DateTime.TryParse(reportDate, out DateTime parsedDate))
        //        return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd." });

        //    string savedFilePath = string.Empty;
        //    string originalFileName = string.Empty;

        //    // Handle optional file upload
        //    if (file != null && file.Length > 0)
        //    {
        //        originalFileName = Path.GetFileName(file.FileName);

        //        // Ensure upload folder exists under wwwroot
        //        string uploadDir = Path.Combine(_env.WebRootPath ?? "wwwroot", UploadFolder);
        //        Directory.CreateDirectory(uploadDir);

        //        // Generate unique filename to avoid collisions
        //        string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
        //        string fullPath = Path.Combine(uploadDir, uniqueFileName);

        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        // Store relative path for retrieval
        //        savedFilePath = $"{UploadFolder}/{uniqueFileName}";
        //    }

        //    var report = new ImportReport
        //    {
        //        ReportName = originalFileName,
        //        DriveLink = driveLink ?? string.Empty,
        //        ReportDate = parsedDate,
        //        FilePath = savedFilePath,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.ImportReports.Add(report);
        //    await _context.SaveChangesAsync();

        //    return Ok(new ImportReportDto
        //    {
        //        Id = report.Id,
        //        ReportName = report.ReportName,
        //        DriveLink = report.DriveLink,
        //        ReportDate = report.ReportDate.ToString("dd/MMM/yyyy"),
        //        FilePath = report.FilePath
        //    });
        //}

        // ─────────────────────────────────────────────────────────────────────
        // GET /api/ImportReports/{id}/download
        // Downloads the physical file associated with this report
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var report = await _context.ImportReports.FindAsync(id);
            if (report == null)
                return NotFound(new { message = "Report not found." });

            if (string.IsNullOrWhiteSpace(report.FilePath))
                return BadRequest(new { message = "No file is attached to this report." });

            string fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", report.FilePath);

            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "Physical file not found on server." });

            // Determine content type
            string contentType = report.ReportName.EndsWith(".xlsx")
                ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                : "application/octet-stream";

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, contentType, report.ReportName);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE /api/ImportReports/{id}
        // Deletes the record and the physical file from disk
        // ─────────────────────────────────────────────────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.ImportReports.FindAsync(id);
            if (report == null)
                return NotFound(new { message = "Report not found." });

            // Delete physical file if it exists
            if (!string.IsNullOrWhiteSpace(report.FilePath))
            {
                string fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", report.FilePath);
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _context.ImportReports.Remove(report);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Report deleted successfully." });
        }
    }
}