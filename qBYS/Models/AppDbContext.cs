﻿using Microsoft.EntityFrameworkCore;
using qBYS.Models;

namespace qBYS.Models
{
    public class AppDbContext : DbContext
    {
        // DbContextOptions<AppDbContext> parametresi ile DbContext'i yapılandırıyoruz
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Advisors> Advisors { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Transcripts> Transcripts { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourseSelections> StudentCourseSelections { get; set; }
        public DbSet<CourseSelectionHistory> CourseSelectionHistory { get; set; }
        public DbSet<NonConfirmedSelections> NonConfirmedSelections { get; set; }
        public DbSet<CourseQuotas> CourseQuotas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourseSelections>()
                .HasOne(scs => scs.Student)
                .WithMany(s => s.StudentCourseSelections)
                .HasForeignKey(scs => scs.StudentID);

            modelBuilder.Entity<StudentCourseSelections>()
                .HasOne(scs => scs.Course)
                .WithMany(c => c.StudentCourseSelections)
                .HasForeignKey(scs => scs.CourseID);
            modelBuilder.Entity<Student>()
                 .HasOne(s => s.Advisors)
                 .WithMany(a => a.Students)
                 .HasForeignKey(s => s.AdvisorID)
                 .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Advisors>()
             .HasMany(a => a.Students)
             .WithOne(s => s.Advisors)
             .HasForeignKey(s => s.AdvisorID)
             .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CourseSelectionHistory>()
              .HasOne(c => c.Student)
              .WithMany()
              .HasForeignKey(c => c.StudentID);
            modelBuilder.Entity<Student>()
    .HasMany(s => s.StudentCourseSelections)
    .WithOne(scs => scs.Student)
    .HasForeignKey(scs => scs.StudentID)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseQuotas>()
    .HasOne(cq => cq.Course)
    .WithOne()  // Burada `WithOne()` çünkü her dersin sadece bir kontenjanı var
    .HasForeignKey<CourseQuotas>(cq => cq.CourseId);
            modelBuilder.Entity<NonConfirmedSelections>()
    .HasOne(ns => ns.Student)
    .WithMany(s => s.NonConfirmedSelections)
    .HasForeignKey(ns => ns.StudentId)
    .OnDelete(DeleteBehavior.Restrict);  // Silme davranışını ayarlıyoruz

            // NonConfirmedSelection ve Course arasındaki ilişki
            modelBuilder.Entity<NonConfirmedSelections>()
                .HasOne(ns => ns.Course)
                .WithMany(c => c.NonConfirmedSelections)
                .HasForeignKey(ns => ns.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Keyless (anahtarsız) entity için
            modelBuilder.Entity<CourseQuotas>()
                .HasNoKey(); // Bu satırı ekleyin

           


        }
    }
}