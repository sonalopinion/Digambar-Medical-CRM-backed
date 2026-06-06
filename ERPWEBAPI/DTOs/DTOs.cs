using System.ComponentModel.DataAnnotations;

namespace ElevateERP.API.DTOs
{
    // ─── AUTH ─────────────────────────────────────────────────────────────────
    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string Token, string Username, string Role);

    // ─── FIRM ─────────────────────────────────────────────────────────────────
    public class FirmDto
    {
        public int Id { get; set; }
        public string FirmName { get; set; } = "";
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
    public class FirmCreateDto
    {
        public string FirmName { get; set; } = "";
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
    // DTO returned to the client for listing reports
    public class ImportReportDto
    {
        public int Id { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public string DriveLink { get; set; } = string.Empty;
        public string ReportDate { get; set; } = string.Empty;  // "dd/MMM/yyyy" formatted
        public string FilePath { get; set; } = string.Empty;    // relative server path
    }

    // DTO used when creating a new report (multipart/form-data)
    public class ImportReportCreateDto
    {
        public string DriveLink { get; set; } = string.Empty;
        public string ReportDate { get; set; } = string.Empty;  // "yyyy-MM-dd" from date input
        // IFormFile is bound separately in the controller via [FromForm]
    }

    // ─── DEPARTMENT ───────────────────────────────────────────────────────────
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; } = "";
        public string? Description { get; set; }
    }
    public class DepartmentCreateDto
    {
        public string DepartmentName { get; set; } = "";
        public string? Description { get; set; }
    }

    // ─── CATEGORY ─────────────────────────────────────────────────────────────
    public class CategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = "";
        public string? Description { get; set; }
    }
    public class CategoryCreateDto
    {
        public string CategoryName { get; set; } = "";
        public string? Description { get; set; }
    }

    // ─── STAFF ────────────────────────────────────────────────────────────────
    public class StaffDto
    {
        public int Id { get; set; }

        public int FirmId { get; set; }
        public string? FirmName { get; set; }

        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public string? Role { get; set; }

        public string? StaffName { get; set; }

        public string? MobileNumber { get; set; }

        public string? EmailId { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Address { get; set; }

        public DateTime? JoiningDate { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Education { get; set; }

        public string? Skill { get; set; }

        public string? AdharNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? AccountNumber { get; set; }

        public string? InsuranceDetails { get; set; }

        public DateTime? InsuranceStart { get; set; }

        public DateTime? InsuranceEnd { get; set; }
    }
    public class StaffCreateDto
    {
        public int FirmId { get; set; }

        public int DepartmentId { get; set; }

        public string? StaffName { get; set; }

        public string? Role { get; set; }

        public string? MobileNumber { get; set; }

        public string? EmailId { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Address { get; set; }

        public DateTime? JoiningDate { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Education { get; set; }

        public string? Skill { get; set; }

        public string? AdharNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? AccountNumber { get; set; }

        public string? InsuranceDetails { get; set; }

        public DateTime? InsuranceStart { get; set; }

        public DateTime? InsuranceEnd { get; set; }
    }
    // ─── REWARD ───────────────────────────────────────────────────────────────
    public class RewardDto
    {
        public int Id { get; set; }
        public string RewardName { get; set; } = "";
        public string? Description { get; set; }
        public decimal? PointsValue { get; set; }
    }
    public class RewardCreateDto
    {
        public string RewardName { get; set; } = "";
        public string? Description { get; set; }
        public decimal? PointsValue { get; set; }
    }
    public class StaffRewardCreateDto
    {
        public int StaffId { get; set; }
        public int RewardId { get; set; }
        public string? Notes { get; set; }
    }
    public class StaffRewardDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string? StaffName { get; set; }
        public int RewardId { get; set; }
        public string? RewardName { get; set; }
        public string? Notes { get; set; }
        public DateTime AwardedAt { get; set; }
    }

    // ─── PERFORMANCE ──────────────────────────────────────────────────────────
    public class PerformanceDto
    {

        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Marks { get; set; }
        //public int      StaffId    { get; set; }
        //public string?  StaffName  { get; set; }
        //public int      Score      { get; set; }
        //public string?  Notes      { get; set; }
        //public DateTime ReviewDate { get; set; }
    }
    public class PerformanceCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public string Marks { get; set; } = string.Empty;
        //public int      StaffId    { get; set; }
        //public int      Score      { get; set; }
        //public string?  Notes      { get; set; }
        //public DateTime ReviewDate { get; set; }
    }

    // ─── FEEDBACK ─────────────────────────────────────────────────────────────
    public class FeedbackDto
    {
        public int Id { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Option1 { get; set; } = string.Empty;

        public string Option2 { get; set; } = string.Empty;

        public string Option3 { get; set; } = string.Empty;

        public string Option4 { get; set; } = string.Empty;
    }
    public class FeedbackCreateDto
    {
        public string Question { get; set; } = string.Empty;

        public string Option1 { get; set; } = string.Empty;

        public string Option2 { get; set; } = string.Empty;

        public string Option3 { get; set; } = string.Empty;

        public string Option4 { get; set; } = string.Empty;
    }
    public class StaffFeedbackCreateDto
    {
        public string StaffName { get; set; } = string.Empty;
        public int FeedbackId { get; set; }               // 0 when custom question
        public string SelectedAnswer { get; set; } = string.Empty;
        public string? CustomQuestion { get; set; }        // filled when typing manually
    }

    public class StaffFeedbackDto
    {
        public int Id { get; set; }
        public int FeedbackId { get; set; }
        public string FeedbackQuestion { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
        public string FeedbackDate { get; set; } = string.Empty;
    }

    // ─── SURPRISE ─────────────────────────────────────────────────────────────
    //public class SurpriseDto
    //{
    //    public int      Id           { get; set; }
    //    public int      StaffId      { get; set; }
    //    public string?  StaffName    { get; set; }
    //    public string?  Title        { get; set; }
    //    public string?  Description  { get; set; }
    //    public DateTime SurpriseDate { get; set; }
    //}
    // ── DTOs ──────────────────────────────────────────────────

    public class SurpriseCreateDto
    {
        [Required]
        public int StaffId { get; set; }          // frontend sends string → int parse handled by model binding

        [Required]
        public string RewardName { get; set; } = string.Empty;   // reward.label  e.g. "EXCELLENT"

        [Required]
        public string RewardFor { get; set; } = string.Empty;    // reason text

        public int Marks { get; set; }            // reward.marks  e.g. 10
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
    //public class SurpriseDto
    //{
    //    public int Id { get; set; }

    //    public int StaffId { get; set; }

    //    public string? StaffName { get; set; }

    //    public string? RewardName { get; set; }

    //    public string? RewardFor { get; set; }

    //    public int Marks { get; set; }

    //    public DateTime SurpriseDate { get; set; }
    //}
    //public class SurpriseCreateDto
    //{
    //    public int      StaffId      { get; set; }
    //    public string?  Title        { get; set; }
    //    public string?  Description  { get; set; }
    //    public DateTime SurpriseDate { get; set; }
    //}

    //public class SurpriseCreateDto
    //{
    //    public int StaffId { get; set; }

    //    public string? RewardName { get; set; }

    //    public string? RewardFor { get; set; }

    //    public int Marks { get; set; }
    //}
    // ─── FOLLOW UP ────────────────────────────────────────────────────────────
    public class FollowUpDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime FollowUpDate { get; set; }
        public string? Status { get; set; }
    }
    public class FollowUpCreateDto
    {
        public int StaffId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime FollowUpDate { get; set; }
        public string? Status { get; set; } = "Pending";
    }

    // ─── SHIFT ────────────────────────────────────────────────────────────────
    public class ShiftDto
    {
        public int Id { get; set; }
        public string ShiftName { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
    }
    public class ShiftCreateDto
    {
        public string ShiftName { get; set; } = "";
        public string StartTime { get; set; } = ""; // "HH:mm"
        public string EndTime { get; set; } = "";
    }
    public class StaffShiftDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; } = "";
        public string ShiftType { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
    }

    public class StaffShiftCreateDto
    {
        public int StaffId { get; set; }
        public string ShiftType { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StartTime { get; set; } = ""; // "HH:mm"
        public string EndTime { get; set; } = "";   // "HH:mm"
    }



    //// ─── ATTENDANCE ───────────────────────────────────────────────────────────
    public class StaffDropdownDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class StaffAttendanceDto
    {
        public int Id { get; set; }
        public string InDate { get; set; } = "";
        public string OutDate { get; set; } = "";
        public string TotalHrs { get; set; } = "0";
        public string Status { get; set; } = "";
    }

    public class CreateStaffAttendanceDto
    {
        public int StaffId { get; set; }
        public DateTime InDate { get; set; }
        public DateTime OutDate { get; set; }
        public string Status { get; set; } = "Present";
    }

    public class StaffListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";   // always lowercase "name"
    }

    // ─── LEAVE ────────────────────────────────────────────────────────────────
    // DTOs/LeaveDto.cs
    public class LeaveDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = "";   // "Subject" in UI
        public string Remarks { get; set; } = "";  // "Others" in UI
        public string Status { get; set; } = "Pending";
    }

    // DTOs/LeaveCreateDto.cs
    public class LeaveCreateDto
    {
        public int StaffId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = "";
        public string Remarks { get; set; } = "";
    }

    // DTOs/StatusUpdateDto.cs  ← CRITICAL NEW DTO (fixes 500 on approve/reject)
    public class StatusUpdateDto
    {
        public string Status { get; set; } = "";
    }

    // DTOs/StaffDropdownDto.cs  ← NEW (fixes missing staff dropdown)
    //public class StaffDropdownDto
    //{
    //    public int Id { get; set; }
    //    public string StaffName { get; set; } = "";
    //}
    //public class CreateStaffAttendanceDto
    //{
    //    public int StaffId { get; set; }

    //    public DateTime InDate { get; set; }

    //    public DateTime OutDate { get; set; }

    //    public string Status { get; set; } = "";
    //}
    // ─── TASK ─────────────────────────────────────────────────────────────────
    //public class TaskDto
    //{
    //    public int Id { get; set; }

    //    public int AssignedToStaffId { get; set; }

    //    public string? AssignedToName { get; set; }

    //    public string Details { get; set; } = "";

    //    public DateTime? StartDate { get; set; }

    //    public DateTime? EndDate { get; set; }

    //    public string? Time { get; set; }

    //    public string Status { get; set; } = "";

    //    public string Priority { get; set; } = "";

    //    public string? Department { get; set; }

    //    public string? Category { get; set; }

    //    public string? Every { get; set; }

    //    public string? AttachmentPath { get; set; }

    //    public DateTime CreatedAt { get; set; }
    //}
    public class TaskDto
    {
        public int Id { get; set; }
        public int AssignedToStaffId { get; set; }
        public string AssignedToName { get; set; } = "";
        public string Details { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Time { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public string Department { get; set; } = "";
        public string Category { get; set; } = "";
        public string Every { get; set; } = "";
        public string? AttachmentPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class TaskCreateDto
    {
        public int AssignedToStaffId { get; set; }
        public string Details { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Time { get; set; } = "";

        // Status is always set to "Pending" on creation server-side;
        // the client does NOT send it on create, only on update.
        public string Status { get; set; } = "Pending";

        public string Priority { get; set; } = "Medium";
        public string Department { get; set; } = "";
        public string Category { get; set; } = "";
        public string Every { get; set; } = "";

        // Optional file upload
        public IFormFile? Attachment { get; set; }
    }


    public class TaskBulkCreateDto
    {

        public List<int> AssignedToStaffIds { get; set; } = new();
        public string Details { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Time { get; set; } = "";
        public string Priority { get; set; } = "Medium";
        public string Department { get; set; } = "";
        public string Category { get; set; } = "";
        public string Every { get; set; } = "";

        // Optional file upload (same attachment applied to all created tasks)
        public IFormFile? Attachment { get; set; }
    }

    public class TaskUpdateDto
    {
        public int AssignedToStaffId { get; set; }
        public string Details { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Time { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public string Department { get; set; } = "";
        public string Category { get; set; } = "";
        public string Every { get; set; } = "";

        // FIX: Original Update method never touched the attachment path.
        // Client can send a new file to replace the old one.
        public IFormFile? Attachment { get; set; }
    }

    //public class TaskCreateDto
    //{
    //    public int AssignedToStaffId { get; set; }

    //    public string Details { get; set; } = "";

    //    public DateTime? StartDate { get; set; }

    //    public DateTime? EndDate { get; set; }

    //    public string? Time { get; set; }

    //    public string Status { get; set; } = "Pending";

    //    public string Priority { get; set; } = "";

    //    public string? Department { get; set; }

    //    public string? Category { get; set; }

    //    public string? Every { get; set; }
    //}

    // ─── MOTIVATIONAL MESSAGE ─────────────────────────────────────────────────
    public class MotivationalMessageDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public string Author { get; set; } = "";
        public string VideoLink { get; set; } = "";
        public string? ImagePath { get; set; }
        public string Date { get; set; } = "";   // formatted for display
        public bool IsActive { get; set; }
    }

    public class MotivationalMessageCreateDto
    {
        public string Message { get; set; } = "";
        public string Author { get; set; } = "";
        public string VideoLink { get; set; } = "";
        public DateTime Date { get; set; }
        public bool IsActive { get; set; } = true;
    }
    // ─── DSI ──────────────────────────────────────────────────────────────────
    public class DSIDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Problem { get; set; } = "";
        public int Priority { get; set; }
        public string Message { get; set; } = "";
        public bool IsSolved { get; set; }
        public string? DocumentPath { get; set; }
        public string RecordDate { get; set; } = "";
    }

    public class DSICreateDto
    {
        public int StaffId { get; set; }
        public int? DepartmentId { get; set; }          // nullable — optional field
        public string Problem { get; set; } = "";
        public int Priority { get; set; } = 0;
        public string Message { get; set; } = "";
        public string IsSolved { get; set; } = "false"; // string — parsed manually in controller
        public DateTime RecordDate { get; set; } = DateTime.Today;
    }

    //staff policy
    public class StaffPolicyDto
    {
        public int Id { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string GoogleDriveLink { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // ─── DTOs/StaffPolicyCreateDto.cs ────────────────────────────────────────────
    public class StaffPolicyCreateDto
    {
        [Required] public string PolicyName { get; set; } = string.Empty;
        [Required] public string Department { get; set; } = string.Empty;
        [Required] public string GoogleDriveLink { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    // ─── HR POLICY ────────────────────────────────────────────────────────────

    public class HRPolicyDto
    {
        public int Id { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string GoogleDriveLink { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // DTOs/HRPolicyCreateDto.cs
    public class HRPolicyCreateDto
    {
        [Required]
        public string PolicyName { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string GoogleDriveLink { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    //public class HRPolicyDto
    //{
    //    public int    Id       { get; set; }
    //    public string Title    { get; set; } = "";
    //    public string? Content  { get; set; }
    //    public string? Category { get; set; }
    //    public bool   IsActive  { get; set; }
    //    public DateTime UpdatedAt { get; set; }
    //}
    //public class HRPolicyCreateDto
    //{
    //    public string  Title    { get; set; } = "";
    //    public string? Content  { get; set; }
    //    public string? Category { get; set; }
    //    public bool    IsActive { get; set; } = true;
    //}

    // ─── DASHBOARD ────────────────────────────────────────────────────────────
    public class DashboardStatsDto
    {
        public int TotalStaff { get; set; }

        public int PresentToday { get; set; }

        public int ClockedOut { get; set; }

        public int LeavesToday { get; set; }

        public int TasksDue { get; set; }

        public int PendingReviews { get; set; }

        public int NewFeedbacks { get; set; }

        public double DSIScore { get; set; }

        public List<LeaderboardEntry> Leaderboard { get; set; } = new();

        public List<BirthdayEntry> Birthdays { get; set; } = new();
    }
    //public class DashboardStatsDto
    //{
    //    public int TotalStaff       { get; set; }
    //    public int PresentToday     { get; set; }
    //    public int AbsentToday      { get; set; }
    //    public int LeavesToday      { get; set; }
    //    public int TasksDue         { get; set; }
    //    public int PendingReviews   { get; set; }
    //    public int NewFeedbacks     { get; set; }
    //    public double DSIScore      { get; set; }
    //    public List<LeaderboardEntry> Leaderboard { get; set; } = new();
    //    public List<BirthdayEntry>    Birthdays   { get; set; } = new();
    //}
    //public class LeaderboardEntry
    //{
    //    public string StaffName { get; set; } = "";
    //    public int    Score     { get; set; }
    //    public string? Department { get; set; }
    //}
    public class LeaderboardEntry
    {
        public string StaffName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public int Score { get; set; }
    }

    public class BirthdayEntry
    {
        public string StaffName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
    }
    //public class BirthdayEntry
    //{
    //    public string    StaffName  { get; set; } = "";
    //    public DateTime  DateOfBirth { get; set; }
    //}
}
