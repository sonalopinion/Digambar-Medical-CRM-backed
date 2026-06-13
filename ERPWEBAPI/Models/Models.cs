using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElevateERP.API.Models
{
    // ─── FIRM ─────────────────────────────────────────────────────────────────
    public class Firm
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(200)] public string FirmName { get; set; } = "";
        [MaxLength(500)] public string? Address { get; set; }
        [MaxLength(20)]  public string? Phone    { get; set; }
        [MaxLength(200)] public string? Email    { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Staff> Staff { get; set; } = new List<Staff>();
    }

    // ─── DEPARTMENT ───────────────────────────────────────────────────────────
    public class Department
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(200)] public string DepartmentName { get; set; } = "";
        [MaxLength(500)] public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Staff> Staff { get; set; } = new List<Staff>();
    }

    // ─── CATEGORY ─────────────────────────────────────────────────────────────
    public class Category
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(200)] public string CategoryName { get; set; } = "";
        [MaxLength(500)] public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ─── STAFF ────────────────────────────────────────────────────────────────
    public class Staff
    {
        [Key] public int Id { get; set; }
        public int FirmId { get; set; }
        [ForeignKey("FirmId")] public Firm? Firm { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")] public Department? Department { get; set; }
        [Required, MaxLength(200)] public string StaffName    { get; set; } = "";
        [MaxLength(20)]  public string? Role            { get; set; } = "Staff";
        [MaxLength(20)]  public string? MobileNumber    { get; set; }
        [MaxLength(200)] public string? EmailId         { get; set; }
        [MaxLength(50)]  public string? Username        { get; set; }
        [MaxLength(200)] public string  PasswordHash    { get; set; } = "";

        [MaxLength(200)] public string? PlainPassword { get; set; }
        [MaxLength(500)] public string? Address         { get; set; }
        public DateTime? JoiningDate    { get; set; }
        public DateTime? DateOfBirth    { get; set; }
        [MaxLength(200)] public string? Education       { get; set; }
        [MaxLength(200)] public string? Skill           { get; set; }
        [MaxLength(20)]  public string? AdharNumber     { get; set; }
        [MaxLength(20)]  public string? PanNumber       { get; set; }
        [MaxLength(30)]  public string? AccountNumber   { get; set; }
        [MaxLength(500)] public string? InsuranceDetails { get; set; }
        public DateTime? InsuranceStart { get; set; }
        public DateTime? InsuranceEnd   { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Attendance>   Attendances  { get; set; } = new List<Attendance>();
        public ICollection<Leave>        Leaves       { get; set; } = new List<Leave>();
        public ICollection<Feedback>     Feedbacks    { get; set; } = new List<Feedback>();
        public ICollection<StaffShift>   StaffShifts  { get; set; } = new List<StaffShift>();
    }

    // ─── REWARD ───────────────────────────────────────────────────────────────
    public class Reward
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(200)] public string RewardName  { get; set; } = "";
        [MaxLength(500)] public string? Description { get; set; }
        public decimal? PointsValue  { get; set; }
        public DateTime CreatedAt    { get; set; } = DateTime.UtcNow;
        public ICollection<StaffReward> StaffRewards { get; set; } = new List<StaffReward>();
    }

    // ─── STAFF REWARD ─────────────────────────────────────────────────────────
    public class StaffReward
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public Staff? Staff { get; set; }

        public int PointsValue { get; set; }   // points stored directly, no Reward FK needed
        public string Notes { get; set; } = "";
        public DateTime AwardedAt { get; set; }
    }

    // ─── PERFORMANCE ──────────────────────────────────────────────────────────
    //public class Performance
    //{
    //    [Key] public int Id { get; set; }
    //    public int StaffId  { get; set; }
    //    [ForeignKey("StaffId")] public Staff? Staff { get; set; }
    //    public int    Score       { get; set; }     // 1-10
    //    [MaxLength(500)] public string? Notes   { get; set; }
    //    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    //    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
    //}
    public class Performance
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Marks { get; set; } = string.Empty;
    }

    // ─── FEEDBACK ─────────────────────────────────────────────────────────────
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Option1 { get; set; } = string.Empty;

        public string Option2 { get; set; } = string.Empty;

        public string Option3 { get; set; } = string.Empty;

        public string Option4 { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class StaffFeedback
    {
        public int Id { get; set; }

        public int FeedbackId { get; set; }

        public string StaffName { get; set; } = string.Empty;

        public string SelectedAnswer { get; set; } = string.Empty;

        public DateTime FeedbackDate { get; set; }

        public Feedback Feedback { get; set; }
    }

    // ─── SURPRISE ─────────────────────────────────────────────────────────────
    public class Surprise
    {
        public int Id { get; set; }

        public int StaffId { get; set; }

        public Staff? Staff { get; set; }

        // UI -> rewardName
        public string? Title { get; set; }

        // UI -> rewardFor
        public string? Description { get; set; }

        // NEW
        public int Marks { get; set; }

        public DateTime SurpriseDate { get; set; }
    }
    //public class Surprise
    //{
    //    [Key] public int Id { get; set; }
    //    public int StaffId  { get; set; }
    //    [ForeignKey("StaffId")] public Staff? Staff { get; set; }
    //    [MaxLength(200)] public string? Title       { get; set; }
    //    [MaxLength(1000)] public string? Description { get; set; }
    //    public DateTime SurpriseDate { get; set; } = DateTime.UtcNow;
    //    public DateTime CreatedAt    { get; set; } = DateTime.UtcNow;
    //}

    // ─── FOLLOW UP ────────────────────────────────────────────────────────────
    public class FollowUp
    {
        [Key] public int Id { get; set; }
        public int StaffId  { get; set; }
        [ForeignKey("StaffId")] public Staff? Staff { get; set; }
        [MaxLength(200)] public string? Title        { get; set; }
        [MaxLength(1000)] public string? Description { get; set; }
        public DateTime FollowUpDate { get; set; }
        [MaxLength(50)] public string? Status  { get; set; } = "Pending"; // Pending / Done
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ─── SHIFT ────────────────────────────────────────────────────────────────
    public class Shift
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string ShiftName { get; set; } = "";
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime   { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<StaffShift> StaffShifts { get; set; } = new List<StaffShift>();
    }

    // ─── STAFF SHIFT ──────────────────────────────────────────────────────────
    //public class StaffShift
    //{
    //    [Key] public int Id { get; set; }
    //    public int StaffId { get; set; }
    //    [ForeignKey("StaffId")] public Staff? Staff { get; set; }
    //    public int ShiftId { get; set; }
    //    [ForeignKey("ShiftId")] public Shift? Shift { get; set; }
    //    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    //}
    public class ImportReport
    {
        public int Id { get; set; }
        public string ReportName { get; set; } = string.Empty;   // original file name
        public string DriveLink { get; set; } = string.Empty;    // Google Drive link
        public DateTime ReportDate { get; set; }                 // date of report
        public string FilePath { get; set; } = string.Empty;     // saved file path on server
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class StaffShift
    {
        [Key]
        public int Id { get; set; }

        public int StaffId { get; set; }

        [ForeignKey("StaffId")]
        public Staff? Staff { get; set; }

        [MaxLength(20)]
        public string? ShiftType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }
    }
    //public class StaffShift
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    public int StaffId { get; set; }

    //    [ForeignKey("StaffId")]
    //    public Staff? Staff { get; set; }

    //    [MaxLength(20)]
    //    public string? ShiftType { get; set; }

    //    public DateTime? StartDate { get; set; }

    //    public DateTime? EndDate { get; set; }

    //    public TimeSpan? StartTime { get; set; }

    //    public TimeSpan? EndTime { get; set; }
    //}

    // ─── ATTENDANCE ───────────────────────────────────────────────────────────
    public class Attendance
    {
        public int Id { get; set; }

        public int StaffId { get; set; }

        public Staff? Staff { get; set; }

        public DateTime Date { get; set; }

        public DateTime? ClockIn { get; set; }

        public DateTime? ClockOut { get; set; }

        public string Status { get; set; } = "";

        public string? Notes { get; set; }
    }
    // ─── LEAVE ────────────────────────────────────────────────────────────────
    // Models/Leave.cs
    public class Leave
    {
        [Key]
        public int Id { get; set; }
        public int StaffId { get; set; }
        [ForeignKey("StaffId")]
        public Staff? Staff { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(500)]
        public string? Reason { get; set; }   // maps to UI "Subject"
        [MaxLength(500)]
        public string? Remarks { get; set; }  // maps to UI "Others"
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // LeaveType REMOVED — was commented out everywhere and caused
        // the duplicate-controller crash when the second version tried to use it
    }

    // ─── TASK ─────────────────────────────────────────────────────────────────
    public class TaskItem
    {
        public int Id { get; set; }

        public int AssignedToStaffId { get; set; }

        public string Details { get; set; } = "";

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Time { get; set; }

        public string Status { get; set; } = "Pending";

        public string Priority { get; set; } = "";

        public string? Department { get; set; }

        public string? Category { get; set; }

        public string? Every { get; set; }

        public string? AttachmentPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Staff? AssignedTo { get; set; }
    }

    // ─── MOTIVATIONAL MESSAGE ─────────────────────────────────────────────────
    public class MotivationalMessage
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public string Author { get; set; } = "";
        public string VideoLink { get; set; } = "";   // added
        public string? ImagePath { get; set; }          // added (stored file path)
        public DateTime Date { get; set; }          // added (user-supplied date)
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ─── DSI (Daily Staff Index) ──────────────────────────────────────────────
    public class DSIRecord
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public Staff? Staff { get; set; }
        public int? DepartmentId { get; set; }
        public string Problem { get; set; } = "";
        public int Priority { get; set; } = 0;
        public string Message { get; set; } = "";
        public bool IsSolved { get; set; } = false;
        public string? DocumentPath { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.Today;
    }
    // ─── HR POLICY ────────────────────────────────────────────────────────────
    //public class HRPolicy
    //{
    //    [Key] public int Id { get; set; }
    //    [Required, MaxLength(200)] public string Title    { get; set; } = "";
    //    [MaxLength(5000)] public string? Content          { get; set; }
    //    [MaxLength(100)] public string? Category          { get; set; }
    //    public bool IsActive  { get; set; } = true;
    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    //}
    public class HRPolicy
    {
        public int Id { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string GoogleDriveLink { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    // ─── Models/StaffPolicy.cs ───────────────────────────────────────────────────
    public class StaffPolicy
    {
        public int Id { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string GoogleDriveLink { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // ─── APP USER (Admin login) ───────────────────────────────────────────────
    public class AppUser
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Username     { get; set; } = "";
        [Required]                 public string PasswordHash { get; set; } = "";
        [MaxLength(50)]            public string Role         { get; set; } = "Admin";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
