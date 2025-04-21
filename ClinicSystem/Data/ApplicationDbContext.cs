
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System;
using ClinicSystem.Models;
namespace ClinicSystem.Data
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet< Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ClinicSystem;Integrated Security=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Doctor>().HasData(new List<Doctor>
            {
            new Doctor { Id = 1, Name = "Dr. John Smith", Specialization = "Cardiology", Img = "doctor1.jpg" },
            new Doctor { Id = 2, Name = "Dr. Sarah Johnson", Specialization = "Pediatrics", Img = "doctor2.jpg" },
            new Doctor { Id = 3, Name = "Dr. Emily Davis", Specialization = "Dermatology", Img = "doctor4.jpg" },
             new Doctor { Id = 4, Name = "Dr. Michael Lee", Specialization = "Orthopedics", Img = "doctor3.jpg" },
            new Doctor { Id = 5, Name = "Dr. William Clark", Specialization = "Neurology", Img = "doctor5.jpg" },
            });
        }
    }
    
}
