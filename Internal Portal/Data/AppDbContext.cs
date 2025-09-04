using Internal_Portal.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Internal_Portal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserRoleMaster> UserRoleMaster { get; set; }
        public DbSet<Register> Register { get; set; }
        public DbSet<Login> Login { get; set; }

        public DbSet<CompanyMaster> Companies { get; set; }
        public DbSet<VendorMaster> Vendors { get; set; }
        public DbSet<CustomerMaster> Customers { get; set; }
        public DbSet<EmployeeMaster> Employees { get; set; }
        public DbSet<ProjectMaster> Projects { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployees { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<TimesheetEntry> TimesheetEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint for Email
            modelBuilder.Entity<Register>()
                .HasIndex(r => r.Email)
                .IsUnique();

            // Configure one-to-many relationship
            modelBuilder.Entity<VendorMaster>()
                        .HasOne(v => v.CompanyMaster)
                        .WithMany(c => c.Vendors)
                        .HasForeignKey(v => v.CompanyCode);
        }
    }

}

