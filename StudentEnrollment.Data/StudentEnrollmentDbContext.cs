using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StudentEnrollment.Data.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentEnrollment.Data
{
    public class StudentEnrollmentDbContext : IdentityDbContext
    {
        public StudentEnrollmentDbContext(DbContextOptions<StudentEnrollmentDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Additional model configurations can be added here
            builder.ApplyConfiguration(new CourseConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
    }

    public class StudentEnrollmentDbContextFactory : IDesignTimeDbContextFactory<StudentEnrollmentDbContext>
    {
        public StudentEnrollmentDbContext CreateDbContext(string[] args)
        {
            // This method is used by EF Core tools to create the DbContext at design time.

            // Get the environment variable for ASP.NET Core environment
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Load the configuration from appsettings.json
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Get connection string from configuration
            var optionsBuilder = new DbContextOptionsBuilder<StudentEnrollmentDbContext>();
            var connectionString = config.GetConnectionString("StudentEnrollmentDbConnection");

            // Configure the DbContext to use SQL Server with the connection string
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
            return new StudentEnrollmentDbContext(optionsBuilder.Options);
        }
    }
}
