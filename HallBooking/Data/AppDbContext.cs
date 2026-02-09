using HallBooking.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallBooking.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<HallRental> HallRentals => Set<HallRental>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();
        public DbSet<CourseSession> CourseSessions => Set<CourseSession>();
     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=hall_booking_db;Username=postgres;Password=postgres");
            }
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(course =>
            {
                course.Property(x => x.Title).IsRequired().HasMaxLength(75);
                course.Property(x => x.InstructorName).IsRequired().HasMaxLength(50);
                course.Property(x => x.Description).IsRequired().HasMaxLength(1000);
                course.Property(x => x.PricePerParticipant).IsRequired().HasPrecision(7, 2);
                course.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Member>(member =>
            {
                member.Property(x => x.Name).IsRequired().HasMaxLength(50);
                member.Property(x => x.Email).IsRequired().HasMaxLength(100);
                member.HasIndex(x => x.Email).IsUnique();
                member.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

                member.HasData(
                    new Member()
                    {
                        Id = 1,
                        Name = "Anna Andersson",
                        Email = "anna@mail.com"
                    },
                    new Member
                    {
                        Id = 2,
                        Name = "Björn Berg",
                        Email = "brotherbear@mail.com"
                    },
                    new Member
                    {
                        Id = 3,
                        Name = "Cecilia Carlsson",
                        Email = "cissi@example.com"
                    });
            });

            modelBuilder.Entity<CourseEnrollment>(enrollment =>
            {
                enrollment.HasKey(x => x.Id);

                //Index to avoid member enrolling for the same course twice
                enrollment.HasIndex(x => new { x.MemberId, x.CourseId }).IsUnique();

                enrollment.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

                enrollment.HasOne(x => x.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

                enrollment.HasOne(x => x.Member)
                .WithMany(m => m.CourseEnrollments)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Payment>(payment =>
            {
                payment.Property(x => x.Amount).HasPrecision(7, 2);
                payment.Property(x => x.Status).HasDefaultValue(PaymentStatus.Pending);
                payment.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

                payment.HasOne(x => x.Member)
                .WithMany(m => m.Payments)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

                payment.HasOne(x => x.CourseEnrollment)
                .WithMany(c => c.Payments)
                .HasForeignKey(x => x.CourseEnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);

                payment.HasOne(x => x.HallRental)
                .WithMany(h => h.Payments)
                .HasForeignKey(x => x.HallRentalId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<HallRental>(hall =>
            {
                hall.Property(x => x.StartTime).HasColumnType("timestamp without time zone");
                hall.Property(x => x.EndTime).HasColumnType("timestamp without time zone");

                hall.Property(x => x.Status).HasDefaultValue(HallRentalStatus.Booked);
                hall.Property(x => x.HourlyPrice).HasPrecision(6, 2);
                hall.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                hall.Ignore(x => x.TotalPrice);

                hall.HasOne(x => x.Member)
                    .WithMany(m => m.HallRentals)
                    .HasForeignKey(x => x.MemberId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CourseSession>(session =>
            {
                session.HasKey(x => x.Id);

                session.Property(x => x.StartTime).HasColumnType("timestamp without time zone");
                session.Property(x => x.EndTime).HasColumnType("timestamp without time zone");

                session.Property(x => x.CreatedAt).HasDefaultValueSql("now()");

                session.HasOne(x => x.Course)
                    .WithMany(c => c.Sessions)
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                session.HasIndex(x => new { x.StartTime, x.EndTime });
                session.HasIndex(x => x.CourseId);
            });
        }
    }
}
