using Microsoft.EntityFrameworkCore;
using ElevateERP.API.Models;

namespace ElevateERP.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AppUser>            AppUsers            { get; set; }
        public DbSet<Firm>               Firms               { get; set; }
        public DbSet<Department>         Departments         { get; set; }
        public DbSet<Category>           Categories          { get; set; }
        public DbSet<Staff>              Staff               { get; set; }
        public DbSet<Reward>             Rewards             { get; set; }
        public DbSet<StaffReward>        StaffRewards        { get; set; }
        public DbSet<Performance>        Performances        { get; set; }
        public DbSet<Feedback>           Feedbacks           { get; set; }

        public DbSet<StaffFeedback>    StaffFeedbacks { get; set; }
        public DbSet<ImportReport> ImportReports { get; set; }
        public DbSet<Surprise>           Surprises           { get; set; }
        public DbSet<FollowUp>           FollowUps           { get; set; }
        public DbSet<Shift>              Shifts              { get; set; }
        public DbSet<StaffShift>         StaffShifts         { get; set; }
        public DbSet<Attendance>         Attendances         { get; set; }
        public DbSet<Leave>              Leaves              { get; set; }
        public DbSet<TaskItem>           Tasks               { get; set; }
        public DbSet<MotivationalMessage> MotivationalMessages { get; set; }
        public DbSet<DSIRecord>          DSIRecords          { get; set; }
        public DbSet<HRPolicy>           HRPolicies          { get; set; }

        public DbSet<StaffPolicy> StaffPolicies { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<TaskItem>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToStaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed default admin
            mb.Entity<AppUser>().HasData(new AppUser
            {
                Id           = 1,
                Username     = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role         = "Admin",
                CreatedAt    = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Seed default firm
            mb.Entity<Firm>().HasData(new Firm
            {
                Id        = 1,
                FirmName  = "DIGAMBER MEDICAL",
                Address   = "Main Road, Pune",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Seed default department
            mb.Entity<Department>().HasData(new Department
            {
                Id             = 1,
                DepartmentName = "2_PHARMACIST",
                CreatedAt      = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
