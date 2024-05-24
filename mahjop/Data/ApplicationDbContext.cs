using mahjop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace mahjop.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Department> Departments   { get; set; }
        public DbSet<Doctor> Doctors   { get; set; }
        public DbSet<WorkingHours> WorkingHours   { get; set; }
        public DbSet<Appointment> Appointments   { get; set; }
        public DbSet<Pharmacy> Pharmacies   { get; set; }
        public DbSet<Lab> Labs   { get; set; }
        public DbSet<Patient> Patients   { get; set; }
        public DbSet<Test> Tests   { get; set; }
        public DbSet<MedicationCategory> MedicationCategories   { get; set; }
        public DbSet<Medication> Medications   { get; set; }
        public DbSet<MedicalHistory> MedicalHistories   { get; set; }
        public DbSet<PageView> PageViews   { get; set; }
        public DbSet<HealthAssessment> HealthAssessments   { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
          
            builder.Entity<User>()
                .ToTable("Users", "security");
            builder.Entity<IdentityRole>().ToTable("Roles", "security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles"  , "security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "security");




        }
        public DbSet<mahjop.Models.Appointment> Appointment { get; set; }
    }
}
